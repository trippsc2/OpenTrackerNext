using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Ids;
using OpenTrackerNext.Core.Results;
using OpenTrackerNext.Core.Storage;
using OpenTrackerNext.Core.Storage.Memory;
using OpenTrackerNext.Editor.Documents;
using OpenTrackerNext.Editor.Pack;
using Splat;
using Xunit;

namespace OpenTrackerNext.Editor.UnitTests.Pack;

[ExcludeFromCodeCoverage]
public abstract class PackFolderServiceTests<TData, TId>
    where TData : class, ITitledDocumentData<TData>, new()
    where TId : struct, IGuidId<TId>
{
    protected readonly CompositeDisposable Disposables = new();
    protected readonly IDocumentService DocumentService = Substitute.For<IDocumentService>();
    protected ReadOnlyObservableCollection<IDocumentFile<TData, TId>> Files = null!;

    protected PackFolderService<TData, TId> Subject = null!;
    
    private readonly Dictionary<IStorageFile, IDocumentFile<TData, TId>> _fileMap = new();
    private readonly MemoryStorageFolder _packFolder;
    private readonly string _folderName;
    private readonly string _itemName;

    protected PackFolderServiceTests(string folderName, string itemName)
    {
        _folderName = folderName;
        _itemName = itemName;
        
        var packFolderName = Guid.NewGuid().ToLowercaseString();
        var packFolderPath = Path.Join(Path.GetTempPath(), packFolderName);

        _packFolder = new MemoryStorageFolder(packFolderPath);
    }

    protected IDocumentFile<TData, TId> FileFactory(IStorageFile file)
    {
        var jsonFile = new DocumentFile<TData, TId>(file);
        _fileMap.Add(file, jsonFile);
        
        return jsonFile;
    }

    private async Task<IDocumentFile<TData, TId>?> AddFileAsync(string friendlyId)
    {
        Subject.AddFileDialog
            .RegisterHandler(context =>
            {
                context.Input.InputText = friendlyId;
                context.SetOutput(OperationResult.Success);
            })
            .DisposeWith(Disposables);
        
        return await Subject.AddFileAsync();
    }
    
    [Fact]
    public void FolderName_ShouldReturnExpected()
    {
        Subject.FolderName
            .Should()
            .Be(_folderName);
    }
    
    [Fact]
    public void ItemName_ShouldReturnExpected()
    {
        Subject.ItemName
            .Should()
            .Be(_itemName);
    }

    [Fact]
    public async Task NameValidationRules_ShouldReturnExpectedRules()
    {
        var rules = Subject.NameValidationRules;
        
        rules.Should().HaveCount(2);
        
        rules[0].FailureMessage
            .Should()
            .Be("Name cannot be empty.");
        
        rules[1].FailureMessage
            .Should()
            .Be("Name must be unique.");
        
        rules[0].Rule(null)
            .Should()
            .BeFalse();
        
        rules[0].Rule(string.Empty)
            .Should()
            .BeFalse();
        
        rules[0].Rule("  ")
            .Should()
            .BeFalse();
        
        rules[0].Rule("Test")
            .Should()
            .BeTrue();
        
        rules[1].Rule("Test")
            .Should()
            .BeTrue();
        
        rules[1].Rule("Test2")
            .Should()
            .BeTrue();

        Subject.NewPack(_packFolder);
        
        await AddFileAsync("Test");
        
        rules[1].Rule("Test")
            .Should()
            .BeFalse();
    }

    [Fact]
    public void NewPack_ShouldCreateNewFolder()
    {
        Subject.NewPack(_packFolder);
        
        _packFolder.GetFolder(_folderName)
            .Should()
            .NotBeNull();

        Files.Should()
            .HaveCount(1)
            .And.Contain(NullDocumentFile<TData, TId>.Instance);
    }
    
    [Fact]
    public async Task OpenPackAsync_ShouldCreateNewFolder_WhenFolderDoesNotExist()
    {
        await Subject.OpenPackAsync(_packFolder);

        _packFolder.GetFolder(_folderName)
            .Should()
            .NotBeNull();

        Files.Should()
            .HaveCount(1)
            .And.Contain(NullDocumentFile<TData, TId>.Instance);
    }
    
    [Fact]
    public async Task OpenPackAsync_ShouldAddFiles_WhenFolderExists()
    {
        var folder = _packFolder.CreateFolder(_folderName);
        
        var files = new List<IStorageFile>
        {
            folder.CreateFile(Guid.NewGuid().ToLowercaseString()),
            folder.CreateFile(Guid.NewGuid().ToLowercaseString()),
            folder.CreateFile(Guid.NewGuid().ToLowercaseString())
        };
        
        await Subject.OpenPackAsync(_packFolder);

        _packFolder.GetFolder(_folderName)
            .Should()
            .Be(folder);
        
        folder.GetFiles()
            .Should()
            .Contain(files);

        Files.Should()
            .HaveCount(4)
            .And.Contain(NullDocumentFile<TData, TId>.Instance)
            .And.Contain(_fileMap[files[0]])
            .And.Contain(_fileMap[files[1]])
            .And.Contain(_fileMap[files[2]]);
    }
    
    [Fact]
    public async Task ClosePack_ShouldDisposeFiles()
    {
        var folder = _packFolder.CreateFolder(_folderName);
        
        var files = new List<IStorageFile>
        {
            folder.CreateFile(Guid.NewGuid().ToLowercaseString()),
            folder.CreateFile(Guid.NewGuid().ToLowercaseString()),
            folder.CreateFile(Guid.NewGuid().ToLowercaseString())
        };

        _packFolder.GetFolder(_folderName)
            .Should()
            .Be(folder);
        
        folder.GetFiles()
            .Should()
            .Contain(files);
        
        await Subject.OpenPackAsync(_packFolder);
        
        Subject.ClosePack();
        
        Files.Should().BeEmpty();
    }

    [Fact]
    public async Task AddFileAsync_ShouldAddFile_WhenFolderIsLoaded_WhenDialogIsSuccess()
    {
        const string friendlyId = "Test";
        
        Subject.NewPack(_packFolder);

        Subject.AddFileDialog
            .RegisterHandler(context =>
            {
                context.Input.InputText = friendlyId;
                context.SetOutput(OperationResult.Success);
            })
            .DisposeWith(Disposables);
        
        await Subject.AddFileAsync();
        
        Files.Should()
            .HaveCount(2)
            .And.Contain(NullDocumentFile<TData, TId>.Instance)
            .And.Contain(x => x.FriendlyId == friendlyId);
    }

    [Fact]
    public async Task AddFileAsync_ShouldOpenExceptionDialog_WhenFolderIsLoaded_WhenCreateFileThrowsException()
    {
        var packFolder = Substitute.For<IStorageFolder>();
        
        var folder = Substitute.For<IStorageFolder>();
        
        packFolder.CreateFolder(Arg.Is(_folderName)).Returns(folder);

        folder.When(x => x.CreateFile(Arg.Any<string>()))
            .Throw<Exception>();
        
        Subject.NewPack(packFolder);
        
        Subject.AddFileDialog
            .RegisterHandler(context => context.SetOutput(OperationResult.Success))
            .DisposeWith(Disposables);

        var exceptionDialogShown = false;
        
        Subject.ExceptionDialog
            .RegisterHandler(context =>
            {
                exceptionDialogShown = true;
                context.SetOutput(Unit.Default);
            })
            .DisposeWith(Disposables);
        
        await Subject.AddFileAsync();
        
        exceptionDialogShown.Should().BeTrue();
    }

    [Fact]
    public async Task AddFileAsync_ShouldDoNothing_WhenFolderIsLoaded_WhenDialogIsCancel()
    {
        Subject.NewPack(_packFolder);
        
        Subject.AddFileDialog
            .RegisterHandler(context => context.SetOutput(OperationResult.Cancel))
            .DisposeWith(Disposables);
        
        await Subject.AddFileAsync();
        
        Files.Should()
            .HaveCount(1)
            .And.Contain(NullDocumentFile<TData, TId>.Instance);
    }
    
    [Fact]
    public async Task AddFileAsync_ShouldDoNothing_WhenFolderIsNotLoaded()
    {
        await Subject.AddFileAsync();
        
        Files.Should().BeEmpty();
    }

    [Fact]
    public async Task RenameFileAsync_ShouldRenameFile_WhenFileIsLoaded_WhenDialogIsSuccess()
    {
        const string friendlyId = "Test";
        
        Subject.NewPack(_packFolder);
        
        var file = await AddFileAsync("Test2");
        
        Subject.RenameFileDialog
            .RegisterHandler(context =>
            {
                context.Input.InputText = friendlyId;
                context.SetOutput(OperationResult.Success);
            })
            .DisposeWith(Disposables);
        
        await Subject.RenameFileAsync(file!);
        
        Files.Should().ContainSingle(x => x.FriendlyId == friendlyId);
    }

    [Fact]
    public async Task RenameFileAsync_ShouldOpenExceptionDialog_WhenFileIsLoaded_WhenDialogIsSuccess_WhenRenameThrowsException()
    {
        var packFolder = Substitute.For<IStorageFolder>();
        
        var folder = Substitute.For<IStorageFolder>();
        var file = Substitute.For<IStorageFile>();
        
        packFolder.CreateFolder(Arg.Is(_folderName)).Returns(folder);
        
        folder.CreateFile(Arg.Any<string>())
            .Returns(info =>
            {
                var id = (string)info[0];
                file.Name.Returns(id);
                
                return file;
            });

        file.OpenWrite().Returns(new MemoryStream());
        
        Subject.NewPack(packFolder);
        
        var jsonFile = await AddFileAsync("Test2");
        
        Subject.RenameFileDialog
            .RegisterHandler(context =>
            {
                context.Input.InputText = "Test";
                context.SetOutput(OperationResult.Success);
            })
            .DisposeWith(Disposables);
        
        var exceptionDialogShown = false;
        
        Subject.ExceptionDialog
            .RegisterHandler(context =>
            {
                exceptionDialogShown = true;
                context.SetOutput(Unit.Default);
            })
            .DisposeWith(Disposables);

        file.When(x => x.OpenWrite())
            .Throw<Exception>();
        
        await Subject.RenameFileAsync(jsonFile!);
        
        exceptionDialogShown.Should().BeTrue();
    }
    
    [Fact]
    public async Task RenameFileAsync_ShouldDoNothing_WhenFileIsLoaded_WhenDialogIsCancel()
    {
        Subject.NewPack(_packFolder);
        
        var file = await AddFileAsync("Test");
        
        Subject.RenameFileDialog
            .RegisterHandler(context => context.SetOutput(OperationResult.Cancel))
            .DisposeWith(Disposables);
        
        await Subject.RenameFileAsync(file!);
        
        Files.Should().ContainSingle(x => x.FriendlyId == "Test");
    }
    
    [Fact]
    public async Task RenameFileAsync_ShouldDoNothing_WhenFolderIsNull()
    {
        var file = Substitute.For<IDocumentFile<TData, TId>>();
        
        await Subject.RenameFileAsync(file);
        
        Files.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteFileAsync_ShouldDeleteFile_WhenFileIsLoaded_WhenDialogIsYes()
    {
        Subject.NewPack(_packFolder);
        
        var file = await AddFileAsync("Test");
        
        Subject.DeleteFileDialog
            .RegisterHandler(context => context.SetOutput(YesNoDialogResult.Yes))
            .DisposeWith(Disposables);
        
        await Subject.DeleteFileAsync(file!);
        
        Files.Should()
            .HaveCount(1)
            .And.Contain(NullDocumentFile<TData, TId>.Instance);
    }

    [Fact]
    public async Task DeleteFileAsync_ShouldNotDeleteFile_WhenFileIsLoaded_WhenDialogIsNo()
    {
        
        Subject.NewPack(_packFolder);
        var file = await AddFileAsync("Test");
        
        Subject.DeleteFileDialog
            .RegisterHandler(context => context.SetOutput(YesNoDialogResult.No))
            .DisposeWith(Disposables);
        
        await Subject.DeleteFileAsync(file!);
        
        Files.Should()
            .HaveCount(2)
            .And.Contain(NullDocumentFile<TData, TId>.Instance)
            .And.Contain(file!);
    }

    [Fact]
    public async Task DeleteFileAsync_ShouldOpenExceptionDialog_WhenFileIsLoaded_WhenDialogIsYes_WhenDeleteThrowsException()
    {
        var packFolder = Substitute.For<IStorageFolder>();
        var folder = Substitute.For<IStorageFolder>();
        
        packFolder.CreateFolder(Arg.Is(_folderName)).Returns(folder);
        
        folder
            .CreateFile(Arg.Any<string>())
            .Returns(x =>
            {
                var id = (string)x[0];
                var file = Substitute.For<IStorageFile>();
                file.Name.Returns(id);
                file.OpenWrite().Returns(_ => new MemoryStream());
                file.When(y => y.Delete())
                    .Throw<Exception>();

                return file;
            });
        
        Subject.NewPack(packFolder);
        
        var jsonFile = await AddFileAsync("Test");
        
        Subject.DeleteFileDialog
            .RegisterHandler(context => context.SetOutput(YesNoDialogResult.Yes))
            .DisposeWith(Disposables);
        
        var exceptionDialogShown = false;
        
        Subject.ExceptionDialog
            .RegisterHandler(context =>
            {
                exceptionDialogShown = true;
                context.SetOutput(Unit.Default);
            })
            .DisposeWith(Disposables);
        
        await Subject.DeleteFileAsync(jsonFile!);

        exceptionDialogShown.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteFileAsync_ShouldDoNothing_WhenFolderIsNull()
    {
        var file = Substitute.For<IDocumentFile<TData, TId>>();
        
        await Subject.DeleteFileAsync(file);
        
        Files.Should().BeEmpty();
        
    }
    
    [Fact]
    public async Task DeleteFileAsync_ShouldDoNothing_WhenFileIsNotLoaded()
    {
        var file = Substitute.For<IDocumentFile<TData, TId>>();
        
        Subject.NewPack(_packFolder);
        await Subject.DeleteFileAsync(file);
        
        Files.Should()
            .HaveCount(1)
            .And.Contain(NullDocumentFile<TData, TId>.Instance);
    }
    
    [Fact]
    public async Task GetFile_ShouldReturnExpectedFile_WhenFileIsLoaded()
    {
        Subject.NewPack(_packFolder);
        
        var file = await AddFileAsync("Test");
        
        Subject.GetFile(file!.Id)
            .Should()
            .Be(file);
    }
    
    [Fact]
    public void GetFile_ShouldReturnNullFile_WhenFileIsNotLoaded()
    {
        Subject.NewPack(_packFolder);

        Subject.GetFile(TId.New())
            .Should()
            .BeSameAs(NullDocumentFile<TData, TId>.Instance);
    }

    [Fact]
    public void SplatResolve_ShouldResolveToSingleInstance()
    {
        var subject1 = Locator.Current.GetService<IPackFolderService<TData, TId>>();
        var subject2 = Locator.Current.GetService<IPackFolderService<TData, TId>>();

        subject1.Should().BeOfType(Subject.GetType());
        subject2.Should().BeOfType(Subject.GetType());
        
        subject1.Should().BeSameAs(subject2);
    }
    
    [Fact]
    public void SplatResolve_ShouldResolveToLazySingleInstance()
    {
        var lazy = Locator.Current.GetService<Lazy<IPackFolderService<TData, TId>>>()!;
        var subject = Locator.Current.GetService<IPackFolderService<TData, TId>>()!;

        lazy.Value
            .Should()
            .BeOfType(Subject.GetType());
        
        subject.Should().BeOfType(Subject.GetType());
        
        lazy.Value
            .Should()
            .BeSameAs(subject);
    }
}