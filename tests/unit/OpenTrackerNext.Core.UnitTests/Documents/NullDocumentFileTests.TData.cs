using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using OpenTrackerNext.Core.Documents;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Documents;

[ExcludeFromCodeCoverage]
public abstract class NullDocumentFileTests<TData> : NullDocumentFileTests
    where TData : class, ITitledDocumentData<TData>, new()
{
    private readonly NullDocumentFile<TData> _subject;
    
    // ReSharper disable once ConvertToPrimaryConstructor
    protected NullDocumentFileTests(NullDocumentFile<TData> subject)
        : base(subject)
    {
        _subject = subject;
    }
    
    [Fact]
    public void SavedData_ShouldThrowNotSupportedException()
    {
        _subject.Invoking(x => x.SavedData)
            .Should()
            .Throw<NotSupportedException>();
    }
    
    [Fact]
    public void WorkingData_ShouldThrowNotSupportedException()
    {
        _subject.Invoking(x => x.WorkingData)
            .Should()
            .Throw<NotSupportedException>();
    }
}