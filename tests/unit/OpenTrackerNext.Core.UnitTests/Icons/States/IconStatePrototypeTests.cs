using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using FluentAssertions.Reactive;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTrackerNext.Core.Equality;
using OpenTrackerNext.Core.Icons.States;
using OpenTrackerNext.Core.Images;
using OpenTrackerNext.Core.Json;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Icons.States;

[ExcludeFromCodeCoverage]
public sealed class IconStatePrototypeTests
{
    private readonly IconStatePrototype _subject = new();
    
    [Fact]
    public void DocumentChanges_ShouldEmitWhenAnyPropertyChanges()
    {
        using var observer = _subject.DocumentChanges.Observe();
        
        _subject.Image = ImageId.New();

        observer.Should().Push(1);
    }

    [Fact]
    public void Image_ShouldInitializeToEmptyImageId()
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
        var imageId = ImageId.New();
        
        _subject.Image = imageId;

        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(IconStatePrototype.Image)]?
            .Value<string>()
            .Should()
            .Be(imageId.Value.ToString());
    }
    
    [Fact]
    public void Image_ShouldDeserializeAsExpected()
    {
        var imageId = ImageId.New();

        var jsonObject = new JObject
        {
            { nameof(IconStatePrototype.Image), new JValue(imageId.Value.ToString()) }
        };
        
        var json = jsonObject.ToString(Formatting.Indented);
        
        var subject = JsonContext.Deserialize<IconStatePrototype>(json);
        
        subject.Image
            .Should()
            .Be(imageId);
    }
    
    [Fact]
    public void MakeValueEqualTo_ShouldDoNothing_WhenOtherIsReferenceEqual()
    {
        ((IMakeValueEqual) _subject)
            .MakeValueEqualTo(_subject)
            .Should()
            .BeTrue();
    }

    [Fact]
    public void MakeValueEqualTo_ShouldSetImageToBeValueEqual()
    {
        _subject.Image = ImageId.New();
        
        var other = new IconStatePrototype { Image = ImageId.New() };

        ((IMakeValueEqual) _subject)
            .MakeValueEqualTo(other)
            .Should()
            .BeTrue();

        _subject.Image
            .Should()
            .Be(other.Image);
    }
    
    [Fact]
    public void MakeValueEqualTo_ShouldDoNothingAndReturnFalse_WhenOtherIsNotSameType()
    {
        ((IMakeValueEqual) _subject)
            .MakeValueEqualTo(new object())
            .Should()
            .BeFalse();
    }

    [Fact]
    public void Clone_ShouldReturnNewEquivalentInstance()
    {
        var clone = ((ICloneable)_subject).Clone();

        clone.Should()
            .BeOfType<IconStatePrototype>()
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
    public void ValueEquals_ShouldReturnFalse_WhenImageIsNotEqual()
    {
        _subject.Image = ImageId.New();
        
        var other = new IconStatePrototype { Image = ImageId.New() };

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnTrue_WhenImageIsEqual()
    {
        _subject.Image = ImageId.New();
        
        var other = new IconStatePrototype { Image = _subject.Image };

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
            .ValueEquals(null)
            .Should()
            .BeFalse();
    }
}