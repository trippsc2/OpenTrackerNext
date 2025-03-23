using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Linq;
using FluentAssertions;
using OpenTrackerNext.Core.Storage.Memory;
using OpenTrackerNext.Core.Storage.Zip;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Storage.Zip;

[ExcludeFromCodeCoverage]
public sealed class ZipEntryFolderTests
{
    private readonly MemoryStorageFile _zipFile;
    
    public ZipEntryFolderTests()
    {
        var fullPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString().ToLowerInvariant());
        _zipFile = new MemoryStorageFile(fullPath);
    }

    private void CreateZipArchive()
    {
        var stream = _zipFile.OpenWrite();
        var archive = new ZipArchive(stream, ZipArchiveMode.Create);
        
        archive.Dispose();
        stream.Dispose();
    }

    private void CreateInvalidZipArchive()
    {
        var stream = _zipFile.OpenWrite();
        var writer = new StreamWriter(stream);
        
        writer.Write("This is not a zip file.");
        writer.Flush();
        
        writer.Dispose();
        stream.Dispose();
    }

    private void CreateZipFolderEntry(string entryPath)
    {
        var stream = _zipFile.OpenModify();
        var archive = new ZipArchive(stream, ZipArchiveMode.Update);
        
        archive.CreateEntry(entryPath);
        
        archive.Dispose();
        stream.Dispose();
    }
    
    private void CreateZipFileEntry(string entryPath, string content)
    {
        var stream = _zipFile.OpenModify();
        var archive = new ZipArchive(stream, ZipArchiveMode.Update);
        
        var entry = archive.CreateEntry(entryPath);
        var entryStream = entry.Open();
        
        var writer = new StreamWriter(entryStream);
        writer.Write(content);
        writer.Flush();
        
        writer.Dispose();
        entryStream.Dispose();
        archive.Dispose();
        stream.Dispose();
    }
    
    [Fact]
    public void FullPath_ShouldReturnEmptyString_WhenFolderIsRoot()
    {
        var subject = new ZipEntryFolder(_zipFile, string.Empty);
        
        subject.FullPath.Should().Be(string.Empty);
    }
    
    [Fact]
    public void FullPath_ShouldReturnExpected_WhenFolderPathIsProvidedWithoutTrailingSlash()
    {
        const string entryPathProvided = "parent/folder";
        const string expected = $"{entryPathProvided}/";
        
        var subject = new ZipEntryFolder(_zipFile, entryPathProvided);
        
        subject.FullPath.Should().Be(expected);
    }

    [Fact]
    public void FullPath_ShouldReturnExpected_WhenFolderPathIsProvidedWithTrailingSlash()
    {
        const string expected = "parent/folder/";
        
        var subject = new ZipEntryFolder(_zipFile, expected);
        
        subject.FullPath.Should().Be(expected);
    }
    
    [Fact]
    public void Name_ShouldReturnEmptyString_WhenFolderIsRoot()
    {
        var subject = new ZipEntryFolder(_zipFile, string.Empty);
        
        subject.Name.Should().Be(string.Empty);
    }
    
    [Fact]
    public void Name_ShouldReturnExpected_WhenFolderPathIsProvidedWithoutTrailingSlash()
    {
        const string expected = "folder";
        const string entryPathProvided = $"parent/{expected}";
        
        var subject = new ZipEntryFolder(_zipFile, entryPathProvided);
        
        subject.Name.Should().Be(expected);
    }
    
    [Fact]
    public void Name_ShouldReturnExpected_WhenFolderPathIsProvidedWithTrailingSlash()
    {
        const string expected = "folder";
        const string entryPathProvided = $"parent/{expected}/";
        
        var subject = new ZipEntryFolder(_zipFile, entryPathProvided);
        
        subject.Name.Should().Be(expected);
    }
    
    [Fact]
    public void Exists_ShouldReturnTrue_WhenZipFileIsValid_WhenFolderIsRoot()
    {
        CreateZipArchive();
        
        var subject = new ZipEntryFolder(_zipFile, string.Empty);
        
        subject.Exists().Should().BeTrue();
    }
    
    [Fact]
    public void Exists_ShouldReturnTrue_WhenZipFileIsValid_WhenFolderIsNotRoot_WhenFolderExists()
    {
        const string entryPath = "parent/folder/";
        
        CreateZipArchive();
        CreateZipFolderEntry(entryPath);
        
        var subject = new ZipEntryFolder(_zipFile, entryPath);
        
        subject.Exists().Should().BeTrue();
    }
    
    [Fact]
    public void Exists_ShouldReturnFalse_WhenZipFileIsValid_WhenFolderIsNotRoot_WhenFolderDoesNotExist()
    {
        CreateZipArchive();
        
        var subject = new ZipEntryFolder(_zipFile, "parent/folder/");
        
        subject.Exists().Should().BeFalse();
    }

    [Fact]
    public void Exists_ShouldThrowInvalidDataException_WhenZipFileIsInvalid()
    {
        CreateInvalidZipArchive();
        
        var subject = new ZipEntryFolder(_zipFile, string.Empty);
        
        subject.Invoking(x => x.Exists())
            .Should()
            .Throw<InvalidDataException>();
    }
    
    [Fact]
    public void Exists_ShouldReturnFalse_WhenZipFileDoesNotExist_WhenFolderIsRoot()
    {
        var subject = new ZipEntryFolder(_zipFile, string.Empty);
        
        subject.Exists().Should().BeFalse();
    }
    
    [Fact]
    public void Exists_ShouldReturnFalse_WhenZipFileDoesNotExist_WhenFolderIsNotRoot()
    {
        var subject = new ZipEntryFolder(_zipFile, "parent/folder/");
        
        subject.Exists().Should().BeFalse();
    }
    
    [Fact]
    public void GetFolder_ShouldReturnNull_WhenZipFileIsValid_WhenFolderIsRoot_WhenSubFolderDoesNotExist()
    {
        CreateZipArchive();
        
        var subject = new ZipEntryFolder(_zipFile, string.Empty);
        var newFolder = subject.GetFolder("folder");

        newFolder.Should().BeNull();
    }
    
    [Fact]
    public void GetFolder_ShouldReturnFolder_WhenZipFileIsValid_WhenFolderIsRoot_WhenSubFolderExists()
    {
        const string subfolderName = "folder";
        const string subfolderPath = $"{subfolderName}/";
        
        CreateZipArchive();
        CreateZipFolderEntry(subfolderPath);
        
        var subject = new ZipEntryFolder(_zipFile, string.Empty);
        var newFolder = subject.GetFolder(subfolderName);

        newFolder.Should().NotBeNull();
        
        newFolder.Name.Should().Be(subfolderName);
        newFolder.FullPath.Should().Be(subfolderPath);
    }
    
    [Fact]
    public void GetFolder_ShouldReturnNull_WhenZipFileIsValid_WhenFolderIsNotRoot_WhenSubFolderDoesNotExist()
    {
        const string entryPath = "parent/";
        
        CreateZipArchive();
        CreateZipFolderEntry(entryPath);
        
        var subject = new ZipEntryFolder(_zipFile, entryPath);
        var newFolder = subject.GetFolder("folder");

        newFolder.Should().BeNull();
    }
    
    [Fact]
    public void GetFolder_ShouldReturnFolder_WhenZipFileIsValid_WhenFolderIsNotRoot_WhenSubFolderExists()
    {
        const string entryPath = "parent/";
        const string subfolderName = "folder";
        const string subfolderPath = $"{entryPath}{subfolderName}/";
        
        CreateZipArchive();
        CreateZipFolderEntry(entryPath);
        CreateZipFolderEntry(subfolderPath);
        
        var subject = new ZipEntryFolder(_zipFile, entryPath);
        var newFolder = subject.GetFolder(subfolderName);

        newFolder.Should().NotBeNull();
        
        newFolder.Name.Should().Be(subfolderName);
        newFolder.FullPath.Should().Be(subfolderPath);
    }
    
    [Fact]
    public void GetFolder_ShouldThrowInvalidDataException_WhenZipFileIsInvalid_WhenFolderIsRoot()
    {
        CreateInvalidZipArchive();
        
        var subject = new ZipEntryFolder(_zipFile, string.Empty);
        
        subject.Invoking(x => x.GetFolder("folder"))
            .Should()
            .Throw<InvalidDataException>();
    }

    [Fact]
    public void GetFolder_ShouldThrowInvalidDataException_WhenZipFileIsInvalid_WhenFolderIsNotRoot()
    {
        CreateInvalidZipArchive();
        
        var subject = new ZipEntryFolder(_zipFile, "parent/");
        
        subject.Invoking(x => x.GetFolder("folder"))
            .Should()
            .Throw<InvalidDataException>();
    }
    
    [Fact]
    public void GetFolder_ShouldReturnNull_WhenZipFileDoesNotExist_WhenFolderIsRoot()
    {
        var subject = new ZipEntryFolder(_zipFile, string.Empty);
        var newFolder = subject.GetFolder("folder");

        newFolder.Should().BeNull();
        
        _zipFile.Exists().Should().BeFalse();
    }
    
    [Fact]
    public void GetFolder_ShouldReturnNull_WhenZipFileDoesNotExist_WhenFolderIsNotRoot()
    {
        var subject = new ZipEntryFolder(_zipFile, "parent/");
        var newFolder = subject.GetFolder("folder");

        newFolder.Should().BeNull();
        
        _zipFile.Exists().Should().BeFalse();
    }
    
    [Fact]
    public void GetFolder_ShouldThrowArgumentException_WhenNameIsEmpty()
    {
        var subject = new ZipEntryFolder(_zipFile, string.Empty);
        
        subject.Invoking(x => x.GetFolder("   "))
            .Should()
            .Throw<ArgumentException>();
    }
    
    [Fact]
    public void GetFile_ShouldReturnNull_WhenZipFileIsValid_WhenFolderIsRoot_WhenFileDoesNotExist()
    {
        CreateZipArchive();
        
        var subject = new ZipEntryFolder(_zipFile, string.Empty);
        var newFile = subject.GetFile("file.txt");

        newFile.Should().BeNull();
    }
    
    [Fact]
    public void GetFile_ShouldReturnFile_WhenZipFileIsValid_WhenFolderIsRoot_WhenFileExists()
    {
        const string fileName = "file.txt";
        const string content = "This is a file.";
        
        CreateZipArchive();
        CreateZipFileEntry(fileName, content);
        
        var subject = new ZipEntryFolder(_zipFile, string.Empty);
        var newFile = subject.GetFile(fileName);

        newFile.Should().NotBeNull();
        
        newFile.Name.Should().Be(fileName);
        newFile.FullPath.Should().Be(fileName);
        
        using var stream = newFile.OpenRead();
        using var reader = new StreamReader(stream);
        
        reader.ReadToEnd().Should().Be(content);
    }
    
    [Fact]
    public void GetFile_ShouldReturnNull_WhenZipFileIsValid_WhenFolderIsNotRoot_WhenFileDoesNotExist()
    {
        const string entryPath = "parent/";
        
        CreateZipArchive();
        
        var subject = new ZipEntryFolder(_zipFile, entryPath);
        var newFile = subject.GetFile("file.txt");

        newFile.Should().BeNull();
    }
    
    [Fact]
    public void GetFile_ShouldReturnFile_WhenZipFileIsValid_WhenFolderIsNotRoot_WhenFileExists()
    {
        const string entryPath = "parent/";
        const string fileName = "file.txt";
        const string filePath = $"{entryPath}{fileName}";
        const string content = "This is a file.";
        
        CreateZipArchive();
        CreateZipFileEntry(filePath, content);
        
        var subject = new ZipEntryFolder(_zipFile, entryPath);
        var newFile = subject.GetFile(fileName);

        newFile.Should().NotBeNull();
        
        newFile.Name.Should().Be(fileName);
        newFile.FullPath.Should().Be(filePath);
        
        using var stream = newFile.OpenRead();
        using var reader = new StreamReader(stream);
        
        reader.ReadToEnd().Should().Be(content);
    }
    
    [Fact]
    public void GetFile_ShouldThrowInvalidDataException_WhenZipFileIsInvalid_WhenFolderIsRoot()
    {
        CreateInvalidZipArchive();
        
        var subject = new ZipEntryFolder(_zipFile, string.Empty);
        
        subject.Invoking(x => x.GetFile("file.txt"))
            .Should()
            .Throw<InvalidDataException>();
    }
    
    [Fact]
    public void GetFile_ShouldThrowInvalidDataException_WhenZipFileIsInvalid_WhenFolderIsNotRoot()
    {
        CreateInvalidZipArchive();
        
        var subject = new ZipEntryFolder(_zipFile, "parent/");
        
        subject.Invoking(x => x.GetFile("file.txt"))
            .Should()
            .Throw<InvalidDataException>();
    }
    
    [Fact]
    public void GetFile_ShouldReturnNull_WhenZipFileDoesNotExist_WhenFolderIsRoot()
    {
        var subject = new ZipEntryFolder(_zipFile, string.Empty);
        var newFile = subject.GetFile("file.txt");

        newFile.Should().BeNull();
        
        _zipFile.Exists().Should().BeFalse();
    }
    
    [Fact]
    public void GetFile_ShouldReturnNull_WhenZipFileDoesNotExist_WhenFolderIsNotRoot()
    {
        var subject = new ZipEntryFolder(_zipFile, "parent/");
        var newFile = subject.GetFile("file.txt");

        newFile.Should().BeNull();
        
        _zipFile.Exists().Should().BeFalse();
    }
    
    [Fact]
    public void GetFile_ShouldThrowArgumentException_WhenNameIsEmpty()
    {
        var subject = new ZipEntryFolder(_zipFile, string.Empty);
        
        subject.Invoking(x => x.GetFile("   "))
            .Should()
            .Throw<ArgumentException>();
    }
    
    [Fact]
    public void GetItems_ShouldReturnEmptyList_WhenZipFileIsValid_WhenFolderIsRoot_WhenFolderIsEmpty()
    {
        CreateZipArchive();
        
        var subject = new ZipEntryFolder(_zipFile, string.Empty);
        var items = subject.GetItems();

        items.Should().BeEmpty();
    }
    
    [Fact]
    public void GetItems_ShouldReturnFoldersAndFiles_WhenZipFileIsValid_WhenFolderIsRoot_WhenFolderIsNotEmpty()
    {
        const string subfolderName = "folder";
        const string subfolderPath = $"{subfolderName}/";
        const string fileName = "file.txt";
        const string content = "This is a file.";
        
        CreateZipArchive();
        CreateZipFolderEntry(subfolderPath);
        CreateZipFileEntry(fileName, content);
        
        var subject = new ZipEntryFolder(_zipFile, string.Empty);
        var items = subject.GetItems().ToList();

        items.Should().HaveCount(2)
            .And.Contain(x => x.Name == subfolderName && x.FullPath == subfolderPath)
            .And.Contain(x => x.Name == fileName && x.FullPath == fileName);
    }
    
    [Fact]
    public void GetItems_ShouldReturnEmptyList_WhenZipFileIsValid_WhenFolderIsNotRoot_WhenFolderIsEmpty()
    {
        const string entryPath = "parent/";
        
        CreateZipArchive();
        
        var subject = new ZipEntryFolder(_zipFile, entryPath);
        var items = subject.GetItems();

        items.Should().BeEmpty();
    }
    
    [Fact]
    public void GetItems_ShouldReturnFoldersAndFiles_WhenZipFileIsValid_WhenFolderIsNotRoot_WhenFolderIsNotEmpty()
    {
        const string entryPath = "parent/";
        const string subfolderName = "folder";
        const string subfolderPath = $"{entryPath}{subfolderName}/";
        const string fileName = "file.txt";
        const string filePath = $"{entryPath}{fileName}";
        const string content = "This is a file.";
        
        CreateZipArchive();
        CreateZipFolderEntry(subfolderPath);
        CreateZipFileEntry(filePath, content);
        
        var subject = new ZipEntryFolder(_zipFile, entryPath);
        var items = subject.GetItems().ToList();

        items.Should().HaveCount(2)
            .And.Contain(x => x.Name == subfolderName && x.FullPath == subfolderPath)
            .And.Contain(x => x.Name == fileName && x.FullPath == filePath);
    }
    
    [Fact]
    public void GetItems_ShouldThrowInvalidDataException_WhenZipFileIsInvalid_WhenFolderIsRoot()
    {
        CreateInvalidZipArchive();
        
        var subject = new ZipEntryFolder(_zipFile, string.Empty);

        subject.Invoking(x => x.GetItems())
            .Should()
            .Throw<InvalidDataException>();
    }
    
    [Fact]
    public void GetItems_ShouldThrowInvalidDataException_WhenZipFileIsInvalid_WhenFolderIsNotRoot()
    {
        CreateInvalidZipArchive();
        
        var subject = new ZipEntryFolder(_zipFile, "parent/");

        subject.Invoking(x => x.GetItems())
            .Should()
            .Throw<InvalidDataException>();
    }
    
    [Fact]
    public void GetItems_ShouldReturnEmptyList_WhenZipFileDoesNotExist_WhenFolderIsRoot()
    {
        var subject = new ZipEntryFolder(_zipFile, string.Empty);
        var items = subject.GetItems();

        items.Should().BeEmpty();
    }
    
    [Fact]
    public void GetItems_ShouldReturnEmptyList_WhenZipFileDoesNotExist_WhenFolderIsNotRoot()
    {
        var subject = new ZipEntryFolder(_zipFile, "parent/");
        var items = subject.GetItems();

        items.Should().BeEmpty();
    }
    
    [Fact]
    public void GetFolders_ShouldReturnEmptyList_WhenZipFileIsValid_WhenFolderIsRoot_WhenFolderIsEmpty()
    {
        CreateZipArchive();
        
        var subject = new ZipEntryFolder(_zipFile, string.Empty);
        var items = subject.GetFolders();

        items.Should().BeEmpty();
    }
    
    [Fact]
    public void GetFolders_ShouldReturnFolders_WhenZipFileIsValid_WhenFolderIsRoot_WhenFolderIsNotEmpty()
    {
        const string subfolderName = "folder";
        const string subfolderPath = $"{subfolderName}/";
        const string fileName = "file.txt";
        const string content = "This is a file.";
        
        CreateZipArchive();
        CreateZipFolderEntry(subfolderPath);
        CreateZipFileEntry(fileName, content);
        
        var subject = new ZipEntryFolder(_zipFile, string.Empty);
        var items = subject.GetFolders().ToList();

        items.Should().ContainSingle()
            .And.Contain(x => x.Name == subfolderName && x.FullPath == subfolderPath);
    }
    
    [Fact]
    public void GetFolders_ShouldReturnEmptyList_WhenZipFileIsValid_WhenFolderIsNotRoot_WhenFolderIsEmpty()
    {
        CreateZipArchive();
        
        var subject = new ZipEntryFolder(_zipFile, "parent/");
        var items = subject.GetFolders();

        items.Should().BeEmpty();
    }
    
    [Fact]
    public void GetFolders_ShouldReturnFolders_WhenZipFileIsValid_WhenFolderIsNotRoot_WhenFolderIsNotEmpty()
    {
        const string entryPath = "parent/";
        const string subfolderName = "folder";
        const string subFolderPath = $"{entryPath}{subfolderName}/";
        const string fileName = "file.txt";
        const string content = "This is a file.";
        
        CreateZipArchive();
        CreateZipFolderEntry(subFolderPath);
        CreateZipFileEntry($"{entryPath}{fileName}", content);
        
        var subject = new ZipEntryFolder(_zipFile, entryPath);
        var items = subject.GetFolders().ToList();

        items.Should().ContainSingle()
            .And.Contain(x => x.Name == subfolderName && x.FullPath == subFolderPath);
    }
    
    [Fact]
    public void GetFolders_ShouldThrowInvalidDataException_WhenZipFileIsInvalid_WhenFolderIsRoot()
    {
        CreateInvalidZipArchive();
        
        var subject = new ZipEntryFolder(_zipFile, string.Empty);
        
        subject.Invoking(x => x.GetFolders())
            .Should()
            .Throw<InvalidDataException>();
    }
    
    [Fact]
    public void GetFolders_ShouldThrowInvalidDataException_WhenZipFileIsInvalid_WhenFolderIsNotRoot()
    {
        CreateInvalidZipArchive();
        
        var subject = new ZipEntryFolder(_zipFile, "parent/");
        
        subject.Invoking(x => x.GetFolders())
            .Should()
            .Throw<InvalidDataException>();
    }
    
    [Fact]
    public void GetFolders_ShouldReturnEmptyList_WhenZipFileDoesNotExist_WhenFolderIsRoot()
    {
        var subject = new ZipEntryFolder(_zipFile, string.Empty);
        var items = subject.GetFolders();

        items.Should().BeEmpty();
    }
    
    [Fact]
    public void GetFolders_ShouldReturnEmptyList_WhenZipFileDoesNotExist_WhenFolderIsNotRoot()
    {
        var subject = new ZipEntryFolder(_zipFile, "parent/");
        var items = subject.GetFolders();

        items.Should().BeEmpty();
    }
    
    [Fact]
    public void GetFiles_ShouldReturnEmptyList_WhenZipFileIsValid_WhenFolderIsRoot_WhenFolderIsEmpty()
    {
        CreateZipArchive();
        
        var subject = new ZipEntryFolder(_zipFile, string.Empty);
        var items = subject.GetFiles();

        items.Should().BeEmpty();
    }
    
    [Fact]
    public void GetFiles_ShouldReturnFiles_WhenZipFileIsValid_WhenFolderIsRoot_WhenFolderIsNotEmpty()
    {
        const string subfolderName = "folder";
        const string fileName = "file.txt";
        const string content = "This is a file.";
        
        CreateZipArchive();
        CreateZipFolderEntry($"{subfolderName}/");
        CreateZipFileEntry(fileName, content);
        
        var subject = new ZipEntryFolder(_zipFile, string.Empty);
        var items = subject.GetFiles().ToList();

        items.Should().ContainSingle()
            .And.Contain(x => x.Name == fileName && x.FullPath == fileName);
    }
    
    [Fact]
    public void GetFiles_ShouldReturnEmptyList_WhenZipFileIsValid_WhenFolderIsNotRoot_WhenFolderIsEmpty()
    {
        CreateZipArchive();
        
        var subject = new ZipEntryFolder(_zipFile, "parent/");
        var items = subject.GetFiles();

        items.Should().BeEmpty();
    }
    
    [Fact]
    public void GetFiles_ShouldReturnFiles_WhenZipFileIsValid_WhenFolderIsNotRoot_WhenFolderIsNotEmpty()
    {
        const string entryPath = "parent/";
        const string subfolderName = "folder";
        const string fileName = "file.txt";
        const string filePath = $"{entryPath}{fileName}";
        const string content = "This is a file.";
        
        CreateZipArchive();
        CreateZipFolderEntry($"{entryPath}{subfolderName}/");
        CreateZipFileEntry(filePath, content);
        
        var subject = new ZipEntryFolder(_zipFile, entryPath);
        var items = subject
            .GetFiles()
            .ToList();

        items.Should().ContainSingle()
            .And.Contain(x => x.Name == fileName && x.FullPath == filePath);
    }
    
    [Fact]
    public void GetFiles_ShouldThrowInvalidDataException_WhenZipFileIsInvalid_WhenFolderIsRoot()
    {
        CreateInvalidZipArchive();
        
        var subject = new ZipEntryFolder(_zipFile, string.Empty);
        
        subject.Invoking(x => x.GetFiles())
            .Should()
            .Throw<InvalidDataException>();
    }
    
    [Fact]
    public void GetFiles_ShouldThrowInvalidDataException_WhenZipFileIsInvalid_WhenFolderIsNotRoot()
    {
        CreateInvalidZipArchive();
        
        var subject = new ZipEntryFolder(_zipFile, "parent/");
        
        subject.Invoking(x => x.GetFiles())
            .Should()
            .Throw<InvalidDataException>();
    }
    
    [Fact]
    public void GetFiles_ShouldReturnEmptyList_WhenZipFileDoesNotExist_WhenFolderIsRoot()
    {
        var subject = new ZipEntryFolder(_zipFile, string.Empty);
        var items = subject.GetFiles();

        items.Should().BeEmpty();
    }
}