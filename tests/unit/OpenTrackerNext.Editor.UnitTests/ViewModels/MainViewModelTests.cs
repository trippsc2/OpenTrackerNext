using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using OpenTrackerNext.Core.Factories;
using OpenTrackerNext.Core.Results;
using OpenTrackerNext.Core.Storage;
using OpenTrackerNext.Core.Storage.Memory;
using OpenTrackerNext.Core.Utils;
using OpenTrackerNext.Core.ViewModels;
using OpenTrackerNext.Core.ViewModels.Dialogs;
using OpenTrackerNext.Editor.Documents;
using OpenTrackerNext.Editor.Images;
using OpenTrackerNext.Editor.Pack;
using OpenTrackerNext.Editor.Tools;
using OpenTrackerNext.Editor.ViewModels;
using OpenTrackerNext.Editor.ViewModels.Documents;
using OpenTrackerNext.Editor.ViewModels.Tools;
using ReactiveUI;
using Splat;
using Xunit;
using IAvaloniaStorageFile = Avalonia.Platform.Storage.IStorageFile;
using IAvaloniaStorageFolder = Avalonia.Platform.Storage.IStorageFolder;
using IAvaloniaStorageItem = Avalonia.Platform.Storage.IStorageItem;

namespace OpenTrackerNext.Editor.UnitTests.ViewModels;

[ExcludeFromCodeCoverage]
public sealed class MainViewModelTests
{
    private readonly CompositeDisposable _disposables = new();
    private readonly Dictionary<DocumentSide, IReactiveProperty<IDocument?>> _activeDocuments = new()
    {
        { DocumentSide.Left, new ReactiveProperty<IDocument?>() },
        { DocumentSide.Right, new ReactiveProperty<IDocument?>() }
    };
    
    private readonly SourceList<IDocument> _documents = new();
    private readonly IDocumentService _documentService = Substitute.For<IDocumentService>();
    private readonly IFactory<IDocument, ViewModel> _documentViewModelFactory =
        Substitute.For<IFactory<IDocument, ViewModel>>();

    private readonly Dictionary<ToolBarPosition, ReactiveProperty<Tool?>> _activeTools = new()
    {
        { ToolBarPosition.TopLeft, new ReactiveProperty<Tool?>() },
        { ToolBarPosition.TopRight, new ReactiveProperty<Tool?>() },
        { ToolBarPosition.BottomLeft, new ReactiveProperty<Tool?>() },
        { ToolBarPosition.BottomRight, new ReactiveProperty<Tool?>() }
    };
    
    private readonly SourceCache<Tool, ToolId> _tools = new(x => x.Id);
    private readonly IToolService _toolService = Substitute.For<IToolService>();
    private readonly IPackService _packService = Substitute.For<IPackService>();
    private readonly IImageService _imageService = Substitute.For<IImageService>();
    private readonly IPackFolderService _folderService = Substitute.For<IPackFolderService>();
    private readonly IServiceCollection<IPackFolderService> _folderServices =
        Substitute.For<IServiceCollection<IPackFolderService>>();

    private readonly ToolLayoutViewModel _toolLayout;

    private readonly MainViewModel _subject;
    
    public MainViewModelTests()
    {
        var documentsObservable = _documents.Connect();
        _documentService.Connect().Returns(documentsObservable);
        _documentService.ActiveDocuments.Returns(_activeDocuments);
        
        _documentService.RequestSaveDialog
            .Returns(new Interaction<YesNoCancelDialogViewModel, YesNoCancelDialogResult>());
        _documentService.ExceptionDialog
            .Returns(new Interaction<ExceptionDialogViewModel, Unit>());
        
        var toolsObservable = _tools.Connect();
        _toolService.Connect().Returns(toolsObservable);
        _toolService.ActiveTools.Returns(_activeTools);
        
        var centerContent = new DocumentLayoutViewModel(_documentService, DocumentDockFactory);
        _toolLayout = new ToolLayoutViewModel(_toolService, centerContent, ToolBarFactory, ToolPaneFactory);
        
        _imageService.AddImagesDialog.Returns(new Interaction<Unit, IReadOnlyList<IAvaloniaStorageFile>?>());
        _imageService.RenameImageDialog.Returns(new Interaction<TextBoxDialogViewModel, OperationResult>());
        _imageService.DeleteImageDialog.Returns(new Interaction<YesNoDialogViewModel, YesNoDialogResult>());
        _imageService.ExceptionDialog.Returns(new Interaction<ExceptionDialogViewModel, Unit>());

        _folderService.AddFileDialog.Returns(new Interaction<TextBoxDialogViewModel, OperationResult>());
        _folderService.RenameFileDialog.Returns(new Interaction<TextBoxDialogViewModel, OperationResult>());
        _folderService.DeleteFileDialog.Returns(new Interaction<YesNoDialogViewModel, YesNoDialogResult>());
        _folderService.ExceptionDialog.Returns(new Interaction<ExceptionDialogViewModel, Unit>());

        _folderServices.All.Returns([_folderService]);

        _subject = new MainViewModel(
            _documentService,
            _toolService,
            _packService,
            _imageService,
            _folderServices,
            FolderFactory,
            _toolLayout);
        
        _subject.DisposeWith(_disposables);
        
        _subject.Activator
            .Activate()
            .DisposeWith(_disposables);
    }

    private static MemoryStorageFolder FolderFactory(string path)
    {
        return new MemoryStorageFolder(path);
    }

    private ToolBarViewModel ToolBarFactory(ToolBarPosition position)
    {
        return new ToolBarViewModel(_toolService, ToolButtonFactory, position);
    }

    private ToolButtonViewModel ToolButtonFactory(Tool tool)
    {
        return new ToolButtonViewModel(_toolService, tool);
    }

    private ToolPaneViewModel ToolPaneFactory(ToolPaneSide side)
    {
        return new ToolPaneViewModel(_toolService, ToolFactory, side);
    }

    private ToolViewModel ToolFactory(Tool tool)
    {
        return new ToolViewModel(_toolService, tool);
    }
    
    private DocumentDockViewModel DocumentDockFactory(DocumentSide side)
    {
        return new DocumentDockViewModel(_documentService, DocumentViewModelFactory, side);
    }

    private DocumentViewModel DocumentViewModelFactory(IDocument document)
    {
        return new DocumentViewModel(_documentService, DocumentBodyFactory, document);
    }

    private DocumentBodyViewModel DocumentBodyFactory(IDocument document)
    {
        return new DocumentBodyViewModel(_documentViewModelFactory, document);
    }
    
    [Fact]
    public void TopMenuItems_ShouldContainExpected()
    {
        _subject.TopMenuItems
            .Should()
            .HaveCount(2);

        var fileMenu = _subject.TopMenuItems
            .Should()
            .Contain(x => x.Header == "File")
            .Subject;
        
        var editMenu = _subject.TopMenuItems
            .Should()
            .Contain(x => x.Header == "Edit")
            .Subject;
        
        var newMenu = fileMenu.Children
            .Should()
            .Contain(x => x.Header == "New...")
            .Subject;
        
        var openMenu = fileMenu.Children
            .Should()
            .Contain(x => x.Header == "Open...")
            .Subject;

        var undoMenu = editMenu.Children
            .Should()
            .Contain(x => x.Header == "Undo")
            .Subject;
        
        var redoMenu = editMenu.Children
            .Should()
            .Contain(x => x.Header == "Redo")
            .Subject;
        
        newMenu.Command
            .Should()
            .Be(_subject.NewCommand);
        
        openMenu.Command
            .Should()
            .Be(_subject.OpenCommand);
        
        undoMenu.Command
            .Should()
            .Be(_subject.UndoCommand);
        
        redoMenu.Command
            .Should()
            .Be(_subject.RedoCommand);
    }

    [Fact]
    public void ToolLayout_ShouldReturnExpected()
    {
        _subject.ToolLayout
            .Should()
            .Be(_toolLayout);
    }

    [Fact]
    public void Title_ShouldReturnBaseTitle_WhenPackIsNotLoaded()
    {
        _packService.LoadedPackFolder.ReturnsNull();
        
        _packService.PropertyChanged += Raise
            .Event<PropertyChangedEventHandler>(
                _packService,
                new PropertyChangedEventArgs(nameof(IPackService.LoadedPackFolder)));
        
        _subject.Title
            .Should()
            .Be(MainViewModel.BaseTitle);
    }
    
    [Fact]
    public void Title_ShouldReturnTitleWithFolderPath_WhenPackIsLoaded()
    {
        const string folderPath = "test";

        var packFolder = new MemoryStorageFolder(folderPath);
        
        _packService.LoadedPackFolder.Returns(packFolder);
        
        _packService.PropertyChanged += Raise
            .Event<PropertyChangedEventHandler>(
                _packService,
                new PropertyChangedEventArgs(nameof(IPackService.LoadedPackFolder)));
        
        _subject.Title
            .Should()
            .Be($"{MainViewModel.BaseTitle} - {folderPath}");
    }

    [Fact]
    public void ToolLayoutIsVisible_ShouldReturnFalse_WhenPackIsNotLoaded()
    {
        _packService.LoadedPackFolder.ReturnsNull();
        
        _packService.PropertyChanged += Raise
            .Event<PropertyChangedEventHandler>(
                _packService,
                new PropertyChangedEventArgs(nameof(IPackService.LoadedPackFolder)));

        _subject.ToolLayoutIsVisible
            .Should()
            .BeFalse();
    }

    [Fact]
    public void ToolLayoutIsVisible_ShouldReturnTrue_WhenPackIsLoaded()
    {
        const string folderPath = "test";

        var packFolder = new MemoryStorageFolder(folderPath);
        
        _packService.LoadedPackFolder.Returns(packFolder);
        
        _packService.PropertyChanged += Raise
            .Event<PropertyChangedEventHandler>(
                _packService,
                new PropertyChangedEventArgs(nameof(IPackService.LoadedPackFolder)));

        _subject.ToolLayoutIsVisible
            .Should()
            .BeTrue();
    }

    [Fact]
    public async Task AddImagesDialog_ShouldBeOpenedByImageService()
    {
        var dialogOpened = false;

        _subject.AddImagesDialog
            .RegisterHandler(context =>
            {
                dialogOpened = true;
                context.SetOutput(Array.Empty<IAvaloniaStorageFile>());
            })
            .DisposeWith(_disposables);

        await _imageService.AddImagesDialog.Handle(Unit.Default);
        
        dialogOpened.Should().BeTrue();
    }

    [Fact]
    public async Task TextBoxDialog_ShouldBeOpenedByImageService()
    {
        var dialogOpened = false;
        
        _subject.TextBoxDialog
            .RegisterHandler(context =>
            {
                dialogOpened = true;
                context.SetOutput(OperationResult.Success);
            })
            .DisposeWith(_disposables);

        var textBox = new TextBoxDialogViewModel
        {
            Title = string.Empty,
            Message = string.Empty
        };
        
        await _imageService.RenameImageDialog.Handle(textBox);
        
        dialogOpened.Should().BeTrue();
    }
    
    [Fact]
    public async Task TextBoxDialog_ShouldBeOpenedByFolderServiceAddFileDialog()
    {
        var dialogOpened = false;
        
        _subject.TextBoxDialog
            .RegisterHandler(context =>
            {
                dialogOpened = true;
                context.SetOutput(OperationResult.Success);
            })
            .DisposeWith(_disposables);

        var textBox = new TextBoxDialogViewModel
        {
            Title = string.Empty,
            Message = string.Empty
        };
        
        await _folderService.AddFileDialog.Handle(textBox);
        
        dialogOpened.Should().BeTrue();
    }
    
    [Fact]
    public async Task TextBoxDialog_ShouldBeOpenedByFolderServiceRenameFileDialog()
    {
        var dialogOpened = false;
        
        _subject.TextBoxDialog
            .RegisterHandler(context =>
            {
                dialogOpened = true;
                context.SetOutput(OperationResult.Success);
            })
            .DisposeWith(_disposables);

        var textBox = new TextBoxDialogViewModel
        {
            Title = string.Empty,
            Message = string.Empty
        };
        
        await _folderService.RenameFileDialog.Handle(textBox);
        
        dialogOpened.Should().BeTrue();
    }
    
    [Fact]
    public async Task YesNoDialog_ShouldBeOpenedByImageService()
    {
        var dialogOpened = false;
        
        _subject.YesNoDialog
            .RegisterHandler(context =>
            {
                dialogOpened = true;
                context.SetOutput(YesNoDialogResult.No);
            })
            .DisposeWith(_disposables);

        var yesNo = new YesNoDialogViewModel(null)
        {
            Title = string.Empty,
            Message = string.Empty
        };
        
        await _imageService.DeleteImageDialog.Handle(yesNo);
        
        dialogOpened.Should().BeTrue();
    }
    
    [Fact]
    public async Task YesNoDialog_ShouldBeOpenedByFolderService()
    {
        var dialogOpened = false;
        
        _subject.YesNoDialog
            .RegisterHandler(context =>
            {
                dialogOpened = true;
                context.SetOutput(YesNoDialogResult.No);
            })
            .DisposeWith(_disposables);

        var yesNo = new YesNoDialogViewModel(null)
        {
            Title = string.Empty,
            Message = string.Empty
        };
        
        await _folderService.DeleteFileDialog.Handle(yesNo);
        
        dialogOpened.Should().BeTrue();
    }
    
    [Fact]
    public async Task YesNoCancelDialog_ShouldBeOpenedByDocumentService()
    {
        var dialogOpened = false;
        
        _subject.YesNoCancelDialog
            .RegisterHandler(context =>
            {
                dialogOpened = true;
                context.SetOutput(YesNoCancelDialogResult.Yes);
            })
            .DisposeWith(_disposables);

        var yesNoCancel = new YesNoCancelDialogViewModel(null)
        {
            Title = string.Empty,
            Message = string.Empty
        };
        
        await _documentService.RequestSaveDialog.Handle(yesNoCancel);
        
        dialogOpened.Should().BeTrue();
    }
    
    [Fact]
    public async Task ExceptionDialog_ShouldBeOpenedByDocumentService()
    {
        var dialogOpened = false;
        
        _subject.ExceptionDialog
            .RegisterHandler(context =>
            {
                dialogOpened = true;
                context.SetOutput(Unit.Default);
            })
            .DisposeWith(_disposables);

        var exception = new ExceptionDialogViewModel(new Exception());
        
        await _documentService.ExceptionDialog.Handle(exception);
        
        dialogOpened.Should().BeTrue();
    }
    
    [Fact]
    public async Task ExceptionDialog_ShouldBeOpenedByImageService()
    {
        var dialogOpened = false;
        
        _subject.ExceptionDialog
            .RegisterHandler(context =>
            {
                dialogOpened = true;
                context.SetOutput(Unit.Default);
            })
            .DisposeWith(_disposables);

        var exception = new ExceptionDialogViewModel(new Exception());
        
        await _imageService.ExceptionDialog.Handle(exception);
        
        dialogOpened.Should().BeTrue();
    }
    
    [Fact]
    public async Task ExceptionDialog_ShouldBeOpenedByFolderService()
    {
        var dialogOpened = false;
        
        _subject.ExceptionDialog
            .RegisterHandler(context =>
            {
                dialogOpened = true;
                context.SetOutput(Unit.Default);
            })
            .DisposeWith(_disposables);

        var exception = new ExceptionDialogViewModel(new Exception());
        
        await _folderService.ExceptionDialog.Handle(exception);
        
        dialogOpened.Should().BeTrue();
    }
    
    [Fact]
    public void NewCommand_ShouldCallNewPackAsyncOnPackService_WhenNoExistingPack()
    {
        const string name = "test";
        
        var bookmark = Path.Combine(Path.GetTempPath(), name);
        
        var storageFolder = Substitute.For<IAvaloniaStorageFolder>();
        
        storageFolder
            .GetItemsAsync()
            .Returns(Array.Empty<IAvaloniaStorageItem>().ToAsyncEnumerable());
        
        storageFolder.SaveBookmarkAsync().Returns(bookmark);
        
        _subject.NewPackDialog
            .RegisterHandler(context => context.SetOutput(storageFolder))
            .DisposeWith(_disposables);

        _packService.LoadedPackFolder.ReturnsNull();
        
        _subject.NewCommand
            .Execute()
            .Subscribe()
            .DisposeWith(_disposables);

        _packService
            .Received(1)
            .NewPackAsync(Arg.Is<IStorageFolder>(folder => folder.Name == name));
    }

    [Fact]
    public void NewCommand_ShouldOpenExceptionDialog_WhenNoExistingPack_WhenFolderIsNotEmpty()
    {
        var storageFolder = Substitute.For<IAvaloniaStorageFolder>();
        var items = new List<IAvaloniaStorageItem> { Substitute.For<IAvaloniaStorageItem>() };
        
        storageFolder.GetItemsAsync().Returns(items.ToAsyncEnumerable());
        
        _subject.NewPackDialog
            .RegisterHandler(context => context.SetOutput(storageFolder))
            .DisposeWith(_disposables);

        _packService.LoadedPackFolder.ReturnsNull();

        var exceptionDialogShown = false;
        
        _subject.ExceptionDialog
            .RegisterHandler(context =>
            {
                exceptionDialogShown = true;
                context.SetOutput(Unit.Default);
            })
            .DisposeWith(_disposables);
        
        _subject.NewCommand
            .Execute()
            .Subscribe()
            .DisposeWith(_disposables);

        exceptionDialogShown.Should().BeTrue();
        _packService
            .DidNotReceive()
            .NewPackAsync(Arg.Is<IStorageFolder>(folder => folder.Name == storageFolder.Name));
    }

    [Fact]
    public void NewCommand_ShouldOpenExceptionDialog_WhenNoExistingPack_WhenCreatingPackThrows()
    {
        const string name = "test";
        
        var bookmark = Path.Combine(Path.GetTempPath(), name);
        
        var storageFolder = Substitute.For<IAvaloniaStorageFolder>();
        
        storageFolder
            .GetItemsAsync()
            .Returns(Array.Empty<IAvaloniaStorageItem>().ToAsyncEnumerable());
        
        storageFolder
            .SaveBookmarkAsync()
            .Returns(bookmark);
        
        _subject.NewPackDialog
            .RegisterHandler(context => context.SetOutput(storageFolder))
            .DisposeWith(_disposables);

        _packService.LoadedPackFolder.ReturnsNull();

        _packService
            .When(
                x =>
                    x.NewPackAsync(Arg.Is<IStorageFolder>(folder => folder.Name == name)))
            .Throw<Exception>();
        
        var exceptionDialogShown = false;
        
        _subject.ExceptionDialog
            .RegisterHandler(context =>
            {
                exceptionDialogShown = true;
                context.SetOutput(Unit.Default);
            })
            .DisposeWith(_disposables);
        
        _subject.NewCommand
            .Execute()
            .Subscribe()
            .DisposeWith(_disposables);

        exceptionDialogShown.Should().BeTrue();
        
        _packService
            .Received(1)
            .NewPackAsync(Arg.Is<IStorageFolder>(folder => folder.Name == name));
    }
    
    [Fact]
    public void NewCommand_ShouldNotCallNewPackAsyncOnPackService_WhenNoExistingPack_WhenDialogIsCancelled()
    {
        _subject.NewPackDialog
            .RegisterHandler(context => context.SetOutput(null))
            .DisposeWith(_disposables);

        _packService.LoadedPackFolder.ReturnsNull();
        
        _subject.NewCommand
            .Execute()
            .Subscribe()
            .DisposeWith(_disposables);

        _packService
            .DidNotReceive()
            .NewPackAsync(Arg.Any<IStorageFolder>());
    }
    
    [Fact]
    public void NewCommand_ShouldCallNewPackAsyncOnPackService_WhenExistingPackHandled()
    {
        const string name = "test";
        
        var bookmark = Path.Combine(Path.GetTempPath(), name);
        
        var storageFolder = Substitute.For<IAvaloniaStorageFolder>();
        
        storageFolder
            .GetItemsAsync()
            .Returns(Array.Empty<IAvaloniaStorageItem>().ToAsyncEnumerable());
        
        storageFolder
            .SaveBookmarkAsync()
            .Returns(bookmark);
        
        _subject.NewPackDialog
            .RegisterHandler(context => context.SetOutput(storageFolder))
            .DisposeWith(_disposables);

        _documentService
            .TryHandleAllUnsavedChangesAsync()
            .Returns(OperationResult.Success);

        _subject.NewCommand
            .Execute()
            .Subscribe()
            .DisposeWith(_disposables);

        _packService
            .Received(1)
            .NewPackAsync(Arg.Is<IStorageFolder>(folder => folder.Name == name));
    }
        
    [Fact]
    public void NewCommand_ShouldOpenExceptionDialog_WhenExistingPackHandled_WhenCreatingPackThrows()
    {
        const string name = "test";
        
        var bookmark = Path.Combine(Path.GetTempPath(), name);
        
        var storageFolder = Substitute.For<IAvaloniaStorageFolder>();
        
        storageFolder
            .GetItemsAsync()
            .Returns(Array.Empty<IAvaloniaStorageItem>().ToAsyncEnumerable());
        
        storageFolder
            .SaveBookmarkAsync()
            .Returns(bookmark);
        
        _subject.NewPackDialog
            .RegisterHandler(context => context.SetOutput(storageFolder))
            .DisposeWith(_disposables);

        _documentService
            .TryHandleAllUnsavedChangesAsync()
            .Returns(OperationResult.Success);

        _packService
            .When(x =>
                x.NewPackAsync(Arg.Is<IStorageFolder>(folder => folder.Name == name)))
            .Throw<Exception>();
        
        var exceptionDialogShown = false;
        
        _subject.ExceptionDialog
            .RegisterHandler(context =>
            {
                exceptionDialogShown = true;
                context.SetOutput(Unit.Default);
            })
            .DisposeWith(_disposables);
        
        _subject.NewCommand
            .Execute()
            .Subscribe()
            .DisposeWith(_disposables);

        exceptionDialogShown.Should().BeTrue();
        
        _packService
            .Received(1)
            .NewPackAsync(Arg.Is<IStorageFolder>(folder => folder.Name == name));
    }

    [Fact]
    public void NewCommand_ShouldOpenExceptionDialog_WhenExistingPackHandled_WhenFolderIsNotEmpty()
    {
        var storageFolder = Substitute.For<IAvaloniaStorageFolder>();
        var items = new List<IAvaloniaStorageItem> { Substitute.For<IAvaloniaStorageItem>() };
        
        storageFolder.GetItemsAsync().Returns(items.ToAsyncEnumerable());
        
        _subject.NewPackDialog
            .RegisterHandler(context => context.SetOutput(storageFolder))
            .DisposeWith(_disposables);

        _documentService.TryHandleAllUnsavedChangesAsync().Returns(OperationResult.Success);

        var exceptionDialogShown = false;
        
        _subject.ExceptionDialog
            .RegisterHandler(context =>
            {
                exceptionDialogShown = true;
                context.SetOutput(Unit.Default);
            })
            .DisposeWith(_disposables);
        
        _subject.NewCommand
            .Execute()
            .Subscribe()
            .DisposeWith(_disposables);

        exceptionDialogShown.Should().BeTrue();
        
        _packService
            .DidNotReceive()
            .NewPackAsync(Arg.Is<IStorageFolder>(folder => folder.Name == storageFolder.Name));
    }
    
    [Fact]
    public void NewCommand_ShouldNotCallNewPackAsyncOnPackService_WhenExistingPackHandled_WhenDialogIsCancelled()
    {
        _subject.NewPackDialog
            .RegisterHandler(context => context.SetOutput(null))
            .DisposeWith(_disposables);

        _documentService
            .TryHandleAllUnsavedChangesAsync()
            .Returns(OperationResult.Success);
        
        _subject.NewCommand
            .Execute()
            .Subscribe()
            .DisposeWith(_disposables);

        _packService
            .DidNotReceive()
            .NewPackAsync(Arg.Any<IStorageFolder>());
    }
    
    [Fact]
    public void NewCommand_ShouldNotCallNewPackAsyncOnPackService_WhenExistingPackHandlingIsCancelled()
    {
        var storageFolder = Substitute.For<IAvaloniaStorageFolder>();
        
        storageFolder
            .GetItemsAsync()
            .Returns(Array.Empty<IAvaloniaStorageItem>().ToAsyncEnumerable());

        _documentService
            .TryHandleAllUnsavedChangesAsync()
            .Returns(OperationResult.Cancel);
        
        _subject.NewCommand
            .Execute()
            .Subscribe()
            .DisposeWith(_disposables);

        _packService
            .DidNotReceive()
            .NewPackAsync(Arg.Any<IStorageFolder>());
    }
    
    [Fact]
    public void NewCommand_ShouldNotCallNewPackAsyncOnPackService_WhenExistingPackHandlingThrowsException()
    {
        var storageFolder = Substitute.For<IAvaloniaStorageFolder>();
        
        storageFolder
            .GetItemsAsync()
            .Returns(Array.Empty<IAvaloniaStorageItem>().ToAsyncEnumerable());

        _documentService
            .When(x => x.TryHandleAllUnsavedChangesAsync())
            .Throw<Exception>();
        
        var exceptionDialogShown = false;
        
        _subject.ExceptionDialog
            .RegisterHandler(context =>
            {
                exceptionDialogShown = true;
                context.SetOutput(Unit.Default);
            })
            .DisposeWith(_disposables);
        
        _subject.NewCommand
            .Execute()
            .Subscribe()
            .DisposeWith(_disposables);

        exceptionDialogShown.Should().BeTrue();
        
        _packService
            .DidNotReceive()
            .NewPackAsync(Arg.Any<IStorageFolder>());
    }
    
    [Fact]
    public void OpenCommand_ShouldCallOpenPackAsyncOnPackService_WhenNoExistingPack()
    {
        const string name = "test";
        
        var bookmark = Path.Combine(Path.GetTempPath(), name);
        
        var storageFolder = Substitute.For<IAvaloniaStorageFolder>();
        
        storageFolder
            .SaveBookmarkAsync()
            .Returns(bookmark);
        
        _subject.OpenPackDialog
            .RegisterHandler(context => context.SetOutput(storageFolder))
            .DisposeWith(_disposables);

        _packService.LoadedPackFolder.ReturnsNull();
        
        _subject.OpenCommand
            .Execute()
            .Subscribe()
            .DisposeWith(_disposables);

        _packService
            .Received(1)
            .OpenPackAsync(Arg.Is<IStorageFolder>(folder => folder.Name == name));
    }

    [Fact]
    public void OpenCommand_ShouldOpenExceptionDialog_WhenNoExistingPack_WhenLoadingPackThrows()
    {
        const string name = "test";
        
        var bookmark = Path.Combine(Path.GetTempPath(), name);
        
        var storageFolder = Substitute.For<IAvaloniaStorageFolder>();
        
        storageFolder
            .SaveBookmarkAsync()
            .Returns(bookmark);
        
        _subject.OpenPackDialog
            .RegisterHandler(context => context.SetOutput(storageFolder))
            .DisposeWith(_disposables);

        _packService.LoadedPackFolder.ReturnsNull();

        _packService
            .When(x =>
                x.OpenPackAsync(Arg.Is<IStorageFolder>(folder => folder.Name == name)))
            .Throw<Exception>();
        
        var exceptionDialogShown = false;
        
        _subject.ExceptionDialog
            .RegisterHandler(context =>
            {
                exceptionDialogShown = true;
                context.SetOutput(Unit.Default);
            })
            .DisposeWith(_disposables);
        
        _subject.OpenCommand
            .Execute()
            .Subscribe()
            .DisposeWith(_disposables);

        exceptionDialogShown.Should().BeTrue();
        
        _packService
            .Received(1)
            .OpenPackAsync(Arg.Is<IStorageFolder>(folder => folder.Name == name));
    }
    
    [Fact]
    public void OpenCommand_ShouldNotCallOpenPackAsyncOnPackService_WhenNoExistingPack_WhenOpenDialogIsCancelled()
    {
        _subject.OpenPackDialog
            .RegisterHandler(context => context.SetOutput(null))
            .DisposeWith(_disposables);

        _packService.LoadedPackFolder.ReturnsNull();
        
        _subject.OpenCommand
            .Execute()
            .Subscribe()
            .DisposeWith(_disposables);

        _packService
            .DidNotReceive()
            .OpenPackAsync(Arg.Any<IStorageFolder>());
    }
    
    [Fact]
    public void OpenCommand_ShouldCallOpenPackAsyncOnPackService_WhenExistingPackHandled()
    {
        const string name = "test";
        
        var bookmark = Path.Combine(Path.GetTempPath(), name);
        
        var storageFolder = Substitute.For<IAvaloniaStorageFolder>();
        
        storageFolder
            .SaveBookmarkAsync()
            .Returns(bookmark);
        
        _subject.OpenPackDialog
            .RegisterHandler(context => context.SetOutput(storageFolder))
            .DisposeWith(_disposables);

        _documentService
            .TryHandleAllUnsavedChangesAsync()
            .Returns(OperationResult.Success);

        _subject.OpenCommand
            .Execute()
            .Subscribe()
            .DisposeWith(_disposables);

        _packService
            .Received(1)
            .OpenPackAsync(Arg.Is<IStorageFolder>(folder => folder.Name == name));
    }
        
    [Fact]
    public void OpenCommand_ShouldOpenExceptionDialog_WhenExistingPackHandled_WhenLoadingPackThrowsException()
    {
        const string name = "test";
        
        var bookmark = Path.Combine(Path.GetTempPath(), name);
        
        var storageFolder = Substitute.For<IAvaloniaStorageFolder>();
        
        storageFolder
            .SaveBookmarkAsync()
            .Returns(bookmark);
        
        _subject.OpenPackDialog
            .RegisterHandler(context => context.SetOutput(storageFolder))
            .DisposeWith(_disposables);

        _documentService.TryHandleAllUnsavedChangesAsync().Returns(OperationResult.Success);

        _packService
            .When(x =>
                x.OpenPackAsync(Arg.Is<IStorageFolder>(folder => folder.Name == name)))
            .Throw<Exception>();
        
        var exceptionDialogShown = false;
        
        _subject.ExceptionDialog
            .RegisterHandler(context =>
            {
                exceptionDialogShown = true;
                context.SetOutput(Unit.Default);
            })
            .DisposeWith(_disposables);
        
        _subject.OpenCommand
            .Execute()
            .Subscribe()
            .DisposeWith(_disposables);

        exceptionDialogShown.Should().BeTrue();
        
        _packService
            .Received(1)
            .OpenPackAsync(Arg.Is<IStorageFolder>(folder => folder.Name == name));
    }
    
    [Fact]
    public void OpenCommand_ShouldNotCallNewPackAsyncOnPackService_WhenExistingPackHandled_WhenDialogIsCancelled()
    {
        _subject.OpenPackDialog
            .RegisterHandler(context => context.SetOutput(null))
            .DisposeWith(_disposables);

        _documentService
            .TryHandleAllUnsavedChangesAsync()
            .Returns(OperationResult.Success);
        
        _subject.OpenCommand
            .Execute()
            .Subscribe()
            .DisposeWith(_disposables);

        _packService
            .DidNotReceive()
            .NewPackAsync(Arg.Any<IStorageFolder>());
    }
    
    [Fact]
    public void OpenCommand_ShouldNotCallOpenPackAsyncOnPackService_WhenExistingPackHandlingIsCancelled()
    {
        var storageFolder = Substitute.For<IAvaloniaStorageFolder>();
        
        storageFolder
            .GetItemsAsync()
            .Returns(Array.Empty<IAvaloniaStorageItem>().ToAsyncEnumerable());

        _documentService
            .TryHandleAllUnsavedChangesAsync()
            .Returns(OperationResult.Cancel);
        
        _subject.OpenCommand
            .Execute()
            .Subscribe()
            .DisposeWith(_disposables);

        _packService
            .DidNotReceive()
            .OpenPackAsync(Arg.Any<IStorageFolder>());
    }
    
    [Fact]
    public void OpenCommand_ShouldNotCallOpenPackAsyncOnPackService_WhenExistingPackHandlingThrowsException()
    {
        var storageFolder = Substitute.For<IAvaloniaStorageFolder>();
        
        storageFolder
            .GetItemsAsync()
            .Returns(Array.Empty<IAvaloniaStorageItem>().ToAsyncEnumerable());

        _documentService
            .When(x => x.TryHandleAllUnsavedChangesAsync())
            .Throw<Exception>();
        
        var exceptionDialogShown = false;
        
        _subject.ExceptionDialog
            .RegisterHandler(context =>
            {
                exceptionDialogShown = true;
                context.SetOutput(Unit.Default);
            })
            .DisposeWith(_disposables);
        
        _subject.OpenCommand
            .Execute()
            .Subscribe()
            .DisposeWith(_disposables);

        exceptionDialogShown.Should().BeTrue();
        
        _packService
            .DidNotReceive()
            .OpenPackAsync(Arg.Any<IStorageFolder>());
    }

    [Fact]
    public void SplatResolve_ShouldResolveToSingleInstance()
    {
        var subject1 = Locator.Current.GetService<MainViewModel>()!;
        var subject2 = Locator.Current.GetService<MainViewModel>()!;
        
        subject1.Should().BeSameAs(subject2);
    }
    
    [Fact]
    public void SplatResolve_ShouldResolveToLazySingleInstance()
    {
        var lazy = Locator.Current.GetService<Lazy<MainViewModel>>()!;
        var subject = Locator.Current.GetService<MainViewModel>()!;
        
        lazy.Value
            .Should()
            .BeSameAs(subject);
    }
}