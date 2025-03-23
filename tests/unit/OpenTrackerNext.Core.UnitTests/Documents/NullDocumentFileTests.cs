using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using OpenTrackerNext.Core.Documents;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Documents;

[ExcludeFromCodeCoverage]
public abstract class NullDocumentFileTests
{
    private readonly NullDocumentFile _subject;
    
    // ReSharper disable once ConvertToPrimaryConstructor
    protected NullDocumentFileTests(NullDocumentFile subject)
    {
        _subject = subject;
    }
    
    [Fact]
    public void TitlePrefix_ShouldThrowNotSupportedException()
    {
        _subject.Invoking(x => x.TitlePrefix)
            .Should()
            .Throw<NotSupportedException>();
    }

    [Fact]
    public void FriendlyId_ShouldReturnExpected()
    {
        _subject.FriendlyId
            .Should()
            .Be(NullDocumentFile.FriendlyIdConstant);
    }
    
    [Fact]
    public void OpenedInDocuments_ShouldThrowNotSupportedException()
    {
        _subject.Invoking(x => x.OpenedInDocuments)
            .Should()
            .Throw<NotSupportedException>();
    }
    
    [Fact]
    public void OpenedInDocuments_ShouldThrowNotSupportedException_WhenSet()
    {
        _subject.Invoking(x => x.OpenedInDocuments = 1)
            .Should()
            .Throw<NotSupportedException>();
    }
    
    [Fact]
    public void Dispose_ShouldNotThrow()
    {
        _subject.Invoking(x => x.Dispose())
            .Should()
            .NotThrow();
    }
    
    [Fact]
    public void LoadDataFromFileAsync_ShouldThrowNotSupportedException()
    {
        _subject.Invoking(x => x.LoadDataFromFileAsync())
            .Should()
            .ThrowAsync<NotSupportedException>();
    }
    
    [Fact]
    public void Revert_ShouldThrowNotSupportedException()
    {
        _subject.Invoking(x => x.Revert())
            .Should()
            .Throw<NotSupportedException>();
    }
    
    [Fact]
    public void SaveAsync_ShouldThrowNotSupportedException()
    {
        _subject.Invoking(x => x.SaveAsync())
            .Should()
            .ThrowAsync<NotSupportedException>();
    }
    
    [Fact]
    public void Delete_ShouldThrowNotSupportedException()
    {
        _subject.Invoking(x => x.Delete())
            .Should()
            .Throw<NotSupportedException>();
    }
}