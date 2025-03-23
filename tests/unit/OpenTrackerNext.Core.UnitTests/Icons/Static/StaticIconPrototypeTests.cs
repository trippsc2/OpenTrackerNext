using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using FluentAssertions.Reactive;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTrackerNext.Core.Equality;
using OpenTrackerNext.Core.Icons.Static;
using OpenTrackerNext.Core.Images;
using OpenTrackerNext.Core.Json;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Icons.Static;

[ExcludeFromCodeCoverage]
public sealed class StaticIconPrototypeTests
{
    private readonly StaticIconPrototype _subject = new();
    
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
    public void Image_ShouldRaisePropertyChanged_WhenChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.Image = ImageId.New();

        monitor.Should().RaisePropertyChangeFor(x => x.Image);
    }

    [Fact]
    public void Image_ShouldSerializeAsExpected()
    {
        var image = ImageId.New();
        
        _subject.Image = image;

        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(StaticIconPrototype.Image)]?
            .Value<string>()
            .Should()
            .Be(image.Value.ToString());
    }
    
    [Fact]
    public void Image_ShouldDeserializeAsExpected()
    {
        var image = ImageId.New();

        var jsonObject = new JObject
        {
            { nameof(StaticIconPrototype.Image), image.Value.ToString() }
        };
        
        var json = jsonObject.ToString(Formatting.Indented);

        var subject = JsonContext.Deserialize<StaticIconPrototype>(json);

        subject.Image
            .Should()
            .Be(image);
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
    public void MakeValueEqualTo_ShouldMakePropertiesValueEqual_WhenOtherValueIsSameType()
    {
        var other = new StaticIconPrototype();

        _subject.Image = ImageId.Empty;
        other.Image = ImageId.New();

        ((IMakeValueEqual) _subject)
            .MakeValueEqualTo(other)
            .Should()
            .BeTrue();

        _subject.Image
            .Should()
            .Be(other.Image);
    }
    
    [Fact]
    public void MakeValueEqualTo_ShouldReturnFalseAndDoNothing_WhenOtherValueIsDifferentType()
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
            .BeOfType<StaticIconPrototype>()
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
    public void ValueEquals_ShouldReturnTrue_WhenOtherValueIsSameTypeAndPropertiesAreEqual()
    {
        var other = new StaticIconPrototype();

        _subject.Image = ImageId.New();
        
        other.Image = _subject.Image;

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenOtherValueIsSameTypeAndPropertiesAreNotEqual()
    {
        var other = new StaticIconPrototype();

        _subject.Image = ImageId.Empty;
        
        other.Image = ImageId.New();

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenOtherValueIsDifferentType()
    {
        ((IValueEquatable)_subject)
            .ValueEquals(new object())
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenOtherValueIsNull()
    {
        ((IValueEquatable)_subject)
            .ValueEquals(null)
            .Should()
            .BeFalse();
    }
}