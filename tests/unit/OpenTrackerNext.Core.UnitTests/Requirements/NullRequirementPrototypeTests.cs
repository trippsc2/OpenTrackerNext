using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using FluentAssertions.Reactive;
using OpenTrackerNext.Core.Equality;
using OpenTrackerNext.Core.Requirements;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Requirements;

[ExcludeFromCodeCoverage]
public sealed class NullRequirementPrototypeTests
{
    private readonly NullRequirementPrototype _subject = new();

    [Fact]
    public void DocumentChanges_ShouldNotEmit()
    {
        using var observer = _subject.DocumentChanges.Observe();

        observer.Should().NotPush();
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
    public void MakeValueEqualTo_ShouldDoNothing_WhenOtherIsSameType()
    {
        ((IMakeValueEqual)_subject)
            .MakeValueEqualTo(new NullRequirementPrototype())
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void MakeValueEqualTo_ShouldDoNothing_WhenOtherIsDifferentType()
    {
        ((IMakeValueEqual)_subject)
            .MakeValueEqualTo(new object())
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void Clone_ShouldReturnNewInstance()
    {
        var clone = ((ICloneable)_subject).Clone();
        
        clone.Should()
            .NotBeSameAs(_subject)
            .And.BeOfType<NullRequirementPrototype>();

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
    public void ValueEquals_ShouldReturnTrue_WhenOtherIsSameType()
    {
        ((IValueEquatable)_subject)
            .ValueEquals(new NullRequirementPrototype())
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenOtherIsDifferentType()
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