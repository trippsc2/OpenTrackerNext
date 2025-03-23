using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using FluentAssertions;
using OpenTrackerNext.Core.Storage;
using OpenTrackerNext.Core.Storage.Memory;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Storage.Memory;

[ExcludeFromCodeCoverage]
public sealed class MemoryStorageFolderTests
{
    private readonly string _testPath;
    private readonly string _testName;

    private readonly MemoryStorageFolder _subject;
    
    public MemoryStorageFolderTests()
    {
        var tempPath = Path.GetTempPath();
        _testName = Guid.NewGuid().ToString();
        _testPath = Path.Combine(tempPath, _testName);
        
        _subject = new MemoryStorageFolder(_testPath);
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
        _subject.Name.Should().Be(_testName);
    }
    
    [Fact]
    public void Exists_ShouldReturnFalse_WhenFolderDoesNotExist()
    {
        _subject.Exists().Should().BeFalse();
    }
    
    [Fact]
    public void Exists_ShouldReturnTrue_WhenFolderExists()
    {
        _subject.Create();
        
        _subject.Exists().Should().BeTrue();
    }
    
    [Fact]
    public void Delete_ShouldDeleteFolder_WhenFolderExists()
    {
        _subject.Create();
        
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
        _subject.Create();
        _subject.Create();
        
        _subject.Exists().Should().BeTrue();
    }
    
    [Fact]
    public void CreateFolder_ShouldCreateSubFolder_WhenFolderDoesNotExist()
    {
        const string name = "test";
        
        var createdFolder = _subject.CreateFolder(name);
        
        _subject.Exists().Should().BeTrue();
        _subject.GetFolder("test").Should().NotBeNull();
        
        createdFolder.FullPath.Should().Be(Path.Combine(_testPath, name));
    }
    
    [Fact]
    public void CreateFolder_ShouldCreateSubFolder_WhenSubFolderDoesNotExist()
    {
        const string name = "test";
        
        _subject.Create();
        
        var createdFolder = _subject.CreateFolder(name);
        
        _subject.Exists().Should().BeTrue();
        _subject.GetFolder("test").Should().NotBeNull();
        
        createdFolder.FullPath.Should().Be(Path.Combine(_testPath, name));
    }
    
    [Fact]
    public void CreateFolder_ShouldDoNothing_WhenSubFolderExists()
    {
        const string name = "test";
        
        _subject.Create();
        _ = _subject.CreateFolder(name);
        
        var createdFolder = _subject.CreateFolder(name);
        
        _subject.Exists().Should().BeTrue();
        _subject.GetFolder("test").Should().NotBeNull();
        
        createdFolder.FullPath.Should().Be(Path.Combine(_testPath, name));
    }
    
    [Fact]
    public void CreateFolder_ShouldThrowIOException_WhenSubFolderExistsAsFile()
    {
        const string name = "test";
        
        _subject.Create();
        _subject.CreateFile(name)
            .OpenWrite()
            .Dispose();

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
        
        _subject.Create();
        
        _subject.CreateFile(name);
        
        _subject.Exists().Should().BeTrue();
        _subject.GetFile(name).Should().NotBeNull();
    }
    
    [Fact]
    public void CreateFile_ShouldDoNothing_WhenFileExists()
    {
        const string name = "test.txt";
        
        _subject.Create();
        _subject.CreateFile(name)
            .OpenWrite()
            .Dispose();
        
        _subject.CreateFile(name);
        
        _subject.Exists().Should().BeTrue();
        _subject.GetFile(name).Should().NotBeNull();
    }
    
    [Fact]
    public void CreateFile_ShouldThrowUnauthorizedAccessException_WhenFileExistsAsFolder()
    {
        const string name = "test";
        
        _subject.Create();
        _subject.CreateFolder(name);
        
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
        
        _subject.Create();
        _subject.CreateFolder(name);
        
        _subject.GetFolder(name).Should().NotBeNull();
    }
    
    [Fact]
    public void GetFolder_ShouldReturnNull_WhenFolderExistsAndIsFile()
    {
        const string name = "test.txt";
        
        _subject.Create();
        _subject.CreateFile(name)
            .OpenWrite()
            .Dispose();
        
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
        
        _subject.Create();
        _subject.CreateFolder(name);

        ((IReadOnlyStorageFolder)_subject)
            .GetFolder(name)
            .Should().NotBeNull();
    }
    
    [Fact]
    public void GetFolder_ReadOnly_ShouldReturnNull_WhenFolderExistsAndIsFile()
    {
        const string name = "test.txt";
        
        _subject.Create();
        _subject.CreateFile(name)
            .OpenWrite()
            .Dispose();

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
        
        _subject.Create();
        _subject.CreateFile(name)
            .OpenWrite()
            .Dispose();
        
        _subject.GetFile(name).Should().NotBeNull();
    }
    
    [Fact]
    public void GetFile_ShouldReturnNull_WhenFileExistsAndIsFolder()
    {
        const string name = "test";
        
        _subject.Create();
        _subject.CreateFolder(name);
        
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
        
        _subject.Create();
        _subject.CreateFile(name)
            .OpenWrite()
            .Dispose();

        ((IReadOnlyStorageFolder)_subject)
            .GetFile(name)
            .Should().NotBeNull();
    }
    
    [Fact]
    public void GetFile_ReadOnly_ShouldReturnNull_WhenFileExistsAndIsFolder()
    {
        const string name = "test";
        
        _subject.Create();
        _subject.CreateFolder(name);

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
        _subject.Create();
        
        _subject.GetItems().Should().BeEmpty();
    }
    
    [Fact]
    public void GetItems_ShouldReturnExpected_WhenFolderExists_WhenFolderContainsFiles()
    {
        const string name1 = "test1.txt";
        const string name2 = "test2.txt";
        
        _subject.Create();
        _subject.CreateFile(name1)
            .OpenWrite()
            .Dispose();
        _subject.CreateFile(name2)
            .OpenWrite()
            .Dispose();
        
        var items = _subject
            .GetItems()
            .ToList();
        
        items.Should().HaveCount(2)
            .And.Contain(x => x.Name == name1)
            .And.Contain(x => x.Name == name2);
    }
    
    [Fact]
    public void GetItems_ShouldReturnExpected_WhenFolderExists_WhenFolderContainsFolders()
    {
        const string name1 = "test1";
        const string name2 = "test2";
        
        _subject.Create();
        _subject.CreateFolder(name1);
        _subject.CreateFolder(name2);
        
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
        
        _subject.Create();
        _subject.CreateFile(name1)
            .OpenWrite()
            .Dispose();
        _subject.CreateFolder(name2);
        
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
        _subject.Create();
        
        ((IReadOnlyStorageFolder) _subject)
            .GetItems()
            .Should().BeEmpty();
    }
    
    [Fact]
    public void GetItems_ReadOnly_ShouldReturnExpected_WhenFolderExists_WhenFolderContainsFiles()
    {
        const string name1 = "test1.txt";
        const string name2 = "test2.txt";

        _subject.Create();
        _subject.CreateFile(name1)
            .OpenWrite()
            .Dispose();
        _subject.CreateFile(name2)
            .OpenWrite()
            .Dispose();
        
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
        
        _subject.Create();
        _subject.CreateFolder(name1);
        _subject.CreateFolder(name2);
        
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
        
        _subject.Create();
        _subject.CreateFile(name1)
            .OpenWrite()
            .Dispose();
        _subject.CreateFolder(name2);
        
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
        _subject.Create();
        
        _subject.GetFolders().Should().BeEmpty();
    }
    
    [Fact]
    public void GetFolders_ShouldReturnEmpty_WhenFolderExists_WhenFolderContainsFiles()
    {
        const string name1 = "test1.txt";
        const string name2 = "test2.txt";
        
        _subject.Create();
        _subject.CreateFile(name1)
            .OpenWrite()
            .Dispose();
        _subject.CreateFile(name2)
            .OpenWrite()
            .Dispose();
        
        _subject.GetFolders().Should().BeEmpty();
    }
    
    [Fact]
    public void GetFolders_ShouldReturnExpected_WhenFolderExists_WhenFolderContainsFolders()
    {
        const string name1 = "test1";
        const string name2 = "test2";
        
        _subject.Create();
        _subject.CreateFolder(name1);
        _subject.CreateFolder(name2);
        
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
        
        _subject.Create();
        _subject.CreateFile(name1)
            .OpenWrite()
            .Dispose();
        _subject.CreateFolder(name2);
        
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
        _subject.Create();
        
        ((IReadOnlyStorageFolder) _subject)
            .GetFolders()
            .Should().BeEmpty();
    }
    
    [Fact]
    public void GetFolders_ReadOnly_ShouldReturnEmpty_WhenFolderExists_WhenFolderContainsFiles()
    {
        const string name1 = "test1.txt";
        const string name2 = "test2.txt";
        
        _subject.Create();
        _subject.CreateFile(name1)
            .OpenWrite()
            .Dispose();
        _subject.CreateFile(name2)
            .OpenWrite()
            .Dispose();
        
        ((IReadOnlyStorageFolder) _subject)
            .GetFolders()
            .Should().BeEmpty();
    }
    
    [Fact]
    public void GetFolders_ReadOnly_ShouldReturnExpected_WhenFolderExists_WhenFolderContainsFolders()
    {
        const string name1 = "test1";
        const string name2 = "test2";
        
        _subject.Create();
        _subject.CreateFolder(name1);
        _subject.CreateFolder(name2);
        
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
        
        _subject.Create();
        _subject.CreateFile(name1)
            .OpenWrite()
            .Dispose();
        _subject.CreateFolder(name2);
        
        var items = ((IReadOnlyStorageFolder) _subject)
            .GetFolders()
            .ToList();
        
        items.Should().HaveCount(1)
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
        _subject.Create();
        
        _subject.GetFiles().Should().BeEmpty();
    }
    
    [Fact]
    public void GetFiles_ShouldReturnExpected_WhenFolderExists_WhenFolderContainsFiles()
    {
        const string name1 = "test1.txt";
        const string name2 = "test2.txt";
        
        _subject.Create();
        _subject.CreateFile(name1)
            .OpenWrite()
            .Dispose();
        _subject.CreateFile(name2)
            .OpenWrite()
            .Dispose();
        
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
        
        _subject.Create();
        _subject.CreateFolder(name1);
        _subject.CreateFolder(name2);
        
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
        _subject.Create();
        
        ((IReadOnlyStorageFolder) _subject)
            .GetFiles()
            .Should().BeEmpty();
    }
    
    [Fact]
    public void GetFiles_ReadOnly_ShouldReturnExpected_WhenFolderExists_WhenFolderContainsFiles()
    {
        const string name1 = "test1.txt";
        const string name2 = "test2.txt";
        
        _subject.Create();
        _subject.CreateFile(name1)
            .OpenWrite()
            .Dispose();
        _subject.CreateFile(name2)
            .OpenWrite()
            .Dispose();
        
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
        
        _subject.Create();
        _subject.CreateFolder(name1);
        _subject.CreateFolder(name2);
        
        ((IReadOnlyStorageFolder) _subject)
            .GetFiles()
            .Should().BeEmpty();
    }
}