using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Reactive;
using NSubstitute;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Ids;
using OpenTrackerNext.Core.Results;
using OpenTrackerNext.Core.Storage;
using OpenTrackerNext.Core.Storage.Memory;
using OpenTrackerNext.Editor.Documents;
using OpenTrackerNext.Editor.Pack;
using OpenTrackerNext.Editor.ViewModels.Tools;
using ReactiveUI;
using Splat;
using Xunit;

namespace OpenTrackerNext.Editor.UnitTests.ViewModels.Tools;

[ExcludeFromCodeCoverage]
public abstract class DocumentFileListBoxItemViewModelTests<TData, TId> : IAsyncLifetime
    where TData : class, ITitledDocumentData<TData>, new()
    where TId : struct, IGuidId<TId>
{
    protected readonly CompositeDisposable Disposables = new();
    private readonly DocumentService _documentService;
    private readonly IPackFolderService<TData, TId> _folderService;
    private readonly MemoryStorageFolder _packFolder = new(Path.Combine(Path.GetTempPath()));
    private IDocumentFile<TData, TId> _jsonFile = null!;
    
    private DocumentFileListBoxItemViewModel<TData, TId> _subject = null!;

    protected DocumentFileListBoxItemViewModelTests(
        DocumentService documentService,
        IPackFolderService<TData, TId> folderService)
    {
        _documentService = documentService;
        _folderService = folderService;
        
        _documentService.DisposeWith(Disposables);
    }

    public async ValueTask InitializeAsync()
    {
        _folderService.NewPack(_packFolder);
        
        var disposable = _folderService
            .AddFileDialog
            .RegisterHandler(
                context =>
                {
                    context.Input.InputText = Guid.NewGuid().ToString();
                    context.SetOutput(OperationResult.Success);
                });

        _jsonFile = (await _folderService.AddFileAsync())!;
        
        disposable.Dispose();
        
        _subject = new DocumentFileListBoxItemViewModel<TData, TId>(_documentService, _folderService, _jsonFile!);

        _subject.Activator
            .Activate()
            .DisposeWith(Disposables);
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        Disposables.Dispose();
        return ValueTask.CompletedTask;
    }

    protected static DocumentFile<TData, TId> DocumentFileFactory(IStorageFile file)
    {
        return new DocumentFile<TData, TId>(file);
    }
    
    [Fact]
    public void ContextMenuItems_ShouldReturnExpected()
    {
        _subject.ContextMenuItems
            .Should()
            .HaveCount(4)
            .And.Contain(
                x =>
                    x.Icon == "mdi-folder-open" &&
                    x.Header == "Open" &&
                    x.Command == _subject.DoubleTapCommand)
            .And.Contain(
                x =>
                    x.Icon == "mdi-rename" &&
                    x.Header == "Rename..." &&
                    x.Command == _subject.RenameCommand)
            .And.Contain(x => x.Header == "-")
            .And.Contain(
                x =>
                    x.Icon == "mdi-delete" &&
                    x.Header == "Delete..." &&
                    x.Command == _subject.DeleteCommand);
    }

    [Theory]
    [InlineData("Test")]
    [InlineData("Test2")]
    public async Task Text_ShouldReturnExpected(string expected)
    {
        await _jsonFile.RenameAsync(expected);
        
        _subject.Text
            .Should()
            .Be(expected);
    }

    [Fact]
    public void DoubleTabCommand_ShouldOpenDocument()
    {
        using var observer = _documentService.ActivationRequests[DocumentSide.Left].Observe();
        
        _subject.DoubleTapCommand
            .Execute()
            .Subscribe()
            .DisposeWith(Disposables);

        observer.Should()
            .PushMatch(
                x =>
                    x is Document<TData> &&
                    ((Document<TData>)x).File == _jsonFile);
    }
    
    [Fact]
    public void RenameCommand_ShouldCallRename()
    {
        var renameInteractionHandled = false;
        _folderService
            .RenameFileDialog
            .RegisterHandler(
                context =>
                {
                    renameInteractionHandled = true;
                    context.SetOutput(OperationResult.Success);
                })
            .DisposeWith(Disposables);

        _subject.RenameCommand
            .Execute()
            .Subscribe()
            .DisposeWith(Disposables);

        renameInteractionHandled.Should().BeTrue();
    }

    [Fact]
    public void DeleteCommand_ShouldCallDelete()
    {
        var deleteInteractionHandled = false;
        _folderService
            .DeleteFileDialog
            .RegisterHandler(
                context =>
                {
                    deleteInteractionHandled = true;
                    context.SetOutput(YesNoDialogResult.No);
                })
            .DisposeWith(Disposables);

        _subject.DeleteCommand
            .Execute()
            .Subscribe()
            .DisposeWith(Disposables);
        
        deleteInteractionHandled.Should().BeTrue();
    }

    [Fact]
    public void GetViewType_ShouldReturnExpected()
    {
        _subject.GetViewType()
            .Should()
            .Be(typeof(IViewFor<IListBoxItemViewModel>));
    }
    
    [Fact]
    public void SplatResolve_ShouldResolveToFactory()
    {
        var factory = Locator.Current.GetService<DocumentFileListBoxItemViewModel<TData, TId>.Factory>()!;
        
        var file = Substitute.For<IDocumentFile<TData, TId>>();
        
        var subject1 = factory(file);
        var subject2 = factory(file);

        subject1.Should().NotBeSameAs(subject2);
    }
}