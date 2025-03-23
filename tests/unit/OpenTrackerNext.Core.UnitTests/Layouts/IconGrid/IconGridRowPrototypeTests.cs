using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using FluentAssertions.Reactive;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTrackerNext.Core.Equality;
using OpenTrackerNext.Core.Icons;
using OpenTrackerNext.Core.Json;
using OpenTrackerNext.Core.Layouts.IconGrid;
using OpenTrackerNext.Core.Layouts.IconGrid.Static;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Layouts.IconGrid;

[ExcludeFromCodeCoverage]
public sealed class IconGridRowPrototypeTests
{
    private readonly IconGridRowPrototype _subject = new();
    
    [Fact]
    public void DocumentChanges_ShouldEmitWhenAnyPropertyChanges()
    {
        using var observer = _subject.DocumentChanges.Observe();

        var iconLayout = new IconLayoutPrototype();
        _subject.Icons.Add(iconLayout);
        iconLayout.Content = new StaticIconLayoutPrototype();
        _subject.Icons.RemoveAt(0);
        
        observer.Should().Push(3);
    }

    [Fact]
    public void Icons_ShouldDefaultToEmpty()
    {
        _subject.Icons
            .Should()
            .BeEmpty();
    }
    
    [Fact]
    public void Icons_ShouldSerializeAsExpected()
    {
        _subject.Icons.Add(new IconLayoutPrototype());
        _subject.Icons.Add(new IconLayoutPrototype());
        
        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(IconGridRowPrototype.Icons)]?
            .Value<JArray>()
            .Should()
            .HaveCount(2);
    }
    
    [Fact]
    public void Icons_ShouldDeserializeAsExpected()
    {
        var jsonObject = new JObject
        {
            {
                nameof(IconGridRowPrototype.Icons), new JArray
                {
                    new JObject(),
                    new JObject()
                }
            }
        };
        
        var json = jsonObject.ToString(Formatting.Indented);

        var subject = JsonContext.Deserialize<IconGridRowPrototype>(json);

        subject.Icons
            .Should()
            .HaveCount(2);
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
    public void MakeValueEqualTo_ShouldMakePropertiesValueEqual_WhenOtherValueIsSameType()
    {
        var other = new IconGridRowPrototype();

        var iconLayout = new IconLayoutPrototype
        {
            Content = new StaticIconLayoutPrototype { Icon = IconId.New() }
        };
        
        other.Icons.Add(iconLayout);
        
        ((IMakeValueEqual)_subject)
            .MakeValueEqualTo(other)
            .Should()
            .BeTrue();

        _subject.Icons
            .Should()
            .BeEquivalentTo(other.Icons);
    }
    
    [Fact]
    public void MakeValueEqualTo_ShouldReturnFalseAndDoNothing_WhenOtherIsDifferentType()
    {
        ((IMakeValueEqual)_subject)
            .MakeValueEqualTo(new object())
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void Clone_ShouldReturnNewEquivalentInstance()
    {
        var iconLayout = new IconLayoutPrototype
        {
            Content = new StaticIconLayoutPrototype { Icon = IconId.New() }
        };
        
        _subject.Icons.Add(iconLayout);
        
        var clone = ((ICloneable) _subject).Clone();

        clone.Should()
            .BeOfType<IconGridRowPrototype>()
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
    public void ValueEquals_ShouldReturnTrue_WhenPropertiesAreValueEqual()
    {
        var icon1 = IconId.New();
        var icon2 = IconId.New();
        
        var iconLayout1 = new IconLayoutPrototype
        {
            Content = new StaticIconLayoutPrototype { Icon = icon1 }
        };
        
        var iconLayout2 = new IconLayoutPrototype
        {
            Content = new StaticIconLayoutPrototype { Icon = icon2 }
        };
        
        _subject.Icons.Add(iconLayout1);
        _subject.Icons.Add(iconLayout2);
        
        var otherIconLayout1 = new IconLayoutPrototype
        {
            Content = new StaticIconLayoutPrototype { Icon = icon1 }
        };
        
        var otherIconLayout2 = new IconLayoutPrototype
        {
            Content = new StaticIconLayoutPrototype { Icon = icon2 }
        };
        
        var other = new IconGridRowPrototype
        {
            Icons =
            [
                otherIconLayout1,
                otherIconLayout2
            ]
        };

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenHasFewerIcons()
    {
        var icon = IconId.New();
        
        var iconLayout = new IconLayoutPrototype
        {
            Content = new StaticIconLayoutPrototype { Icon = icon }
        };
        
        _subject.Icons.Add(iconLayout);
        
        var otherIconLayout = new IconLayoutPrototype
        {
            Content = new StaticIconLayoutPrototype { Icon = icon }
        };
        
        var other = new IconGridRowPrototype
        {
            Icons =
            [
                otherIconLayout,
                new IconLayoutPrototype()
            ]
        };

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }
        
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenHasMoreIcons()
    {
        var icon1 = IconId.New();
        var icon2 = IconId.New();
        
        var iconLayout1 = new IconLayoutPrototype
        {
            Content = new StaticIconLayoutPrototype { Icon = icon1 }
        };
        
        var iconLayout2 = new IconLayoutPrototype
        {
            Content = new StaticIconLayoutPrototype { Icon = icon2 }
        };
        
        var iconLayout3 = new IconLayoutPrototype
        {
            Content = new StaticIconLayoutPrototype { Icon = IconId.New() }
        };
        
        _subject.Icons.Add(iconLayout1);
        _subject.Icons.Add(iconLayout2);
        _subject.Icons.Add(iconLayout3);
        
        var otherIconLayout1 = new IconLayoutPrototype
        {
            Content = new StaticIconLayoutPrototype { Icon = icon1 }
        };
        
        var otherIconLayout2 = new IconLayoutPrototype
        {
            Content = new StaticIconLayoutPrototype { Icon = icon2 }
        };
        
        var other = new IconGridRowPrototype
        {
            Icons =
            [
                otherIconLayout1,
                otherIconLayout2
            ]
        };

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }
        
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenIconsValuesAreDifferent()
    {
        var iconLayout1 = new IconLayoutPrototype
        {
            Content = new StaticIconLayoutPrototype { Icon = IconId.New() }
        };
        
        var iconLayout2 = new IconLayoutPrototype
        {
            Content = new StaticIconLayoutPrototype { Icon = IconId.New() }
        };
        
        _subject.Icons.Add(iconLayout1);
        _subject.Icons.Add(iconLayout2);
        
        var otherIconLayout1 = new IconLayoutPrototype
        {
            Content = new StaticIconLayoutPrototype { Icon = IconId.New() }
        };
        
        var otherIconLayout2 = new IconLayoutPrototype
        {
            Content = new StaticIconLayoutPrototype { Icon = IconId.New() }
        };
        
        var other = new IconGridRowPrototype
        {
            Icons =
            [
                otherIconLayout1,
                otherIconLayout2
            ]
        };

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