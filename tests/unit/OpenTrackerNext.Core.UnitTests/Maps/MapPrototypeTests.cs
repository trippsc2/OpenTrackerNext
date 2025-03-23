using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using FluentAssertions.Reactive;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTrackerNext.Core.Equality;
using OpenTrackerNext.Core.Images;
using OpenTrackerNext.Core.Json;
using OpenTrackerNext.Core.Maps;
using OpenTrackerNext.Core.Maps.Static;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Maps;

[ExcludeFromCodeCoverage]
public sealed class MapPrototypeTests
{
    private readonly MapPrototype _subject = new();
    
    [Fact]
    public void TitlePrefix_ShouldReturnExpected()
    {
        MapPrototype.TitlePrefix
            .Should()
            .Be(MapPrototype.MapTitlePrefix);
    }
    
    [Fact]
    public void DocumentChanges_ShouldEmitWhenAnyPropertyChanges()
    {
        using var observer = _subject.DocumentChanges.Observe();

        var staticMap = new StaticMapPrototype();
        _subject.Content = staticMap;
        staticMap.Image = ImageId.New();
        
        observer.Should().Push(2);
    }
    
    [Fact]
    public void Content_ShouldDefaultToStaticMapPrototype()
    {
        _subject.Content
            .Should()
            .BeOfType<StaticMapPrototype>();
    }
    
    [Fact]
    public void Content_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();

        _subject.Content = new StaticMapPrototype();
        
        monitor.Should().RaisePropertyChangeFor(x => x.Content);
    }
    
    [Theory]
    [InlineData("static", typeof(StaticMapPrototype))]
    public void Content_ShouldSerializeAsExpected(string expected, Type type)
    {
        var content = (IMapSubtypePrototype)Activator.CreateInstance(type)!;
        
        _subject.Content = content;

        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);
        
        jsonObject[nameof(MapPrototype.Content)]?
            .Value<JObject>()?["$type"]?
            .Value<string>()
            .Should()
            .Be(expected);
    }

    [Theory]
    [InlineData(typeof(StaticMapPrototype), "static")]
    public void Content_ShouldDeserializeAsExpected(Type expected, string typeDiscriminator)
    {
        var jsonObject = new JObject
        {
            {
                nameof(MapPrototype.Content), new JObject
                {
                    { "$type", typeDiscriminator }
                }
            }
        };
        
        var json = jsonObject.ToString(Formatting.Indented);
        
        var subject = JsonContext.Deserialize<MapPrototype>(json);
        
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
    public void MakeValueEqualTo_ShouldReturnTrueAndDoNothing_WhenOtherIsReferenceEqual()
    {
        ((IMakeValueEqual)_subject)
            .MakeValueEqualTo(_subject)
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void MakeValueEqualTo_ShouldSetPropertyValueEqual_WhenOtherIsSameType()
    {
        var imageId = ImageId.New();
        
        _subject.Content = new StaticMapPrototype { Image = ImageId.New() };
        
        var other = new MapPrototype
        {
            Content = new StaticMapPrototype { Image = imageId }
        };

        ((IMakeValueEqual)_subject)
            .MakeValueEqualTo(other)
            .Should()
            .BeTrue();
        
        _subject.Content
            .Should()
            .BeOfType<StaticMapPrototype>();
            
        _subject.Content
            .As<StaticMapPrototype>()
            .Image
            .Should()
            .Be(imageId);
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
        var clone = ((ICloneable)_subject).Clone();
        
        clone.Should()
            .BeOfType<MapPrototype>()
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
        var imageId = ImageId.New();
        
        _subject.Content = new StaticMapPrototype { Image = imageId };
        
        var other = new MapPrototype
        {
            Content = new StaticMapPrototype { Image = imageId }
        };
        
        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenOtherIsSameTypeAndImagesAreNotEqual()
    {
        _subject.Content = new StaticMapPrototype { Image = ImageId.New() };
        
        var other = new MapPrototype
        {
            Content = new StaticMapPrototype { Image = ImageId.New() }
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