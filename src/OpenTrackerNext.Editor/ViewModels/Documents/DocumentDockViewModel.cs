using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData;
using OpenTrackerNext.Core.ViewModels;
using OpenTrackerNext.Editor.Documents;
using OpenTrackerNext.SplatRegistration;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Editor.ViewModels.Documents;

/// <summary>
/// Represents a document dock control view model.
/// </summary>
[Splat(RegisterAsType = typeof(Factory))]
public sealed class DocumentDockViewModel : ViewModel
{
    private readonly DocumentViewModel.Factory _documentFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentDockViewModel"/> class.
    /// </summary>
    /// <param name="documentService">
    /// The <see cref="IDocumentService"/>.
    /// </param>
    /// <param name="documentFactory">
    /// A factory method for creating new <see cref="DocumentViewModel"/> objects.
    /// </param>
    /// <param name="side">
    /// A <see cref="DocumentSide"/> representing the side of the document dock.
    /// </param>
    public DocumentDockViewModel(
        IDocumentService documentService,
        DocumentViewModel.Factory documentFactory,
        DocumentSide side)
    {
        _documentFactory = documentFactory;

        var activeDocumentProperty = documentService.ActiveDocuments[side];
        
        this.WhenActivated(
            disposables =>
            {
                documentService
                    .Connect()
                    .AutoRefresh(x => x.Side)
                    .Filter(x => x.Side == side)
                    .Transform(CreateDocumentViewModel)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(out var documents)
                    .Subscribe()
                    .DisposeWith(disposables);
                
                Documents = documents;

                documentService.ActivationRequests[side]
                    .SubscribeOn(RxApp.MainThreadScheduler)
                    .Select(document => Observable.FromAsync(() => SetActiveDocumentFromPropertyValueAsync(document)))
                    .Concat()
                    .Subscribe()
                    .DisposeWith(disposables);

                this.WhenAnyValue(x => x.ActiveDocument)
                    .Subscribe(x => activeDocumentProperty.Value = x?.Document)
                    .DisposeWith(disposables);
            });
    }
    
    /// <summary>
    /// A factory for creating new <see cref="DocumentDockViewModel"/> objects.
    /// </summary>
    /// <param name="side">
    /// A <see cref="DocumentSide"/> representing the side of the document dock.
    /// </param>
    /// <returns>
    /// A new <see cref="DocumentDockViewModel"/> object.
    /// </returns>
    public delegate DocumentDockViewModel Factory(DocumentSide side);

    /// <summary>
    /// Gets a <see cref="ReadOnlyObservableCollection{T}"/> of <see cref="DocumentViewModel"/> containing the document
    /// view models in the dock.
    /// </summary>
    [Reactive]
    public ReadOnlyObservableCollection<DocumentViewModel> Documents { get; private set; } = null!;
    
    /// <summary>
    /// Gets or sets a <see cref="DocumentViewModel"/> representing the active document.
    /// </summary>
    [Reactive]
    public DocumentViewModel? ActiveDocument { get; set; }
    
    private DocumentViewModel CreateDocumentViewModel(IDocument document)
    {
        return _documentFactory(document);
    }

    private async Task SetActiveDocumentFromPropertyValueAsync(IDocument document)
    {
        if (ActiveDocument?.Document == document)
        {
            return;
        }

        while (Documents.All(x => x.Document != document))
        {
            await Task.Delay(1).ConfigureAwait(true);
        }

        var documentViewModel = Documents.First(x => x.Document == document);
        
        ActiveDocument = documentViewModel;
    }
}