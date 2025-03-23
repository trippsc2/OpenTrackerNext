using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using FluentAssertions.Reactive;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTrackerNext.Core.Equality;
using OpenTrackerNext.Core.Icons;
using OpenTrackerNext.Core.Icons.Counted;
using OpenTrackerNext.Core.Icons.DoubleEntity;
using OpenTrackerNext.Core.Icons.SingleEntity;
using OpenTrackerNext.Core.Icons.Static;
using OpenTrackerNext.Core.Images;
using OpenTrackerNext.Core.Json;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Icons;

[ExcludeFromCodeCoverage]
public sealed class IconPrototypeTests
{
    private readonly IconPrototype _subject = new();
    
    [Fact]
    public void TitlePrefix_ShouldReturnExpected()
    {
        IconPrototype.TitlePrefix
            .Should()
            .Be(IconPrototype.IconTitlePrefix);
    }
    
    [Fact]
    public void DocumentChanges_ShouldEmitWhenAnyPropertyChanges()
    {
        using var observer = _subject.DocumentChanges.Observe();
        
        var staticIcon = new StaticIconPrototype();
        
        _subject.Content = staticIcon;
        staticIcon.Image = ImageId.New();

        observer.Should().Push(2);
    }
    
    [Fact]
    public void Content_ShouldInitializeAsStaticIconPrototype()
    {
        _subject.Content
            .Should()
            .BeOfType<StaticIconPrototype>();
    }
    
    [Fact]
    public void Content_ShouldRaisePropertyChanged_WhenChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.Content = new CountedIconPrototype();
        
        monitor.Should().RaisePropertyChangeFor(x => x.Content);
    }
    
    [Theory]
    [InlineData("static", typeof(StaticIconPrototype))]
    [InlineData("counted", typeof(CountedIconPrototype))]
    [InlineData("single", typeof(SingleEntityIconPrototype))]
    [InlineData("double", typeof(DoubleEntityIconPrototype))]
    public void Content_ShouldSerializeAsExpected(string expected, Type type)
    {
        var content = (IIconSubtypePrototype)Activator.CreateInstance(type)!;
        
        _subject.Content = content;

        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(IconPrototype.Content)]?
            .Value<JObject>()?["$type"]?
            .Value<string>()
            .Should()
            .Be(expected);
    }

    [Theory]
    [InlineData(typeof(StaticIconPrototype), "static")]
    [InlineData(typeof(CountedIconPrototype), "counted")]
    [InlineData(typeof(SingleEntityIconPrototype), "single")]
    [InlineData(typeof(DoubleEntityIconPrototype), "double")]
    public void Content_ShouldDeserializeAsExpected(Type expected, string typeDiscriminator)
    {
        var jsonObject = new JObject
        {
            {
                nameof(IconPrototype.Content), new JObject
                {
                    { "$type", new JValue(typeDiscriminator) }
                }
            }
        };

        var json = jsonObject.ToString(Formatting.Indented);

        var subject = JsonContext.Deserialize<IconPrototype>(json);

        subject.Content
            .Should()
            .BeOfType(expected);
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
    public void MakeValueEqualTo_ShouldSetPropertyValuesEqual_WhenOtherIsSameTypeAndContentIsSameType()
    {
        _subject.Content = new CountedIconPrototype();
        
        var other = new IconPrototype
        {
            Content = new CountedIconPrototype { DefaultImage = ImageId.New() }
        };

        ((IMakeValueEqual) _subject)
            .MakeValueEqualTo(other)
            .Should()
            .BeTrue();
        
        _subject.Content
            .Should()
            .BeOfType(other.Content.GetType());
        
        _subject.Content
            .As<CountedIconPrototype>()
            .DisabledImage
            .Should()
            .Be(other.Content.As<CountedIconPrototype>().DisabledImage);
    }
    
    [Fact]
    public void MakeValueEqualTo_ShouldSetPropertyValuesEqual_WhenOtherIsSameTypeAndContentIsDifferentType()
    {
        _subject.Content = new StaticIconPrototype();
        
        var other = new IconPrototype
        {
            Content = new CountedIconPrototype { DisabledImage = ImageId.New() }
        };
        
        ((IMakeValueEqual) _subject)
            .MakeValueEqualTo(other)
            .Should()
            .BeTrue();
        
        _subject.Content
            .Should()
            .BeOfType(other.Content.GetType());
        
        _subject.Content
            .As<CountedIconPrototype>()
            .DisabledImage
            .Should()
            .Be(other.Content.As<CountedIconPrototype>().DisabledImage);
    }
    
    [Fact]
    public void MakeValueEqualTo_ShouldReturnFalseAndDoNothing_WhenOtherIsDifferentType()
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
        
        clone
            .Should()
            .NotBeSameAs(_subject);

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
    public void ValueEquals_ShouldReturnTrue_WhenOtherIsSameTypeAndContentIsSameTypeAndPropertyValuesAreEqual()
    {
        var other = new IconPrototype
        {
            Content = new CountedIconPrototype { DisabledImage = ImageId.New() }
        };
        
        _subject.MakeValueEqualTo(other);

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeTrue();
    }
        
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenOtherIsSameTypeAndContentIsSameTypeAndPropertyValuesAreNotEqual()
    {
        var other = new IconPrototype
        {
            Content = new CountedIconPrototype { DisabledImage = ImageId.New() }
        };
        
        _subject.MakeValueEqualTo(other);

        ((CountedIconPrototype) _subject.Content).DisabledImage = ImageId.New();

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }

    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenOtherIsSameTypeAndContentIsDifferentType()
    {
        _subject.Content = new CountedIconPrototype();
        
        var other = new IconPrototype
        {
            Content = new CountedIconPrototype { DisabledImage = ImageId.New() }
        };
        
        _subject.MakeValueEqualTo(other);

        _subject.Content = new StaticIconPrototype();

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