using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using FluentAssertions;
using OpenTrackerNext.Core.Storage.Memory;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Storage.Memory;

[ExcludeFromCodeCoverage]
public sealed class MemoryStorageFileTests
{
    private const string TestName = "test.txt";

    private readonly string _testPath;

    private readonly MemoryStorageFile _subject;
    
    public MemoryStorageFileTests()
    {
        var tempPath = Path.GetTempPath();
        var guid = Guid.NewGuid().ToString();
        
        var directoryPath = Path.Combine(tempPath, guid);
        _testPath = Path.Combine(directoryPath, TestName);
        
        _subject = new MemoryStorageFile(_testPath);
    }

    [Fact]
    public void FullPath_ShouldEqualConstructorParameter()
    {
        _subject.FullPath
            .Should()
            .Be(_testPath);
    }
    
    [Fact]
    public void Name_ShouldEqualConstructorParameter()
    {
        _subject.Name
            .Should()
            .Be(TestName);
    }
    
    [Fact]
    public void Exists_ShouldReturnFalse_WhenFileDoesNotExist()
    {
        _subject.Exists().Should().BeFalse();
    }
    
    [Fact]
    public void Exists_ShouldReturnTrue_WhenFileExists()
    {
        _subject.OpenWrite().Dispose();
        
        _subject.Exists().Should().BeTrue();
    }
    
    [Fact]
    public void Delete_ShouldDeleteFile_WhenFileExists()
    {
        _subject.Delete();
        
        _subject.Exists().Should().BeFalse();
    }
    
    [Fact]
    public void Delete_ShouldDoNothing_WhenFileDoesNotExist()
    {
        _subject.Delete();
        
        _subject.Exists().Should().BeFalse();
    }
    
    [Fact]
    public void OpenRead_ShouldReturnExpectedStream_WhenFileExists()
    {
        const string expected = "test";

        var writeStream = _subject.OpenWrite();

        var writer = new StreamWriter(writeStream);
        
        writer.Write(expected);
        writer.Flush();
        
        writer.Dispose();
        writeStream.Dispose();
        
        using var stream = _subject.OpenRead();
        using var reader = new StreamReader(stream);
        
        reader.ReadToEnd().Should().Be(expected);
    }
    
    [Fact]
    public void OpenRead_ShouldReturnEmptyMemoryStream_WhenFileDoesNotExist()
    {
        var stream = (MemoryStream)_subject.OpenRead();
        
        stream.Length.Should().Be(0);
    }
    
    [Fact]
    public void OpenModify_ShouldReturnExpectedStream_WhenFileExists()
    {
        const string prefix = "prefix";
        const string appended = " test";
        const string expected = $"{prefix}{appended}";
        
        var existingStream = _subject.OpenModify();
        var existingWriter = new StreamWriter(existingStream);
        
        existingWriter.Write(prefix);
        existingWriter.Flush();
        
        existingWriter.Dispose();
        existingStream.Dispose();
        
        var stream = _subject.OpenModify();
        stream.Seek(0, SeekOrigin.End);
        var writer = new StreamWriter(stream);
        
        writer.Write(appended);
        writer.Flush();
        
        writer.Dispose();
        stream.Dispose();
        
        using var readStream = _subject.OpenRead();
        using var reader = new StreamReader(readStream);
        
        reader.ReadToEnd().Should().Be(expected);
    }
    
    [Fact]
    public void OpenModify_ShouldReturnExpectedStream_WhenFileDoesNotExist()
    {
        const string expected = "test";
        
        var stream = _subject.OpenWrite();
        var writer = new StreamWriter(stream);
        
        writer.Write(expected);
        writer.Flush();
        
        writer.Dispose();
        stream.Dispose();

        using var readStream = _subject.OpenRead();
        using var reader = new StreamReader(readStream);
        
        reader.ReadToEnd().Should().Be(expected);
    }
    
    [Fact]
    public void OpenWrite_ShouldReturnExpectedStream_WhenFileExists()
    {
        const string expected = "test";
        
        var existingStream = _subject.OpenWrite();
        var existingWriter = new StreamWriter(existingStream);
        
        existingWriter.Write("overwritten");
        existingWriter.Flush();
        
        existingWriter.Dispose();
        existingStream.Dispose();
        
        var stream = _subject.OpenWrite();
        var writer = new StreamWriter(stream);
        
        writer.Write(expected);
        writer.Flush();
        
        writer.Dispose();
        stream.Dispose();
        
        using var readStream = _subject.OpenRead();
        using var reader = new StreamReader(readStream);
        
        reader.ReadToEnd().Should().Be(expected);
    }
    
    [Fact]
    public void OpenWrite_ShouldReturnExpectedStream_WhenFileDoesNotExist()
    {
        const string expected = "test";
        
        var stream = _subject.OpenWrite();
        var writer = new StreamWriter(stream);
        
        writer.Write(expected);
        writer.Flush();
        
        writer.Dispose();
        stream.Dispose();

        using var readStream = _subject.OpenRead();
        using var reader = new StreamReader(readStream);
        
        reader.ReadToEnd().Should().Be(expected);
    }
}