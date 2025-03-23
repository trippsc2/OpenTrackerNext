using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Aggregation;
using DynamicData.Alias;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Results;
using OpenTrackerNext.Core.ViewModels.Dialogs;
using OpenTrackerNext.SplatRegistration;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Editor.Documents;

/// <inheritdoc cref="IDocumentService" />
[Splat(RegisterAsType = typeof(IDocumentService))]
[SplatSingleInstance]
public sealed class DocumentService : ReactiveObject, IDocumentService
{
    private readonly CompositeDisposable _disposables = new();
    
    private readonly SourceList<IDocument> _documents = new();
    
    private readonly Dictionary<DocumentSide, Subject<IDocument>> _activationRequests = new()
    {
        { DocumentSide.Left, new Subject<IDocument>() },
        { DocumentSide.Right, new Subject<IDocument>() }
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentService"/> class.
    /// </summary>
    public DocumentService()
    {
        ActivationRequests = new Dictionary<DocumentSide, IObservable<IDocument>>
        {
            { DocumentSide.Left, _activationRequests[DocumentSide.Left].AsObservable() },
            { DocumentSide.Right, _activationRequests[DocumentSide.Right].AsObservable() }
        };

        _documents
            .Connect()
            .AutoRefresh(x => x.Side)
            .Where(x => x.Side == DocumentSide.Right)
            .Count()
            .Select(x => x > 0)
            .ToPropertyEx(this, x => x.RightHasDocuments)
            .DisposeWith(_disposables);

        _documents
            .Connect()
            .AutoRefresh(x => x.Side)
            .Where(x => x.Side == DocumentSide.Left)
            .Count()
            .Select(x => x > 0)
            .ToPropertyEx(this, x => x.LeftHasDocuments)
            .DisposeWith(_disposables);

        this.WhenAnyValue(x => x.LeftHasDocuments)
            .Where(x => !x)
            .Subscribe(_ => MoveRightDocumentsToLeft())
            .DisposeWith(_disposables);
    }

    /// <inheritdoc/>
    [ObservableAsProperty]
    public bool RightHasDocuments { get; }

    /// <inheritdoc/>
    public IDictionary<DocumentSide, IReactiveProperty<IDocument?>> ActiveDocuments { get; } =
        new Dictionary<DocumentSide, IReactiveProperty<IDocument?>>
        {
            { DocumentSide.Left, new ReactiveProperty<IDocument?>() },
            { DocumentSide.Right, new ReactiveProperty<IDocument?>() }
        };

    /// <inheritdoc/>
    public IDictionary<DocumentSide, IObservable<IDocument>> ActivationRequests { get; }

    /// <inheritdoc/>
    public Interaction<YesNoCancelDialogViewModel, YesNoCancelDialogResult> RequestSaveDialog { get; } = new();

    /// <inheritdoc/>
    public Interaction<ExceptionDialogViewModel, Unit> ExceptionDialog { get; } = new();
    
    [ObservableAsProperty]
    private bool LeftHasDocuments { get; }

    /// <inheritdoc/>
    public void Dispose()
    {
        CloseAll();
        _disposables.Dispose();
        _documents.Dispose();
    }

    /// <inheritdoc/>
    public IObservable<IChangeSet<IDocument>> Connect()
    {
        return _documents.Connect();
    }

    /// <inheritdoc/>
    public void Open<TData>(IDocumentFile<TData> file)
        where TData : class, ITitledDocumentData<TData>, new()
    {
        var document = _documents.Items
            .FirstOrDefault(x => x is IDocument<TData> existingDocument && existingDocument.File == file);

        if (document is null)
        {
            document = new Document<TData>(file);
            _documents.Add(document);
        }
        
        Activate(document);
    }
    
    /// <inheritdoc/>
    public void MoveToOtherSide(IDocument document)
    {
        document.Side = document.Side == DocumentSide.Left
            ? DocumentSide.Right
            : DocumentSide.Left;
        
        Activate(document);
    }

    /// <inheritdoc/>
    public async Task<OperationResult> TryHandleUnsavedChangesAsync(IDocument document)
    {
        const string saveChangesTitle = "Save Changes?";

        if (!_documents.Items.Contains(document))
        {
            return OperationResult.Success;
        }
        
        if (!document.IsUnsaved)
        {
            return OperationResult.Success;
        }

        Activate(document);

        var dialog = new YesNoCancelDialogViewModel(DialogIcon.Question)
        {
            Title = saveChangesTitle,
            Message = $"Do you want to save changes to '{document.BaseTitle}'?"
        };

        var result = await RequestSaveDialog.Handle(dialog);

        return await result
            .MatchAsync(
                _ => TrySaveAsync(document),
                _ => OperationResult.Success,
                _ => OperationResult.Cancel)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<OperationResult> TryHandleAllUnsavedChangesAsync()
    {
        foreach (var document in _documents.Items)
        {
            var result = await TryHandleUnsavedChangesAsync(document).ConfigureAwait(false);

            if (result.IsCancel)
            {
                return result;
            }
        }
        
        return OperationResult.Success;
    }

    /// <inheritdoc/>
    public async Task TryCloseAsync(IDocument document)
    {
        var result = await TryHandleUnsavedChangesAsync(document).ConfigureAwait(false);
        
        if (result.IsCancel)
        {
            return;
        }

        Close(document);
    }

    /// <inheritdoc/>
    public void Close<TData>(IDocumentFile<TData> file)
        where TData : class, ITitledDocumentData<TData>, new()
    {
        var document = _documents.Items
            .FirstOrDefault(x => x is IDocument<TData> existingDocument && existingDocument.File == file);

        if (document is null)
        {
            return;
        }

        Close(document);
    }

    /// <inheritdoc/>
    public void Close(IDocument document)
    {
        if (_documents.Items.Contains(document))
        {
            _documents.Remove(document);
        }
        
        if (ActiveDocuments[document.Side].Value == document)
        {
            ActiveDocuments[document.Side].Value = null;
        }
        
        document.Dispose();
    }

    /// <inheritdoc/>
    public void CloseAll()
    {
        foreach (var document in _documents.Items)
        {
            Close(document);
        }
    }

    /// <summary>
    /// Adds a new document.
    /// </summary>
    /// <param name="document">
    /// The <see cref="IDocument"/> to add.
    /// </param>
    /// <remarks>
    /// This method is used for testing purposes only.
    /// </remarks>
    [ExcludeFromCodeCoverage]
    public void Add(IDocument document)
    {
        _documents.Add(document);
    }

    /// <summary>
    /// Activates the specified document.
    /// </summary>
    /// <param name="document">
    /// The <see cref="IDocument"/> to activate.
    /// </param>
    /// <remarks>
    /// This method is used for testing purposes only.
    /// </remarks>
    [ExcludeFromCodeCoverage]
    public void Activate(IDocument document)
    {
        if (!_documents.Items.Contains(document))
        {
            return;
        }
        
        _activationRequests[document.Side].OnNext(document);
    }

    private async Task<OperationResult> TrySaveAsync(IDocument document)
    {
        try
        {
            await document.SaveAsync();
            return OperationResult.Success;
        }
        catch (Exception exception)
        {
            const string saveFailedTitle = "Save Failed";
            
            var dialog = new ExceptionDialogViewModel(exception, saveFailedTitle);
            await ExceptionDialog.Handle(dialog);
            
            return OperationResult.Cancel;
        }
    }

    private void MoveRightDocumentsToLeft()
    {
        if (!RightHasDocuments)
        {
            return;
        }
        
        var activeDocument = ActiveDocuments[DocumentSide.Right].Value;

        foreach (var document in _documents.Items)
        {
            document.Side = DocumentSide.Left;
        }

        if (activeDocument is null)
        {
            return;
        }

        Activate(activeDocument);
    }
}