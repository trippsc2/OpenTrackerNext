using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Ids;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Documents;

[ExcludeFromCodeCoverage]
public abstract class NullDocumentFileTests<TData, TId> : NullDocumentFileTests<TData>
    where TData : class, ITitledDocumentData<TData>, new()
    where TId : struct, IGuidId<TId>
{
    private readonly NullDocumentFile<TData, TId> _subject;
    
    // ReSharper disable once ConvertToPrimaryConstructor
    protected NullDocumentFileTests(NullDocumentFile<TData, TId> subject)
        : base(subject)
    {
        _subject = subject;
    }

    [Fact]
    public void Id_ShouldReturnExpected()
    {
        _subject.Id
            .Should()
            .Be(default(TId));
    }
    
    [Fact]
    public void RenameAsync_ShouldThrowNotSupportedException()
    {
        _subject.Invoking(x => x.RenameAsync("New Name"))
            .Should()
            .ThrowAsync<NotSupportedException>();
    }
}