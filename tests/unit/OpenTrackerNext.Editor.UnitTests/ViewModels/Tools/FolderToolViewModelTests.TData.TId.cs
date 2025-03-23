using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using FluentAssertions;
using FluentAssertions.Reactive;
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
public abstract class FolderToolViewModelTests<TData, TId>
    where TData : class, ITitledDocumentData<TData>, new()
    where TId : struct, IGuidId<TId>
{
    protected readonly CompositeDisposable Disposables = new();
    private readonly DocumentService _documentService;
    private readonly IPackFolderService<TData, TId> _folderService;

    private readonly FolderToolViewModel<TData, TId> _subject;
    
    protected FolderToolViewModelTests(DocumentService documentService, IPackFolderService<TData, TId> folderService)
    {
        _documentService = documentService;
        _folderService = folderService;
        
        _subject = new FolderToolViewModel<TData, TId>(_documentService, _folderService, ListBoxItemFactory);

        _documentService.DisposeWith(Disposables);
        
        var packFolder = new MemoryStorageFolder(Path.GetTempPath());
        _folderService.NewPack(packFolder);

        _subject.Activator
            .Activate()
            .DisposeWith(Disposables);

        _subject.ListBoxItems
            .ToObservableChangeSet()
            .Transform(x => x.Activator.Activate())
            .DisposeMany()
            .Subscribe()
            .DisposeWith(Disposables);
    }

    protected static DocumentFile<TData, TId> DocumentFileFactory(IStorageFile file)
    {
        return new DocumentFile<TData, TId>(file);
    }

    private DocumentFileListBoxItemViewModel<TData, TId> ListBoxItemFactory(IDocumentFile<TData, TId> file)
    {
        return new DocumentFileListBoxItemViewModel<TData, TId>(_documentService, _folderService, file);
    }
    
    private async Task<IDocumentFile<TData, TId>> CreateFileAsync()
    {
        var disposable = _folderService
            .AddFileDialog
            .RegisterHandler(
                context =>
                {
                    context.Input.InputText = Guid.NewGuid().ToString();
                    context.SetOutput(OperationResult.Success);
                });

        var file = await _folderService.AddFileAsync();
        
        disposable.Dispose();
        
        return file!;
    }
    
    [Fact]
    public async Task ListBoxItems_ShouldContainExpected()
    {
        var file1 = await CreateFileAsync();
        var file2 = await CreateFileAsync();
        
        _subject.ListBoxItems
            .Should()
            .HaveCount(2)
            .And.Contain(x => x.Text == file1.FriendlyId)
            .And.Contain(x => x.Text == file2.FriendlyId);
    }

    [Fact]
    public void SelectedListBoxItem_ShouldDefaultToNull()
    {
        _subject.SelectedListBoxItem
            .Should()
            .BeNull();
    }

    [Fact]
    public async Task SelectedListBoxItem_ShouldRaisePropertyChanged()
    {
        await CreateFileAsync();

        using var monitor = _subject.Monitor();
        
        _subject.SelectedListBoxItem = _subject.ListBoxItems[0];

        monitor.Should().RaisePropertyChangeFor(x => x.SelectedListBoxItem);
    }
    
    [Fact]
    public void AddCommand_ShouldCallAddFileOnFolderService()
    {
        var addFileDialogHandled = false;
        
        var disposable = _folderService
            .AddFileDialog
            .RegisterHandler(
                context =>
                {
                    addFileDialogHandled = true;
                    context.SetOutput(OperationResult.Success);
                });

        _subject.AddCommand
            .Execute()
            .Subscribe()
            .DisposeWith(Disposables);
        
        disposable.Dispose();
        
        addFileDialogHandled.Should().BeTrue();
    }

    [Fact]
    public void OpenCommand_ShouldNotOpenDocument_WhenNothingIsSelected()
    {
        _subject.OpenCommand
            .Execute()
            .Subscribe()
            .DisposeWith(Disposables);

        _documentService.ActiveDocuments[DocumentSide.Left]
            .Value
            .Should()
            .BeNull();
    }
    
    [Fact]
    public async Task OpenCommand_ShouldOpenDocument_WhenSelected()
    {
        var file = await CreateFileAsync();
        
        _subject.SelectedListBoxItem = _subject.ListBoxItems[0];

        using var observer = _documentService.ActivationRequests[DocumentSide.Left].Observe();
        
        _subject.OpenCommand
            .Execute()
            .Subscribe()
            .DisposeWith(Disposables);
        
        await observer.Should()
            .PushMatchAsync(
                x =>
                    x is Document<TData> &&
                    ((Document<TData>)x).File == file);
    }
    
    [Fact]
    public void RenameCommand_ShouldNotRenameFile_WhenNothingIsSelected()
    {
        var renameFileDialogHandled = false;
        
        var disposable = _folderService
            .RenameFileDialog
            .RegisterHandler(
                context =>
                {
                    renameFileDialogHandled = true;
                    context.SetOutput(OperationResult.Success);
                });
        
        _subject.RenameCommand
            .Execute()
            .Subscribe()
            .DisposeWith(Disposables);
        
        disposable.Dispose();

        renameFileDialogHandled.Should().BeFalse();
    }

    [Fact]
    public async Task RenameCommand_ShouldRenameFile_WhenFileIsSelected()
    {
        var renameFileDialogHandled = false;
        
        var disposable = _folderService
            .RenameFileDialog
            .RegisterHandler(
                context =>
                {
                    renameFileDialogHandled = true;
                    context.SetOutput(OperationResult.Success);
                });
        
        await CreateFileAsync();
        
        _subject.SelectedListBoxItem = _subject.ListBoxItems[0];
        
        _subject.RenameCommand
            .Execute()
            .Subscribe()
            .DisposeWith(Disposables);
        
        disposable.Dispose();
        
        renameFileDialogHandled.Should().BeTrue();
    }
    
    [Fact]
    public void DeleteCommand_ShouldNotDeleteFile_WhenNothingIsSelected()
    {
        var deleteFileDialogHandled = false;
        
        var disposable = _folderService
            .DeleteFileDialog
            .RegisterHandler(
                context =>
                {
                    deleteFileDialogHandled = true;
                    context.SetOutput(YesNoDialogResult.Yes);
                });
        
        _subject.DeleteCommand
            .Execute()
            .Subscribe()
            .DisposeWith(Disposables);
        
        disposable.Dispose();

        deleteFileDialogHandled.Should().BeFalse();
    }
    
    [Fact]
    public async Task DeleteCommand_ShouldDeleteFile_WhenFileIsSelected()
    {
        var deleteFileDialogHandled = false;
        
        var disposable = _folderService
            .DeleteFileDialog
            .RegisterHandler(
                context =>
                {
                    deleteFileDialogHandled = true;
                    context.SetOutput(YesNoDialogResult.Yes);
                });
        
        await CreateFileAsync();
        
        _subject.SelectedListBoxItem = _subject.ListBoxItems[0];
        
        _subject.DeleteCommand
            .Execute()
            .Subscribe()
            .DisposeWith(Disposables);
        
        disposable.Dispose();
        
        deleteFileDialogHandled.Should().BeTrue();
    }

    [Fact]
    public void GetViewType_ShouldReturnExpected()
    {
        _subject.GetViewType()
            .Should()
            .Be(typeof(IViewFor<IFolderToolViewModel>));
    }
    
    [Fact]
    public void SplatResolve_ShouldResolveToSingleInstance()
    {
        var subject1 = Locator.Current.GetService<FolderToolViewModel<TData, TId>>()!;
        var subject2 = Locator.Current.GetService<FolderToolViewModel<TData, TId>>()!;
        
        subject1.Should().BeSameAs(subject2);
    }
    
    [Fact]
    public void SplatResolve_ShouldResolveToLazySingleInstance()
    {
        var lazy = Locator.Current.GetService<Lazy<FolderToolViewModel<TData, TId>>>()!;
        var subject = Locator.Current.GetService<FolderToolViewModel<TData, TId>>()!;
        
        lazy.Value
            .Should()
            .BeSameAs(subject);
    }
}