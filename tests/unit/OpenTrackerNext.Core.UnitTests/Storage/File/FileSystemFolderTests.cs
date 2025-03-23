using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using FluentAssertions;
using OpenTrackerNext.Core.Storage;
using OpenTrackerNext.Core.Storage.File;
using Splat;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Storage.File;

[ExcludeFromCodeCoverage]
public sealed class FileSystemFolderTests : IDisposable
{
    private readonly string _testPath;
    private readonly string _testName;

    private readonly FileSystemFolder _subject;
    
    public FileSystemFolderTests()
    {
        var tempPath = Path.GetTempPath();
        _testName = Guid.NewGuid().ToString();
        _testPath = Path.Combine(tempPath, _testName);
        
        _subject = new FileSystemFolder(_testPath);
    }
    
    public void Dispose()
    {
        if (Directory.Exists(_testPath))
        {
            Directory.Delete(_testPath, true);
        }
    }

    [Fact]
    public void FullPath_ShouldReturnExpected()
    {
        _subject.FullPath
            .Should()
            .Be(_testPath);
    }
    
    [Fact]
    public void Name_ShouldReturnExpected()
    {
        _subject.Name
            .Should()
            .Be(_testName);
    }
    
    [Fact]
    public void Exists_ShouldReturnFalse_WhenFolderDoesNotExist()
    {
        _subject.Exists()
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void Exists_ShouldReturnTrue_WhenFolderExists()
    {
        Directory.CreateDirectory(_testPath);
        
        _subject.Exists()
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void Delete_ShouldDeleteFolder_WhenFolderExists()
    {
        Directory.CreateDirectory(_testPath);
        
        _subject.Delete();
        
        _subject.Exists().Should().BeFalse();
    }
    
    [Fact]
    public void Delete_ShouldDoNothing_WhenFolderDoesNotExist()
    {
        _subject.Delete();
        
        _subject.Exists().Should().BeFalse();
    }
    
    [Fact]
    public void Create_ShouldCreateFolder_WhenFolderDoesNotExist()
    {
        _subject.Create();
        
        _subject.Exists().Should().BeTrue();
    }
    
    [Fact]
    public void Create_ShouldDoNothing_WhenFolderExists()
    {
        Directory.CreateDirectory(_testPath);
        
        _subject.Create();
        
        _subject.Exists().Should().BeTrue();
    }
    
    [Fact]
    public void CreateFolder_ShouldCreateSubFolder_WhenFolderDoesNotExist()
    {
        const string name = "test";
        
        var createdFolder = _subject.CreateFolder(name);
        
        _subject.Exists().Should().BeTrue();
        
        _subject.GetFolder("test")
            .Should()
            .NotBeNull();
        
        createdFolder.FullPath.Should().Be(Path.Combine(_testPath, name));
    }
    
    [Fact]
    public void CreateFolder_ShouldCreateSubFolder_WhenSubFolderDoesNotExist()
    {
        const string name = "test";
        
        Directory.CreateDirectory(_testPath);
        
        var createdFolder = _subject.CreateFolder(name);
        
        _subject.Exists().Should().BeTrue();
        
        _subject.GetFolder("test")
            .Should()
            .NotBeNull();
        
        createdFolder.FullPath.Should().Be(Path.Combine(_testPath, name));
    }
    
    [Fact]
    public void CreateFolder_ShouldDoNothing_WhenSubFolderExists()
    {
        const string name = "test";
        
        Directory.CreateDirectory(_testPath);
        Directory.CreateDirectory(Path.Combine(_testPath, name));
        
        var createdFolder = _subject.CreateFolder(name);
        
        _subject.Exists().Should().BeTrue();
        
        _subject.GetFolder("test")
            .Should()
            .NotBeNull();
        
        createdFolder.FullPath.Should().Be(Path.Combine(_testPath, name));
    }
    
    [Fact]
    public void CreateFolder_ShouldThrowIOException_WhenSubFolderExistsAsFile()
    {
        const string name = "test";
        
        Directory.CreateDirectory(_testPath);
        new FileInfo(Path.Combine(_testPath, name)).Create().Dispose();
        
        _subject.Invoking(x => x.CreateFolder(name))
            .Should()
            .Throw<IOException>();
    }
    
    [Fact]
    public void CreateFile_ShouldCreateFile_WhenFolderDoesNotExist()
    {
        const string name = "test.txt";
        
        _subject.CreateFile(name);
        
        _subject.Exists().Should().BeTrue();
        
        _subject.GetFile(name).Should().NotBeNull();
    }
    
    [Fact]
    public void CreateFile_ShouldCreateFile_WhenFileDoesNotExist()
    {
        const string name = "test.txt";
        
        Directory.CreateDirectory(_testPath);
        
        _subject.CreateFile(name);
        
        _subject.Exists().Should().BeTrue();
        
        _subject.GetFile(name).Should().NotBeNull();
    }
    
    [Fact]
    public void CreateFile_ShouldDoNothing_WhenFileExists()
    {
        const string name = "test.txt";
        
        Directory.CreateDirectory(_testPath);
        new FileInfo(Path.Combine(_testPath, name)).Create().Dispose();
        
        _subject.CreateFile(name);
        
        _subject.Exists().Should().BeTrue();
        
        _subject.GetFile(name).Should().NotBeNull();
    }
    
    [Fact]
    public void CreateFile_ShouldThrowUnauthorizedAccessException_WhenFileExistsAsFolder()
    {
        const string name = "test";
        
        Directory.CreateDirectory(_testPath);
        Directory.CreateDirectory(Path.Combine(_testPath, name));
        
        _subject.Invoking(x => x.CreateFile(name))
            .Should()
            .Throw<UnauthorizedAccessException>();
    }
    
    [Fact]
    public void GetFolder_ShouldReturnNull_WhenFolderDoesNotExist()
    {
        _subject.GetFolder("test").Should().BeNull();
    }
    
    [Fact]
    public void GetFolder_ShouldReturnNull_WhenFolderExists()
    {
        const string name = "test";
        
        Directory.CreateDirectory(_testPath);
        Directory.CreateDirectory(Path.Combine(_testPath, name));
        
        _subject.GetFolder(name).Should().NotBeNull();
    }
    
    [Fact]
    public void GetFolder_ShouldReturnNull_WhenFolderExistsAndIsFile()
    {
        const string name = "test.txt";
        
        Directory.CreateDirectory(_testPath);
        new FileInfo(Path.Combine(_testPath, name)).Create().Dispose();
        
        _subject.GetFolder(name).Should().BeNull();
    }
    
    [Fact]
    public void GetFolder_ReadOnly_ShouldReturnNull_WhenFolderDoesNotExist()
    {
        ((IReadOnlyStorageFolder)_subject)
            .GetFolder("test")
            .Should().BeNull();
    }
    
    [Fact]
    public void GetFolder_ReadOnly_ShouldReturnNull_WhenFolderExists()
    {
        const string name = "test";
        
        Directory.CreateDirectory(_testPath);
        Directory.CreateDirectory(Path.Combine(_testPath, name));

        ((IReadOnlyStorageFolder)_subject)
            .GetFolder(name)
            .Should().NotBeNull();
    }
    
    [Fact]
    public void GetFolder_ReadOnly_ShouldReturnNull_WhenFolderExistsAndIsFile()
    {
        const string name = "test.txt";
        
        Directory.CreateDirectory(_testPath);
        new FileInfo(Path.Combine(_testPath, name)).Create().Dispose();

        ((IReadOnlyStorageFolder)_subject)
            .GetFolder(name)
            .Should().BeNull();
    }
    
    [Fact]
    public void GetFile_ShouldReturnNull_WhenFileDoesNotExist()
    {
        _subject.GetFile("test.txt").Should().BeNull();
    }
    
    [Fact]
    public void GetFile_ShouldReturnNull_WhenFileExists()
    {
        const string name = "test.txt";
        
        Directory.CreateDirectory(_testPath);
        new FileInfo(Path.Combine(_testPath, name))
            .Create()
            .Dispose();
        
        _subject.GetFile(name).Should().NotBeNull();
    }
    
    [Fact]
    public void GetFile_ShouldReturnNull_WhenFileExistsAndIsFolder()
    {
        const string name = "test";
        
        Directory.CreateDirectory(_testPath);
        Directory.CreateDirectory(Path.Combine(_testPath, name));
        
        _subject.GetFile(name).Should().BeNull();
    }
    
    [Fact]
    public void GetFile_ReadOnly_ShouldReturnNull_WhenFileDoesNotExist()
    {
        ((IReadOnlyStorageFolder)_subject)
            .GetFile("test.txt")
            .Should().BeNull();
    }
    
    [Fact]
    public void GetFile_ReadOnly_ShouldReturnNull_WhenFileExists()
    {
        const string name = "test.txt";
        
        Directory.CreateDirectory(_testPath);
        new FileInfo(Path.Combine(_testPath, name)).Create().Dispose();

        ((IReadOnlyStorageFolder)_subject)
            .GetFile(name)
            .Should().NotBeNull();
    }
    
    [Fact]
    public void GetFile_ReadOnly_ShouldReturnNull_WhenFileExistsAndIsFolder()
    {
        const string name = "test";
        
        Directory.CreateDirectory(_testPath);
        Directory.CreateDirectory(Path.Combine(_testPath, name));

        ((IReadOnlyStorageFolder)_subject)
            .GetFile(name)
            .Should().BeNull();
    }
    
    [Fact]
    public void GetItems_ShouldReturnEmpty_WhenFolderDoesNotExist()
    {
        _subject.GetItems().Should().BeEmpty();
    }
    
    [Fact]
    public void GetItems_ShouldReturnEmpty_WhenFolderExists_WhenFolderIsEmpty()
    {
        Directory.CreateDirectory(_testPath);
        
        _subject.GetItems().Should().BeEmpty();
    }
    
    [Fact]
    public void GetItems_ShouldReturnExpected_WhenFolderExists_WhenFolderContainsFiles()
    {
        const string name1 = "test1.txt";
        const string name2 = "test2.txt";
        
        Directory.CreateDirectory(_testPath);
        
        new FileInfo(Path.Combine(_testPath, name1)).Create().Dispose();
        new FileInfo(Path.Combine(_testPath, name2)).Create().Dispose();
        
        var items = _subject.GetItems().ToList();
        
        items.Should().HaveCount(2)
            .And.Contain(x => x.Name == name1)
            .And.Contain(x => x.Name == name2);
    }
    
    [Fact]
    public void GetItems_ShouldReturnExpected_WhenFolderExists_WhenFolderContainsFolders()
    {
        const string name1 = "test1";
        const string name2 = "test2";
        
        Directory.CreateDirectory(_testPath);
        Directory.CreateDirectory(Path.Combine(_testPath, name1));
        Directory.CreateDirectory(Path.Combine(_testPath, name2));
        
        var items = _subject
            .GetItems()
            .ToList();
        
        items.Should().HaveCount(2)
            .And.Contain(x => x.Name == name1)
            .And.Contain(x => x.Name == name2);
    }
    
    [Fact]
    public void GetItems_ShouldReturnExpected_WhenFolderExists_WhenFolderContainsFilesAndFolders()
    {
        const string name1 = "test1.txt";
        const string name2 = "test2";
        
        Directory.CreateDirectory(_testPath);
        
        new FileInfo(Path.Combine(_testPath, name1)).Create().Dispose();
        
        Directory.CreateDirectory(Path.Combine(_testPath, name2));
        
        var items = _subject
            .GetItems()
            .ToList();
        
        items.Should().HaveCount(2)
            .And.Contain(x => x.Name == name1)
            .And.Contain(x => x.Name == name2);
    }
    
    [Fact]
    public void GetItems_ReadOnly_ShouldReturnEmpty_WhenFolderDoesNotExist()
    {
        ((IReadOnlyStorageFolder) _subject)
            .GetItems()
            .Should().BeEmpty();
    }
    
    [Fact]
    public void GetItems_ReadOnly_ShouldReturnEmpty_WhenFolderExists_WhenFolderIsEmpty()
    {
        Directory.CreateDirectory(_testPath);
        
        ((IReadOnlyStorageFolder) _subject)
            .GetItems()
            .Should().BeEmpty();
    }
    
    [Fact]
    public void GetItems_ReadOnly_ShouldReturnExpected_WhenFolderExists_WhenFolderContainsFiles()
    {
        const string name1 = "test1.txt";
        const string name2 = "test2.txt";
        
        Directory.CreateDirectory(_testPath);
        
        new FileInfo(Path.Combine(_testPath, name1)).Create().Dispose();
        new FileInfo(Path.Combine(_testPath, name2)).Create().Dispose();
        
        var items = ((IReadOnlyStorageFolder) _subject)
            .GetItems()
            .ToList();
        
        items.Should().HaveCount(2)
            .And.Contain(x => x.Name == name1)
            .And.Contain(x => x.Name == name2);
    }
    
    [Fact]
    public void GetItems_ReadOnly_ShouldReturnExpected_WhenFolderExists_WhenFolderContainsFolders()
    {
        const string name1 = "test1";
        const string name2 = "test2";
        
        Directory.CreateDirectory(_testPath);
        
        Directory.CreateDirectory(Path.Combine(_testPath, name1));
        Directory.CreateDirectory(Path.Combine(_testPath, name2));
        
        var items = ((IReadOnlyStorageFolder) _subject)
            .GetItems()
            .ToList();
        
        items.Should().HaveCount(2)
            .And.Contain(x => x.Name == name1)
            .And.Contain(x => x.Name == name2);
    }
    
    [Fact]
    public void GetItems_ReadOnly_ShouldReturnExpected_WhenFolderExists_WhenFolderContainsFilesAndFolders()
    {
        const string name1 = "test1.txt";
        const string name2 = "test2";
        
        Directory.CreateDirectory(_testPath);
        
        new FileInfo(Path.Combine(_testPath, name1)).Create().Dispose();
        
        Directory.CreateDirectory(Path.Combine(_testPath, name2));
        
        var items = ((IReadOnlyStorageFolder) _subject)
            .GetItems()
            .ToList();
        
        items.Should().HaveCount(2)
            .And.Contain(x => x.Name == name1)
            .And.Contain(x => x.Name == name2);
    }
    
    [Fact]
    public void GetFolders_ShouldReturnEmpty_WhenFolderDoesNotExist()
    {
        _subject.GetFolders().Should().BeEmpty();
    }
    
    [Fact]
    public void GetFolders_ShouldReturnEmpty_WhenFolderExists_WhenFolderIsEmpty()
    {
        Directory.CreateDirectory(_testPath);
        
        _subject.GetFolders().Should().BeEmpty();
    }
    
    [Fact]
    public void GetFolders_ShouldReturnEmpty_WhenFolderExists_WhenFolderContainsFiles()
    {
        const string name1 = "test1.txt";
        const string name2 = "test2.txt";
        
        Directory.CreateDirectory(_testPath);
        
        new FileInfo(Path.Combine(_testPath, name1)).Create().Dispose();
        new FileInfo(Path.Combine(_testPath, name2)).Create().Dispose();
        
        _subject.GetFolders().Should().BeEmpty();
    }
    
    [Fact]
    public void GetFolders_ShouldReturnExpected_WhenFolderExists_WhenFolderContainsFolders()
    {
        const string name1 = "test1";
        const string name2 = "test2";
        
        Directory.CreateDirectory(_testPath);
        
        Directory.CreateDirectory(Path.Combine(_testPath, name1));
        Directory.CreateDirectory(Path.Combine(_testPath, name2));
        
        var items = _subject
            .GetFolders()
            .ToList();
        
        items.Should().HaveCount(2)
            .And.Contain(x => x.Name == name1)
            .And.Contain(x => x.Name == name2);
    }
    
    [Fact]
    public void GetFolders_ShouldReturnExpected_WhenFolderExists_WhenFolderContainsFilesAndFolders()
    {
        const string name1 = "test1.txt";
        const string name2 = "test2";
        
        Directory.CreateDirectory(_testPath);
        
        new FileInfo(Path.Combine(_testPath, name1)).Create().Dispose();
        
        Directory.CreateDirectory(Path.Combine(_testPath, name2));
        
        var items = _subject
            .GetFolders()
            .ToList();
        
        items.Should()
            .ContainSingle()
            .And.Contain(x => x.Name == name2);
    }
    
    [Fact]
    public void GetFolders_ReadOnly_ShouldReturnEmpty_WhenFolderDoesNotExist()
    {
        ((IReadOnlyStorageFolder) _subject)
            .GetFolders()
            .Should().BeEmpty();
    }
    
    [Fact]
    public void GetFolders_ReadOnly_ShouldReturnEmpty_WhenFolderExists_WhenFolderIsEmpty()
    {
        Directory.CreateDirectory(_testPath);
        
        ((IReadOnlyStorageFolder) _subject)
            .GetFolders()
            .Should().BeEmpty();
    }
    
    [Fact]
    public void GetFolders_ReadOnly_ShouldReturnEmpty_WhenFolderExists_WhenFolderContainsFiles()
    {
        const string name1 = "test1.txt";
        const string name2 = "test2.txt";
        
        Directory.CreateDirectory(_testPath);
        
        new FileInfo(Path.Combine(_testPath, name1)).Create().Dispose();
        new FileInfo(Path.Combine(_testPath, name2)).Create().Dispose();
        
        ((IReadOnlyStorageFolder) _subject)
            .GetFolders()
            .Should().BeEmpty();
    }
    
    [Fact]
    public void GetFolders_ReadOnly_ShouldReturnExpected_WhenFolderExists_WhenFolderContainsFolders()
    {
        const string name1 = "test1";
        const string name2 = "test2";
        
        Directory.CreateDirectory(_testPath);
        
        Directory.CreateDirectory(Path.Combine(_testPath, name1));
        Directory.CreateDirectory(Path.Combine(_testPath, name2));
        
        var items = ((IReadOnlyStorageFolder) _subject)
            .GetFolders()
            .ToList();
        
        items.Should().HaveCount(2)
            .And.Contain(x => x.Name == name1)
            .And.Contain(x => x.Name == name2);
    }
    
    [Fact]
    public void GetFolders_ReadOnly_ShouldReturnExpected_WhenFolderExists_WhenFolderContainsFilesAndFolders()
    {
        const string name1 = "test1.txt";
        const string name2 = "test2";
        
        Directory.CreateDirectory(_testPath);
        
        new FileInfo(Path.Combine(_testPath, name1)).Create().Dispose();
        
        Directory.CreateDirectory(Path.Combine(_testPath, name2));
        
        var items = ((IReadOnlyStorageFolder) _subject)
            .GetFolders()
            .ToList();
        
        items.Should()
            .ContainSingle()
            .And.Contain(x => x.Name == name2);
    }
    
    [Fact]
    public void GetFiles_ShouldReturnEmpty_WhenFolderDoesNotExist()
    {
        _subject.GetFiles().Should().BeEmpty();
    }
    
    [Fact]
    public void GetFiles_ShouldReturnEmpty_WhenFolderExists_WhenFolderIsEmpty()
    {
        Directory.CreateDirectory(_testPath);
        
        _subject.GetFiles().Should().BeEmpty();
    }
    
    [Fact]
    public void GetFiles_ShouldReturnExpected_WhenFolderExists_WhenFolderContainsFiles()
    {
        const string name1 = "test1.txt";
        const string name2 = "test2.txt";
        
        Directory.CreateDirectory(_testPath);
        
        new FileInfo(Path.Combine(_testPath, name1)).Create().Dispose();
        new FileInfo(Path.Combine(_testPath, name2)).Create().Dispose();
        
        var items = _subject
            .GetFiles()
            .ToList();
        
        items.Should().HaveCount(2)
            .And.Contain(x => x.Name == name1)
            .And.Contain(x => x.Name == name2);
    }
    
    [Fact]
    public void GetFiles_ShouldReturnEmpty_WhenFolderExists_WhenFolderContainsFolders()
    {
        const string name1 = "test1";
        const string name2 = "test2";
        
        Directory.CreateDirectory(_testPath);
        
        Directory.CreateDirectory(Path.Combine(_testPath, name1));
        Directory.CreateDirectory(Path.Combine(_testPath, name2));
        
        _subject.GetFiles().Should().BeEmpty();
    }
    
    [Fact]
    public void GetFiles_ReadOnly_ShouldReturnEmpty_WhenFolderDoesNotExist()
    {
        ((IReadOnlyStorageFolder) _subject)
            .GetFiles()
            .Should().BeEmpty();
    }
    
    [Fact]
    public void GetFiles_ReadOnly_ShouldReturnEmpty_WhenFolderExists_WhenFolderIsEmpty()
    {
        Directory.CreateDirectory(_testPath);
        
        ((IReadOnlyStorageFolder) _subject)
            .GetFiles()
            .Should().BeEmpty();
    }
    
    [Fact]
    public void GetFiles_ReadOnly_ShouldReturnExpected_WhenFolderExists_WhenFolderContainsFiles()
    {
        const string name1 = "test1.txt";
        const string name2 = "test2.txt";
        
        Directory.CreateDirectory(_testPath);
        
        new FileInfo(Path.Combine(_testPath, name1)).Create().Dispose();
        new FileInfo(Path.Combine(_testPath, name2)).Create().Dispose();
        
        var items = ((IReadOnlyStorageFolder) _subject)
            .GetFiles()
            .ToList();
        
        items.Should().HaveCount(2)
            .And.Contain(x => x.Name == name1)
            .And.Contain(x => x.Name == name2);
    }
    
    [Fact]
    public void GetFiles_ReadOnly_ShouldReturnEmpty_WhenFolderExists_WhenFolderContainsFolders()
    {
        const string name1 = "test1";
        const string name2 = "test2";
        
        Directory.CreateDirectory(_testPath);
        
        Directory.CreateDirectory(Path.Combine(_testPath, name1));
        Directory.CreateDirectory(Path.Combine(_testPath, name2));
        
        ((IReadOnlyStorageFolder) _subject)
            .GetFiles()
            .Should().BeEmpty();
    }
    
    [Fact]
    public void SplatResolve_ShouldResolveToFactory()
    {
        var factory = Locator.Current.GetService<IStorageFolder.Factory>()!;
        
        var subject1 = factory(_testPath);
        var subject2 = factory(_testPath);

        subject1.Should().BeOfType<FileSystemFolder>()
            .Should().NotBeSameAs(subject2);
    }
}