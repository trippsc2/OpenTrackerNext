using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using FluentAssertions.Reactive;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTrackerNext.Core.Equality;
using OpenTrackerNext.Core.Icons;
using OpenTrackerNext.Core.Json;
using OpenTrackerNext.Core.Layouts.IconGrid.Static;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Layouts.IconGrid.Static;

[ExcludeFromCodeCoverage]
public sealed class StaticIconLayoutPrototypeTests
{
    private readonly StaticIconLayoutPrototype _subject = new();

    [Fact]
    public void DocumentChanges_ShouldEmitWhenPropertyChanges()
    {
        using var observer = _subject.DocumentChanges.Observe();
        
        _subject.Icon = IconId.New();

        observer.Should().Push(1);
    }
    
    [Fact]
    public void Icon_ShouldDefaultToEmpty()
    {
        _subject.Icon
            .Should()
            .Be(IconId.Empty);
    }
    
    [Fact]
    public void Icon_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.Icon = IconId.New();
        
        monitor.Should().RaisePropertyChangeFor(x => x.Icon);
    }
    
    [Fact]
    public void Icon_ShouldSerializeAsExpected()
    {
        var icon = IconId.New();
        _subject.Icon = icon;
        
        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(StaticIconLayoutPrototype.Icon)]?
            .Value<string>()
            .Should()
            .Be(icon.Value.ToString());
    }
    
    [Fact]
    public void Icon_ShouldDeserializeAsExpected()
    {
        var icon = IconId.New();

        var jsonObject = new JObject
        {
            { nameof(StaticIconLayoutPrototype.Icon), new JValue(icon.Value.ToString()) }
        };
        
        var json = jsonObject.ToString(Formatting.Indented);

        var subject = JsonContext.Deserialize<StaticIconLayoutPrototype>(json);
        
        subject.Icon
            .Should()
            .Be(icon);
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
    public void MakeValueEqualTo_ShouldSetValueEqual_WhenOtherIsSameType()
    {
        var other = new StaticIconLayoutPrototype { Icon = IconId.New() };
        
        ((IMakeValueEqual)_subject)
            .MakeValueEqualTo(other)
            .Should()
            .BeTrue();

        _subject.Icon
            .Should()
            .Be(other.Icon);
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
    public void Clone_ShouldReturnNewEquivalentInstance()
    {
        _subject.Icon = IconId.New();
        
        var clone = ((ICloneable)_subject).Clone();
        
        clone.Should()
            .NotBeSameAs(_subject)
            .And.BeOfType<StaticIconLayoutPrototype>();

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
        _subject.Icon = IconId.New();
        
        var other = new StaticIconLayoutPrototype { Icon = _subject.Icon };
        
        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenOtherHasDifferentValues()
    {
        _subject.Icon = IconId.New();

        var other = new StaticIconLayoutPrototype { Icon = IconId.New() };
        
        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
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