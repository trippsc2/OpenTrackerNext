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
using OpenTrackerNext.Core.Layouts.IconGrid.Dynamic;
using OpenTrackerNext.Core.Layouts.IconGrid.Static;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Layouts.IconGrid;

[ExcludeFromCodeCoverage]
public sealed class IconLayoutPrototypeTests
{
    private readonly IconLayoutPrototype _subject = new();

    [Fact]
    public void DocumentChanges_ShouldEmitWhenPropertiesChange()
    {
        using var observer = _subject.DocumentChanges.Observe();

        var content = new DynamicIconLayoutPrototype();
        _subject.Content = content;
        content.MetIcon = IconId.New();
        content.UnmetIcon = IconId.New();
        
        observer.Should().Push(3);
    }
    
    [Fact]
    public void Content_ShouldDefaultToStaticIconLayoutPrototype()
    {
        _subject.Content
            .Should()
            .BeOfType<StaticIconLayoutPrototype>();
    }
    
    [Fact]
    public void Content_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.Content = new DynamicIconLayoutPrototype();
        
        monitor.Should().RaisePropertyChangeFor(x => x.Content);
    }

    [Theory]
    [InlineData("static", typeof(StaticIconLayoutPrototype))]
    [InlineData("dynamic", typeof(DynamicIconLayoutPrototype))]
    public void Content_ShouldSerializeAsExpected(string expected, Type type)
    {
        var content = (IIconLayoutSubtypePrototype)Activator.CreateInstance(type)!;
        _subject.Content = content;

        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);
        
        jsonObject[nameof(IconLayoutPrototype.Content)]?
            .Value<JObject>()?["$type"]?
            .Value<string>()
            .Should()
            .Be(expected);
    }
    
    [Theory]
    [InlineData(typeof(StaticIconLayoutPrototype), "static")]
    [InlineData(typeof(DynamicIconLayoutPrototype), "dynamic")]
    public void Content_ShouldDeserializeAsExpected(Type expected, string typeDiscriminator)
    {
        var jsonObject = new JObject
        {
            {
                nameof(IconLayoutPrototype.Content), new JObject
                {
                    { "$type", new JValue(typeDiscriminator) }
                }
            }
        };
        
        var json = jsonObject.ToString(Formatting.Indented);

        var subject = JsonContext.Deserialize<IconLayoutPrototype>(json);
        
        subject.Content
            .Should()
            .BeOfType(expected);
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
    public void MakeValueEqualTo_ShouldSetValuesEqual_WhenContentIsSameType()
    {
        _subject.Content = new StaticIconLayoutPrototype();
        
        var icon = IconId.New();
        
        var other = new IconLayoutPrototype
        {
            Content = new StaticIconLayoutPrototype { Icon = icon }
        };
        
        ((IMakeValueEqual)_subject)
            .MakeValueEqualTo(other)
            .Should()
            .BeTrue();

        _subject.Content
            .Should()
            .BeOfType<StaticIconLayoutPrototype>()
            .Subject
            .Icon
            .Should()
            .Be(icon);
    }
    
    [Fact]
    public void MakeValueEqualTo_ShouldSetValuesEqual_WhenContentIsDifferentType()
    {
        _subject.Content = new DynamicIconLayoutPrototype();
        
        var icon = IconId.New();
        
        var other = new IconLayoutPrototype
        {
            Content = new StaticIconLayoutPrototype { Icon = icon }
        };
        
        ((IMakeValueEqual)_subject)
            .MakeValueEqualTo(other)
            .Should()
            .BeTrue();

        _subject.Content
            .Should()
            .BeOfType<StaticIconLayoutPrototype>()
            .Subject
            .Icon
            .Should()
            .Be(icon);
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
        var icon = IconId.New();
        
        _subject.Content = new StaticIconLayoutPrototype { Icon = icon };
        
        var clone = ((ICloneable)_subject).Clone();
        
        clone.Should()
            .NotBeSameAs(_subject)
            .And.BeOfType<IconLayoutPrototype>();

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
        var icon = IconId.New();
        
        _subject.Content = new StaticIconLayoutPrototype { Icon = icon };
        
        var other = new IconLayoutPrototype
        {
            Content = new StaticIconLayoutPrototype { Icon = icon }
        };
        
        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenContentIsDifferentType()
    {
        _subject.Content = new StaticIconLayoutPrototype();
        
        var other = new IconLayoutPrototype { Content = new DynamicIconLayoutPrototype() };
        
        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenContentHasDifferentValues()
    {
        _subject.Content = new StaticIconLayoutPrototype { Icon = IconId.New() };
        
        var other = new IconLayoutPrototype
        {
            Content = new StaticIconLayoutPrototype { Icon = IconId.New() }
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