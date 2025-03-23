using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Ids;
using OpenTrackerNext.Core.Storage;
using OpenTrackerNext.Core.Storage.Memory;
using OpenTrackerNext.Editor.Documents;
using OpenTrackerNext.Editor.Pack;
using Splat;
using Xunit;

namespace OpenTrackerNext.Editor.UnitTests.Pack;

[ExcludeFromCodeCoverage]
public abstract class PackFileServiceTests<TData>
    where TData : class, ITitledDocumentData<TData>, new()
{
    protected readonly IDocumentService DocumentService = Substitute.For<IDocumentService>();

    protected PackFileService<TData> Subject = null!;

    private IDocumentFile<TData>? _file;

    private readonly MemoryStorageFolder _packFolder;
    private readonly string _fileName;
    
    protected IDocumentFile<TData> FileFactory(IStorageFile file)
    {
        _file = new DocumentFile<TData>(file);
        
        return _file;
    }

    protected PackFileServiceTests(string fileName)
    {
        _fileName = fileName;
        
        var packFolderName = Guid.NewGuid().ToLowercaseString();
        var packFolderPath = Path.Join(Path.GetTempPath(), packFolderName);
        
        _packFolder = new MemoryStorageFolder(packFolderPath);
    }
    
    [Fact]
    public void FileName_ShouldReturnExpected()
    {
        Subject.FileName
            .Should()
            .Be(_fileName);
    }

    [Fact]
    public void File_ShouldInitializeAsNull()
    {
        Subject.File
            .Should()
            .BeNull();
    }
    
    [Fact]
    public async Task NewPackAsync_ShouldCreateNewFile()
    {
        await Subject.NewPackAsync(_packFolder);
        
        Subject.File
            .Should()
            .NotBeNull()
            .And.BeSameAs(_file);
    }

    [Fact]
    public async Task OpenPackAsync_ShouldOpenExistingFile_WhenFileExists()
    {
        _packFolder.CreateFile(_fileName);
        
        await Subject.OpenPackAsync(_packFolder);
        
        Subject.File
            .Should()
            .NotBeNull()
            .And.BeSameAs(_file);
    }
    
    [Fact]
    public async Task OpenPackAsync_ShouldCreateNewFile_WhenFileDoesNotExist()
    {
        await Subject.OpenPackAsync(_packFolder);
        
        Subject.File
            .Should()
            .NotBeNull()
            .And.BeSameAs(_file);
    }

    [Fact]
    public async Task ClosePack_ShouldCloseDocument_ShouldDisposeFile_ShouldSetFileToNull_WhenFileIsLoaded()
    {
        await Subject.NewPackAsync(_packFolder);
        
        Subject.ClosePack();
        
        DocumentService.Received(1)!.Close(Arg.Is(_file!));
        
        Subject.File
            .Should()
            .BeNull();
    }
    
    [Fact]
    public void ClosePack_ShouldDoNothing_WhenFileIsNull()
    {
        Subject.ClosePack();
        
        DocumentService
            .DidNotReceive()
            .Close(Arg.Any<IDocument>());
    }

    [Fact]
    public void SplatResolve_ShouldResolveToSingleInstance()
    {
        var subject1 = Locator.Current.GetService<IPackFileService<TData>>()!;
        var subject2 = Locator.Current.GetService<IPackFileService<TData>>()!;

        subject1.Should().BeOfType(Subject.GetType());
        subject2.Should().BeOfType(Subject.GetType());
        
        subject1.Should().BeSameAs(subject2);
    }

    [Fact]
    public void SplatResolve_ShouldResolveToLazySingleInstance()
    {
        var lazy = Locator.Current.GetService<Lazy<IPackFileService<TData>>>()!;
        var subject = Locator.Current.GetService<IPackFileService<TData>>()!;
        
        lazy.Value
            .Should()
            .BeOfType(Subject.GetType());
        
        subject.Should().BeOfType(Subject.GetType());

        lazy.Value
            .Should()
            .BeSameAs(subject);
    }
}