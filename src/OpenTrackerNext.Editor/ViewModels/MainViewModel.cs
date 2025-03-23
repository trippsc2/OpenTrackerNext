using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Avalonia.Input;
using OpenTrackerNext.Core.Exceptions;
using OpenTrackerNext.Core.Results;
using OpenTrackerNext.Core.Storage;
using OpenTrackerNext.Core.Utils;
using OpenTrackerNext.Core.ViewModels;
using OpenTrackerNext.Core.ViewModels.Dialogs;
using OpenTrackerNext.Core.ViewModels.Menus;
using OpenTrackerNext.Editor.Documents;
using OpenTrackerNext.Editor.Images;
using OpenTrackerNext.Editor.Pack;
using OpenTrackerNext.Editor.Tools;
using OpenTrackerNext.Editor.ViewModels.Tools;
using OpenTrackerNext.SplatRegistration;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using IAvaloniaStorageFile = Avalonia.Platform.Storage.IStorageFile;
using IAvaloniaStorageFolder = Avalonia.Platform.Storage.IStorageFolder;
using IAvaloniaStorageItem = Avalonia.Platform.Storage.IStorageItem;

namespace OpenTrackerNext.Editor.ViewModels;

/// <summary>
/// Represents the main window view model.
/// </summary>
[Splat]
[SplatSingleInstance]
public sealed class MainViewModel : ViewModel, IDisposable
{
    /// <summary>
    /// A <see cref="string"/> representing the base window title.
    /// </summary>
    public const string BaseTitle = "OpenTrackerNext Editor";

    private readonly CompositeDisposable _disposables = new();

    private readonly IDocumentService _documentService;
    private readonly IToolService _toolService;
    private readonly IPackService _packService;
    private readonly IStorageFolder.Factory _folderFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class.
    /// </summary>
    /// <param name="documentService">
    /// The <see cref="IDocumentService"/>.
    /// </param>
    /// <param name="toolService">
    /// The <see cref="IToolService"/>.
    /// </param>
    /// <param name="packService">
    /// The <see cref="IPackService"/>.
    /// </param>
    /// <param name="imageService">
    /// The <see cref="IImageService"/>.
    /// </param>
    /// <param name="packFolderServices">
    /// The <see cref="IServiceCollection{TService}"/> of <see cref="IPackFolderService"/>.
    /// </param>
    /// <param name="folderFactory">
    /// A factory method for creating new <see cref="IStorageFolder"/> objects.
    /// </param>
    /// <param name="toolLayout">
    /// A factory method for creating new <see cref="ToolLayoutViewModel"/> objects with the specified center content.
    /// </param>
    public MainViewModel(
        IDocumentService documentService,
        IToolService toolService,
        IPackService packService,
        IImageService imageService,
        IServiceCollection<IPackFolderService> packFolderServices,
        IStorageFolder.Factory folderFactory,
        ToolLayoutViewModel toolLayout)
    {
        _toolService = toolService;
        _documentService = documentService;
        _packService = packService;
        _folderFactory = folderFactory;

        NewCommand = ReactiveCommand.CreateFromTask(NewAsync);
        OpenCommand = ReactiveCommand.CreateFromTask(OpenAsync);

        var disabledCommandSubject = new Subject<bool>();

        var disabledCommandObservable = disabledCommandSubject
            .AsObservable()
            .ObserveOn(RxApp.MainThreadScheduler);

        UndoCommand = ReactiveCommand
            .Create(() => { }, disabledCommandObservable)
            .DisposeWith(_disposables);
        RedoCommand = ReactiveCommand
            .Create(() => { }, disabledCommandObservable)
            .DisposeWith(_disposables);

        var separator = new MenuItemViewModel { Header = "-" };
        var newMenuItem = new MenuItemViewModel
        {
            Header = "New...",
            Icon = "mdi-file",
            InputGestureText = new KeyGesture(Key.N, KeyModifiers.Control),
            Command = NewCommand
        };
        var openMenuItem = new MenuItemViewModel
        {
            Header = "Open...",
            Icon = "mdi-folder-open",
            InputGestureText = new KeyGesture(Key.O, KeyModifiers.Control),
            Command = OpenCommand
        };

        var fileMenuItem = new MenuItemViewModel
        {
            Header = "File",
            Children = new ObservableCollection<MenuItemViewModel>
            {
                newMenuItem,
                separator,
                openMenuItem
            }
        };

        var undoMenuItem = new MenuItemViewModel
        {
            Header = "Undo",
            Icon = "mdi-undo",
            InputGestureText = new KeyGesture(Key.Z, KeyModifiers.Control),
            Command = UndoCommand
        };
        var redoMenuItem = new MenuItemViewModel
        {
            Header = "Redo",
            Icon = "mdi-redo",
            InputGestureText = new KeyGesture(Key.Y, KeyModifiers.Control),
            Command = RedoCommand
        };

        var editMenuItem = new MenuItemViewModel
        {
            Header = "Edit",
            Children = new ObservableCollection<MenuItemViewModel>
            {
                undoMenuItem,
                redoMenuItem
            }
        };

        TopMenuItems = [fileMenuItem, editMenuItem];

        ToolLayout = toolLayout;

        this.WhenActivated(
            disposables =>
            {
                this.WhenAnyValue(x => x._packService.LoadedPackFolder)
                    .Select(x => x is not null ? $"{BaseTitle} - {x.FullPath}" : BaseTitle)
                    .ToPropertyEx(
                        this,
                        x => x.Title,
                        scheduler: RxApp.MainThreadScheduler)
                    .DisposeWith(disposables);

                this.WhenAnyValue(x => x._packService.LoadedPackFolder)
                    .Select(x => x is not null)
                    .ToPropertyEx(
                        this,
                        x => x.ToolLayoutIsVisible,
                        scheduler: RxApp.MainThreadScheduler)
                    .DisposeWith(disposables);

                documentService.RequestSaveDialog
                    .RegisterHandler(async context => context.SetOutput(await YesNoCancelDialog.Handle(context.Input)))
                    .DisposeWith(disposables);

                documentService.ExceptionDialog
                    .RegisterHandler(async context => context.SetOutput(await ExceptionDialog.Handle(context.Input)))
                    .DisposeWith(disposables);

                imageService.AddImagesDialog
                    .RegisterHandler(async context => context.SetOutput(await AddImagesDialog.Handle(context.Input)))
                    .DisposeWith(disposables);

                imageService.RenameImageDialog
                    .RegisterHandler(async context => context.SetOutput(await TextBoxDialog.Handle(context.Input)))
                    .DisposeWith(disposables);

                imageService.DeleteImageDialog
                    .RegisterHandler(async context => context.SetOutput(await YesNoDialog.Handle(context.Input)))
                    .DisposeWith(disposables);

                imageService.ExceptionDialog
                    .RegisterHandler(async context => context.SetOutput(await ExceptionDialog.Handle(context.Input)))
                    .DisposeWith(disposables);

                foreach (var service in packFolderServices.All)
                {
                    service.AddFileDialog
                        .RegisterHandler(async context => context.SetOutput(await TextBoxDialog.Handle(context.Input)))
                        .DisposeWith(disposables);

                    service.RenameFileDialog
                        .RegisterHandler(async context => context.SetOutput(await TextBoxDialog.Handle(context.Input)))
                        .DisposeWith(disposables);
                    
                    service.DeleteFileDialog
                        .RegisterHandler(async context => context.SetOutput(await YesNoDialog.Handle(context.Input)))
                        .DisposeWith(disposables);

                    service.ExceptionDialog
                        .RegisterHandler(
                            async context => context.SetOutput(await ExceptionDialog.Handle(context.Input)))
                        .DisposeWith(disposables);
                }
            });
    }

    /// <summary>
    /// Gets an <see cref="ObservableCollection{T}"/> of <see cref="MenuItemViewModel"/> representing the top menu
    /// items.
    /// </summary>
    public ObservableCollection<MenuItemViewModel> TopMenuItems { get; }

    /// <summary>
    /// Gets a <see cref="ToolLayoutViewModel"/> representing the tool layout control view model.
    /// </summary>
    public ToolLayoutViewModel ToolLayout { get; }

    /// <summary>
    /// Gets a <see cref="string"/> representing the window title.
    /// </summary>
    [ObservableAsProperty]
    public string Title { get; } = BaseTitle;
    
    /// <summary>
    /// Gets a value indicating whether the tool layout is visible.
    /// </summary>
    [ObservableAsProperty]
    public bool ToolLayoutIsVisible { get; }

    /// <summary>
    /// Gets an <see cref="Interaction{TInput,TOutput}"/> with a <see cref="IStorageFolder"/> output representing the
    /// new pack dialog. 
    /// </summary>
    public Interaction<Unit, IAvaloniaStorageFolder?> NewPackDialog { get; } = new(RxApp.MainThreadScheduler);
    
    /// <summary>
    /// Gets an <see cref="Interaction{TInput,TOutput}"/> with a <see cref="IStorageFolder"/> output representing the
    /// open pack dialog. 
    /// </summary>
    public Interaction<Unit, IAvaloniaStorageFolder?> OpenPackDialog { get; } = new(RxApp.MainThreadScheduler);
    
    /// <summary>
    /// Gets an <see cref="Interaction{TInput,TOutput}"/> with an output of a <see cref="IReadOnlyList{T}"/> of
    /// <see cref="IAvaloniaStorageFile"/> representing the add images dialog.
    /// </summary>
    public Interaction<Unit, IReadOnlyList<IAvaloniaStorageFile>?> AddImagesDialog { get; } =
        new(RxApp.MainThreadScheduler);
    
    /// <summary>
    /// Gets an <see cref="Interaction{TInput,TOutput}"/> with an input of a <see cref="TextBoxDialogViewModel"/> and an
    /// output of an <see cref="OperationResult"/> representing opening a text box dialog.
    /// </summary>
    public Interaction<TextBoxDialogViewModel, OperationResult> TextBoxDialog { get; } =
        new(RxApp.MainThreadScheduler);
    
    /// <summary>
    /// Gets an <see cref="Interaction{TInput,TOutput}"/> with an input of a <see cref="YesNoDialogViewModel"/> and an
    /// output of a <see cref="YesNoDialogResult"/> representing opening a yes/no dialog.
    /// </summary>
    public Interaction<YesNoDialogViewModel, YesNoDialogResult> YesNoDialog { get; } = new(RxApp.MainThreadScheduler);
    
    /// <summary>
    /// Gets an <see cref="Interaction{TInput,TOutput}"/> with an input of a <see cref="YesNoCancelDialogViewModel"/>
    /// and an output of a <see cref="YesNoCancelDialogResult"/> representing opening a yes/no/cancel dialog.
    /// </summary>
    public Interaction<YesNoCancelDialogViewModel, YesNoCancelDialogResult> YesNoCancelDialog { get; } =
        new(RxApp.MainThreadScheduler);
    
    /// <summary>
    /// Gets an <see cref="Interaction{TInput,TOutput}"/> with an input of <see cref="ExceptionDialogViewModel"/>
    /// representing showing an exception dialog.
    /// </summary>
    public Interaction<ExceptionDialogViewModel, Unit> ExceptionDialog { get; } = new(RxApp.MainThreadScheduler);

    /// <summary>
    /// Gets a <see cref="ReactiveCommand{TParam,TResult}"/> to create a new pack folder.
    /// </summary>
    public ReactiveCommand<Unit, Unit> NewCommand { get; }
    
    /// <summary>
    /// Gets a <see cref="ReactiveCommand{TParam,TResult}"/> to open an existing pack folder.
    /// </summary>
    public ReactiveCommand<Unit, Unit> OpenCommand { get; }

    /// <summary>
    /// Gets a <see cref="ReactiveCommand{TParam,TResult}"/> to undo the last change.
    /// </summary>
    public ReactiveCommand<Unit, Unit> UndoCommand { get; }
    
    /// <summary>
    /// Gets a <see cref="ReactiveCommand{TParam,TResult}"/> to redo the last change.
    /// </summary>
    public ReactiveCommand<Unit, Unit> RedoCommand { get; }
    
    /// <inheritdoc />
    public void Dispose()
    {
        _disposables.Dispose();
    }
    
    private async Task NewAsync()
    {
        var closeResult = await TryCloseOpenPack().ConfigureAwait(false);

        if (closeResult.IsCancel)
        {
            return;
        }

        using var storageFolder = await NewPackDialog.Handle(Unit.Default);

        if (storageFolder is null)
        {
            return;
        }

        var items = await storageFolder
            .GetItemsAsync()
            .ToListAsync()
            .ConfigureAwait(false);

        if (items.Count != 0)
        {
            const string notEmptyTitle = "Folder Not Empty";
            const string notEmptyMessage = "The selected folder is not empty.\nPlease select an empty folder for a new pack.";
            try
            {
                throw new NewPackException(notEmptyMessage);
            }
            catch (NewPackException exception)
            {
                await HandleExceptionAsync(exception, notEmptyTitle).ConfigureAwait(false);
            }

            return;
        }

        try
        {
            var folder = await CreateFolderFromAvaloniaFolderAsync(storageFolder).ConfigureAwait(false);
            
            await _packService
                .NewPackAsync(folder)
                .ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(exception).ConfigureAwait(false);
        }
    }

    private async Task OpenAsync()
    {
        var closeResult = await TryCloseOpenPack().ConfigureAwait(false);

        if (closeResult.IsCancel)
        {
            return;
        }

        using var storageFolder = await OpenPackDialog.Handle(Unit.Default);

        if (storageFolder is null)
        {
            return;
        }

        try
        {
            var folder = await CreateFolderFromAvaloniaFolderAsync(storageFolder).ConfigureAwait(false);
            
            await _packService
                .OpenPackAsync(folder)
                .ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(exception).ConfigureAwait(false);
        }
    }

    private async Task<OperationResult> TryCloseOpenPack()
    {
        if (_packService.LoadedPackFolder is null)
        {
            return OperationResult.Success;
        }

        try
        {
            var result = await HandleOpenPackUnsavedChangesAsync()
                .ConfigureAwait(false);

            if (result.IsSuccess)
            {
                CloseOpenPack();
            }

            return result;
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(exception, "Close Failed").ConfigureAwait(false);
            return OperationResult.Cancel;
        }
    }

    private async Task<OperationResult> HandleOpenPackUnsavedChangesAsync()
    {
        return await _documentService
            .TryHandleAllUnsavedChangesAsync()
            .ConfigureAwait(false);
    }

    private void CloseOpenPack()
    {
        _toolService.DeactivateAllTools();
        _documentService.CloseAll();
        _packService.ClosePack();
    }
    
    private async Task<IStorageFolder> CreateFolderFromAvaloniaFolderAsync(IAvaloniaStorageItem folder)
    {
        var bookmarkId = await folder
            .SaveBookmarkAsync()
            .ConfigureAwait(false);

        return bookmarkId is not null
            ? _folderFactory(bookmarkId)
            : throw new StorageProviderFailureException();
    }

    private async Task HandleExceptionAsync(Exception exception, string? title = null)
    {
        var dialog = new ExceptionDialogViewModel(exception, title);

        _ = await ExceptionDialog.Handle(dialog);
    }
}