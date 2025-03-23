using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Kernel;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Ids;
using OpenTrackerNext.Core.Results;
using OpenTrackerNext.Core.Storage;
using OpenTrackerNext.Core.Validation;
using OpenTrackerNext.Core.ViewModels.Dialogs;
using OpenTrackerNext.Editor.Documents;
using ReactiveUI;

namespace OpenTrackerNext.Editor.Pack;

/// <inheritdoc />
public abstract class PackFolderService<TData, TId> : IPackFolderService<TData, TId>
    where TData : class, ITitledDocumentData<TData>, new()
    where TId : struct, IGuidId<TId>
{
    private readonly IDocumentService _documentService;
    private readonly IDocumentFile<TData, TId>.Factory _fileFactory;
    private readonly SourceCache<IDocumentFile<TData, TId>, TId> _files = new(x => x.Id);
    private IStorageFolder? _folder;

    /// <summary>
    /// Initializes a new instance of the <see cref="PackFolderService{TFile,TId}"/> class.
    /// </summary>
    /// <param name="documentService">
    /// The <see cref="IDocumentService"/>.
    /// </param>
    /// <param name="fileFactory">
    /// An Autofac factory for creating new <see cref="IDocumentFile{TData,TId}"/> objects.
    /// </param>
    /// <param name="folderName">
    /// A <see cref="string"/> representing the name of the folder.
    /// </param>
    /// <param name="itemName">
    /// A <see cref="string"/> representing the name of the item.
    /// </param>
    protected PackFolderService(
        IDocumentService documentService,
        IDocumentFile<TData, TId>.Factory fileFactory,
        string folderName,
        string itemName)
    {
        _documentService = documentService;
        _fileFactory = fileFactory;
        FolderName = folderName;
        ItemName = itemName;

        NameValidationRules =
        [
            new ValidationRule<string?>
            {
                Rule = name => !string.IsNullOrWhiteSpace(name),
                FailureMessage = "Name cannot be empty."
            },
            new ValidationRule<string?>
            {
                Rule = name => _files.Items.All(x => x.FriendlyId != name),
                FailureMessage = "Name must be unique."
            }
        ];
    }

    /// <inheritdoc/>
    public string FolderName { get; }

    /// <inheritdoc/>
    public string ItemName { get; }

    /// <inheritdoc/>
    public List<ValidationRule<string?>> NameValidationRules { get; }

    /// <inheritdoc/>
    public Interaction<TextBoxDialogViewModel, OperationResult> AddFileDialog { get; } = new();
    
    /// <inheritdoc/>
    public Interaction<TextBoxDialogViewModel, OperationResult> RenameFileDialog { get; } = new();
    
    /// <inheritdoc/>
    public Interaction<YesNoDialogViewModel, YesNoDialogResult> DeleteFileDialog { get; } = new();

    /// <inheritdoc/>
    public Interaction<ExceptionDialogViewModel, Unit> ExceptionDialog { get; } = new();
    
    /// <inheritdoc/>
    public IObservable<IChangeSet<IDocumentFile<TData, TId>, TId>> Connect()
    {
        return _files.Connect();
    }

    /// <inheritdoc/>
    public void NewPack(IStorageFolder folder)
    {
        _folder = folder.CreateFolder(FolderName);
        _files.AddOrUpdate(NullDocumentFile<TData, TId>.Instance);
    }

    /// <inheritdoc/>
    public async Task OpenPackAsync(IStorageFolder folder)
    {
        _folder = folder.GetFolder(FolderName);

        if (_folder is null)
        {
            NewPack(folder);
            return;
        }
        
        _files.AddOrUpdate(NullDocumentFile<TData, TId>.Instance);

        foreach (var file in _folder.GetFiles())
        {
            var jsonFile = _fileFactory(file);
            await jsonFile.LoadDataFromFileAsync().ConfigureAwait(false);

            _files.AddOrUpdate(jsonFile);
        }
    }

    /// <inheritdoc/>
    public void ClosePack()
    {
        foreach (var file in _files.Items)
        {
            file.Dispose();
        }

        _files.Clear();
        _folder = null;
    }

    /// <inheritdoc/>
    public async Task<IDocumentFile<TData, TId>?> AddFileAsync()
    {
        if (_folder is null)
        {
            return null;
        }

        var textBoxDialog = new TextBoxDialogViewModel(DialogIcon.Question, NameValidationRules)
        {
            Title = $"Add {ItemName}",
            Message = "Enter Name"
        };
        var result = await AddFileDialog.Handle(textBoxDialog);

        if (result.IsCancel)
        {
            return null;
        }

        var id = Guid.NewGuid().ToLowercaseString();

        try
        {
            var file = _folder.CreateFile(id);

            var jsonFile = _fileFactory(file);
            await jsonFile.RenameAsync(textBoxDialog.InputText).ConfigureAwait(false);

            _files.AddOrUpdate(jsonFile);

            return jsonFile;
        }
        catch (Exception exception)
        {
            await ExceptionDialog.Handle(new ExceptionDialogViewModel(exception));

            return null;
        }
    }

    /// <inheritdoc/>
    public async Task RenameFileAsync(IDocumentFile<TData, TId> file)
    {
        if (_folder is null)
        {
            return;
        }

        var textBoxDialog = new TextBoxDialogViewModel(DialogIcon.Question, NameValidationRules)
        {
            Title = $"Rename {file.TitlePrefix}{file.FriendlyId}",
            Message = "Enter New Name"
        };
        var result = await RenameFileDialog.Handle(textBoxDialog);

        if (result.IsCancel)
        {
            return;
        }

        try
        {
            await file.RenameAsync(textBoxDialog.InputText).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            await ExceptionDialog.Handle(new ExceptionDialogViewModel(exception));
        }
    }

    /// <inheritdoc/>
    public async Task DeleteFileAsync(IDocumentFile<TData, TId> file)
    {
        if (_folder is null)
        {
            return;
        }

        if (!_files.Items.Contains(file))
        {
            return;
        }

        var dialog = new YesNoDialogViewModel(DialogIcon.Question)
        {
            Title = $"Delete {file.TitlePrefix}{file.FriendlyId}",
            Message = $"Are you sure you want to delete '{file.TitlePrefix}{file.FriendlyId}'?"
        };
        var result = await DeleteFileDialog.Handle(dialog);

        if (result.IsNo)
        {
            return;
        }

        file.Dispose();
        _files.Remove(file);
        _documentService.Close(file);

        try
        {
            file.Delete();
        }
        catch (Exception exception)
        {
            await ExceptionDialog.Handle(new ExceptionDialogViewModel(exception));
        }
    }

    /// <inheritdoc/>
    public IDocumentFile<TData, TId> GetFile(TId id)
    {
        return _files
            .Lookup(id)
            .ValueOr(() => NullDocumentFile<TData, TId>.Instance);
    }
}