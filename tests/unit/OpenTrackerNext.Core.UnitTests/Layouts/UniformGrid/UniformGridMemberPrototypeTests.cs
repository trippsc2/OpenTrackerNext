using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using FluentAssertions.Reactive;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTrackerNext.Core.Equality;
using OpenTrackerNext.Core.Json;
using OpenTrackerNext.Core.Layouts;
using OpenTrackerNext.Core.Layouts.UniformGrid;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Layouts.UniformGrid;

[ExcludeFromCodeCoverage]
public sealed class UniformGridMemberPrototypeTests
{
    private readonly UniformGridMemberPrototype _subject = new();

    [Fact]
    public void DocumentChanges_ShouldEmitWhenLayoutChanges()
    {
        using var observer = _subject.DocumentChanges.Observe();
        
        _subject.Layout = LayoutId.New();

        observer.Should().Push(1);
    }

    [Fact]
    public void Layout_ShouldDefaultToEmptyLayoutId()
    {
        _subject.Layout
            .Should()
            .Be(LayoutId.Empty);
    }
    
    [Fact]
    public void Layout_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.Layout = LayoutId.New();
        
        monitor.Should().RaisePropertyChangeFor(x => x.Layout);
    }

    [Fact]
    public void Layout_ShouldSerializeAsExpected()
    {
        _subject.Layout = LayoutId.New();

        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);
        
        jsonObject[nameof(UniformGridMemberPrototype.Layout)]?
            .Value<string>()
            .Should()
            .Be(_subject.Layout.ToString());
    }
    
    [Fact]
    public void Layout_ShouldDeserializeAsExpected()
    {
        var layout = LayoutId.New();

        var jsonObject = new JObject
        {
            { nameof(UniformGridMemberPrototype.Layout), new JValue(layout.ToString()) }
        };

        var json = jsonObject.ToString(Formatting.Indented);

        var subject = JsonContext.Deserialize<UniformGridMemberPrototype>(json);
        
        subject.Layout
            .Should()
            .Be(layout);
    }

    [Fact]
    public void MakeValueEqualTo_ShouldReturnTrueAndDoNothing_WhenOtherIsReferenceEqual()
    {
        ((IMakeValueEqual)_subject)
            .MakeValueEqualTo(_subject)
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void MakeValueEqualTo_ShouldMakePropertiesValueEqual_WhenOtherIsSameType()
    {
        var layout = LayoutId.New();
        
        var other = new UniformGridMemberPrototype { Layout = layout };

        ((IMakeValueEqual)_subject)
            .MakeValueEqualTo(other)
            .Should()
            .BeTrue();

        _subject.Layout
            .Should()
            .Be(layout);
    }
    
    [Fact]
    public void MakeValueEqualTo_ShouldReturnFalse_WhenOtherIsNotSameType()
    {
        ((IMakeValueEqual)_subject)
            .MakeValueEqualTo(new object())
            .Should()
            .BeFalse();
    }

    [Fact]
    public void Clone_ShouldReturnNewEquivalentInstance()
    {
        var clone = ((ICloneable)_subject).Clone();
        
        clone.Should()
            .BeOfType<UniformGridMemberPrototype>()
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
        var layout = LayoutId.New();
        
        _subject.Layout = layout;
        
        var other = new UniformGridMemberPrototype { Layout = layout };

        ((IValueEquatable)_subject)
            .ValueEquals(other)
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
            .ValueEquals(null!)
            .Should()
            .BeFalse();
    }
}