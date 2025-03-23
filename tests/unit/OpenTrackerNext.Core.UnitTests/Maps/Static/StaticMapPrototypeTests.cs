using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTrackerNext.Core.Equality;
using OpenTrackerNext.Core.Images;
using OpenTrackerNext.Core.Json;
using OpenTrackerNext.Core.Maps.Static;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Maps.Static;

[ExcludeFromCodeCoverage]
public sealed class StaticMapPrototypeTests
{
    private readonly StaticMapPrototype _subject = new();

    [Fact]
    public void Image_ShouldInitializeAsEmptyImageId()
    {
        _subject.Image
            .Should()
            .Be(ImageId.Empty);
    }

    [Fact]
    public void Image_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.Image = ImageId.New();
        
        monitor.Should().RaisePropertyChangeFor(x => x.Image);
    }

    [Fact]
    public void Image_ShouldSerializeAsExpected()
    {
        _subject.Image = ImageId.New();

        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(StaticMapPrototype.Image)]?
            .Value<string>()
            .Should()
            .Be(_subject.Image.Value.ToString());
    }
    
    [Fact]
    public void Image_ShouldDeserializeAsExpected()
    {
        var imageId = ImageId.New();

        var jsonObject = new JObject
        {
            { nameof(StaticMapPrototype.Image), new JValue(imageId.Value.ToString()) }
        };
        var json = jsonObject.ToString(Formatting.Indented);
        
        var subject = JsonContext.Deserialize<StaticMapPrototype>(json);

        subject.Image
            .Should()
            .Be(imageId);
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
    public void MakeValueEqualTo_ShouldSetImageToValueEqual()
    {
        var other = new StaticMapPrototype { Image = ImageId.New() };
        
        ((IMakeValueEqual)_subject)
            .MakeValueEqualTo(other)
            .Should()
            .BeTrue();

        _subject.Image
            .Should()
            .Be(other.Image);
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
            .BeOfType<StaticMapPrototype>()
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
        _subject.Image = ImageId.New();
        var other = new StaticMapPrototype { Image = _subject.Image };

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenImageIsNotEqual()
    {
        _subject.Image = ImageId.New();
        var other = new StaticMapPrototype { Image = ImageId.New() };

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenOtherIsDifferentType()
    {
        ((IValueEquatable) _subject)
            .ValueEquals(new object())
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenOtherIsNull()
    {
        ((IValueEquatable) _subject)
            .ValueEquals(null)
            .Should()
            .BeFalse();
    }
}