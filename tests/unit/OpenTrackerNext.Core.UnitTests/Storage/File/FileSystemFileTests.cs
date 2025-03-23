using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using FluentAssertions;
using OpenTrackerNext.Core.Storage;
using OpenTrackerNext.Core.Storage.File;
using Splat;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Storage.File;

[ExcludeFromCodeCoverage]
public sealed class FileSystemFileTests
{
    private const string TestName = "test.txt";

    private readonly string _testPath;

    private readonly FileSystemFile _subject;

    public FileSystemFileTests()
    {
        var tempPath = Path.GetTempPath();
        var guid = Guid.NewGuid().ToString();
        
        var directoryPath = Path.Combine(tempPath, guid);
        _testPath = Path.Combine(directoryPath, TestName);
        
        Directory.CreateDirectory(directoryPath);
        
        _subject = new FileSystemFile(_testPath);
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
        _subject.Exists()
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void Exists_ShouldReturnTrue_WhenFileExists()
    {
        new FileInfo(_testPath).Create().Dispose();
        
        _subject.Exists().Should().BeTrue();
    }
    
    [Fact]
    public void Delete_ShouldDeleteFile_WhenFileExists()
    {
        new FileInfo(_testPath).Create().Dispose();
        
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
        
        new FileInfo(_testPath).Create().Dispose();
        
        System.IO.File.WriteAllText(_testPath, expected);

        using var stream = _subject.OpenRead();
        using var reader = new StreamReader(stream);
        
        reader.ReadToEnd().Should().Be(expected);
    }
    
    [Fact]
    public void OpenRead_ShouldThrowFileNotFoundException_WhenFileDoesNotExist()
    {
        _subject.Invoking(x => x.OpenRead())
            .Should()
            .Throw<FileNotFoundException>();
    }
    
    [Fact]
    public void OpenModify_ShouldReturnExpectedStream_WhenFileExists()
    {
        const string prefix = "prefix";
        const string appended = " test";
        const string expected = $"{prefix}{appended}";
        
        new FileInfo(_testPath).Create().Dispose();
        
        System.IO.File.WriteAllText(_testPath, prefix);
        
        var stream = _subject.OpenModify();
        stream.Seek(0, SeekOrigin.End);
        var writer = new StreamWriter(stream);
        
        writer.Write(appended);
        writer.Flush();
        
        writer.Dispose();
        stream.Dispose();
        
        System.IO.File
            .ReadAllText(_testPath)
            .Should().Be(expected);
    }
    
    [Fact]
    public void OpenModify_ShouldReturnExpectedStream_WhenFileDoesNotExist()
    {
        const string expected = "test";
        
        var stream = _subject.OpenModify();
        var writer = new StreamWriter(stream);
        
        writer.Write(expected);
        writer.Flush();
        
        writer.Dispose();
        stream.Dispose();
        
        System.IO.File
            .ReadAllText(_testPath)
            .Should().Be(expected);
    }
    
    [Fact]
    public void OpenWrite_ShouldReturnExpectedStream_WhenFileExists()
    {
        const string expected = "test";
        
        new FileInfo(_testPath).Create().Dispose();
        
        System.IO.File.WriteAllText(_testPath, "overwritten");
        
        var stream = _subject.OpenWrite();
        var writer = new StreamWriter(stream);
        
        writer.Write(expected);
        writer.Flush();
        
        writer.Dispose();
        stream.Dispose();
        
        System.IO.File
            .ReadAllText(_testPath)
            .Should().Be(expected);
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
        
        System.IO.File
            .ReadAllText(_testPath)
            .Should().Be(expected);
    }
    
    [Fact]
    public void SplatResolve_ShouldResolveToFactory()
    {
        var factory = Locator.Current.GetService<IStorageFile.Factory>()!;
        
        var subject1 = factory(_testPath);
        var subject2 = factory(_testPath);

        subject1.Should().BeOfType<FileSystemFile>()
            .And
            .NotBeSameAs(subject2);
    }
}