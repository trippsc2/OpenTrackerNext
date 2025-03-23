using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using FluentAssertions;
using OpenTrackerNext.Core.Storage.Memory;
using OpenTrackerNext.Core.Storage.Zip;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Storage.Zip;

[ExcludeFromCodeCoverage]
public sealed class ZipEntryFileTests
{
    private readonly MemoryStorageFile _zipFile;

    public ZipEntryFileTests()
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
    public void FullPath_ShouldEqualConstructorParameter_WhenFileInRootFolder()
    {
        const string entryPath = "test.txt";

        var subject = new ZipEntryFile(_zipFile, entryPath);
        
        subject.FullPath.Should().Be(entryPath);
    }
    
    [Fact]
    public void FullPath_ShouldEqualConstructorParameter_WhenFileInSubFolder()
    {
        const string entryPath = "test/test.txt";

        var subject = new ZipEntryFile(_zipFile, entryPath);
        
        subject.FullPath.Should().Be(entryPath);
    }
    
    [Fact]
    public void Name_ShouldReturnConstructorParameter_WhenFileInRootFolder()
    {
        const string entryPath = "test.txt";

        var subject = new ZipEntryFile(_zipFile, entryPath);
        
        subject.Name.Should().Be(entryPath);
    }
    
    [Fact]
    public void Name_ShouldReturnExpected_WhenFileInSubFolder()
    {
        const string name = "test.txt";
        const string entryPath = $"test/{name}";

        var subject = new ZipEntryFile(_zipFile, entryPath);
        
        subject.Name.Should().Be(name);
    }
    
    [Fact]
    public void Exists_ShouldReturnTrue_WhenZipFileIsValid_WhenFileEntryExists()
    {
        const string entryPath = "test.txt";
        
        CreateZipArchive();
        CreateZipFileEntry(entryPath, "This is a file.");

        var subject = new ZipEntryFile(_zipFile, entryPath);
        
        subject.Exists().Should().BeTrue();
    }
    
    [Fact]
    public void Exists_ShouldReturnFalse_WhenZipFileIsValid_WhenFileEntryDoesNotExist()
    {
        const string entryPath = "test.txt";
        
        CreateZipArchive();

        var subject = new ZipEntryFile(_zipFile, entryPath);
        
        subject.Exists().Should().BeFalse();
    }
    
    [Fact]
    public void Exists_ShouldThrowInvalidDataException_WhenZipFileIsInvalid()
    {
        CreateInvalidZipArchive();

        var subject = new ZipEntryFile(_zipFile, "test.txt");
        
        subject.Invoking(x => x.Exists())
            .Should()
            .Throw<InvalidDataException>();
    }
    
    [Fact]
    public void Exists_ShouldReturnFalse_WhenZipFileDoesNotExist()
    {
        const string entryPath = "test.txt";

        var subject = new ZipEntryFile(_zipFile, entryPath);
        
        subject.Exists().Should().BeFalse();
    }
    
    [Fact]
    public void OpenRead_ShouldReturnExpectedStream_WhenZipFileIsValid_WhenFileEntryExists()
    {
        const string entryPath = "test.txt";
        const string content = "This is a file.";
        
        CreateZipArchive();
        CreateZipFileEntry(entryPath, content);

        var subject = new ZipEntryFile(_zipFile, entryPath);
        
        using var stream = subject.OpenRead();
        using var reader = new StreamReader(stream);
        
        reader.ReadToEnd().Should().Be(content);
    }
    
    [Fact]
    public void OpenRead_ShouldThrowFileNotFoundException_WhenZipFileIsValid_WhenFileEntryDoesNotExist()
    {
        const string entryPath = "test.txt";
        
        CreateZipArchive();

        var subject = new ZipEntryFile(_zipFile, entryPath);
        
        subject.Invoking(x => x.OpenRead())
            .Should()
            .Throw<FileNotFoundException>();
    }
    
    [Fact]
    public void OpenRead_ShouldThrowInvalidDataException_WhenZipFileIsInvalid()
    {
        CreateInvalidZipArchive();

        var subject = new ZipEntryFile(_zipFile, "test.txt");
        
        subject.Invoking(x => x.OpenRead())
            .Should()
            .Throw<InvalidDataException>();
    }
    
    [Fact]
    public void OpenRead_ShouldThrowFileNotFoundException_WhenZipFileDoesNotExist()
    {
        const string entryPath = "test.txt";

        var subject = new ZipEntryFile(_zipFile, entryPath);
        
        subject.Invoking(x => x.OpenRead())
            .Should()
            .Throw<FileNotFoundException>();
    }
}