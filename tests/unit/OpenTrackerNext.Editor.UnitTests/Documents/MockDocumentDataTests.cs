using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using FluentAssertions.Reactive;
using OpenTrackerNext.Core.Equality;
using Xunit;

namespace OpenTrackerNext.Editor.UnitTests.Documents;

[ExcludeFromCodeCoverage]
public sealed class MockDocumentDataTests
{
    private readonly MockDocumentData _subject = new();

    [Fact]
    public void TitlePrefix_ShouldReturnExpected()
    {
        MockDocumentData.TitlePrefix
            .Should()
            .Be(MockDocumentData.MockTitlePrefix);
    }

    [Fact]
    public void Value_ShouldInitializeToDefault()
    {
        _subject.Value
            .Should()
            .Be(0);
    }

    [Fact]
    public void Value_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.Value = 1;
        
        monitor.Should().RaisePropertyChangeFor(x => x.Value);
    }

    [Fact]
    public void Changes_ShouldEmitWhenValueChanges()
    {
        using var observer = _subject.DocumentChanges.Observe();
        
        _subject.Value = 1;

        observer.Should().Push(1, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void MakeValueEqualTo_ShouldDoNothing_WhenOtherIsReferenceEqual()
    {
        ((IMakeValueEqual)_subject)
            .MakeValueEqualTo(_subject)
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void MakeValueEqualTo_ShouldSetExpected()
    {
        const int expected = 1;
        
        var other = new MockDocumentData { Value = expected };

        ((IMakeValueEqual)_subject)
            .MakeValueEqualTo(other)
            .Should()
            .BeTrue();
        
        _subject.Value
            .Should()
            .Be(expected);
    }
    
    [Fact]
    public void MakeValueEqualTo_ShouldReturnFalse_WhenOtherIsNotMockDocumentData()
    {
        ((IMakeValueEqual)_subject)
            .MakeValueEqualTo(new object())
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void Clone_ShouldReturnNewEquivalentInstance()
    {
        const int expected = 1;
        
        _subject.Value = expected;
        
        var clone = ((ICloneable)_subject).Clone();

        clone.Should()
            .BeOfType<MockDocumentData>()
            .And.NotBeSameAs(_subject);

        ((IValueEquatable)_subject)
            .ValueEquals(clone)
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnTrue_WhenOtherIsReferenceEqual()
    {
        ((IValueEquatable)_subject)
            .ValueEquals(_subject)
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnTrue_WhenOtherIsValueEqual()
    {
        const int expected = 1;
        
        _subject.Value = expected;
        
        var other = new MockDocumentData { Value = expected };

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenOtherIsNotValueEqual()
    {
        _subject.Value = 1;
        
        var other = new MockDocumentData { Value = 2 };

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenOtherIsNotMockDocumentData()
    {
        ((IValueEquatable)_subject)
            .ValueEquals(new object())
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenOtherIsNull()
    {
        ((IValueEquatable)_subject)
            .ValueEquals(null)
            .Should()
            .BeFalse();
    }
}