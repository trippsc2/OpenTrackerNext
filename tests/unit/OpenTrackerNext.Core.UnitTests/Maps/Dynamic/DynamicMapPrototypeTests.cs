using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using FluentAssertions.Reactive;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTrackerNext.Core.Equality;
using OpenTrackerNext.Core.Images;
using OpenTrackerNext.Core.Json;
using OpenTrackerNext.Core.Maps.Dynamic;
using OpenTrackerNext.Core.Requirements;
using OpenTrackerNext.Core.Requirements.Aggregate;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Maps.Dynamic;

[ExcludeFromCodeCoverage]
public sealed class DynamicMapPrototypeTests
{
    private readonly DynamicMapPrototype _subject = new();

    [Fact]
    public void DocumentChanges_ShouldEmitWhenPropertiesChange()
    {
        using var observer = _subject.DocumentChanges.Observe();

        _subject.MetImage = ImageId.New();

        observer.Should().Push(1);
    }

    [Fact]
    public void Requirement_ShouldDefaultAsExpected()
    {
        _subject.Requirement
            .Should()
            .NotBeNull();

        _subject.Requirement
            .Content
            .Should()
            .BeOfType<NullRequirementPrototype>();
    }
    
    [Fact]
    public void Requirement_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.Requirement = new RequirementPrototype();
        
        monitor.Should().RaisePropertyChangeFor(x => x.Requirement);
    }
    
    [Fact]
    public void Requirement_ShouldSerializeAsExpected()
    {
        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);
        
        jsonObject[nameof(DynamicMapPrototype.Requirement)]?
            .Value<JObject>()
            .Should()
            .NotBeNull();
    }
    
    [Fact]
    public void Requirement_ShouldDeserializeAsExpected()
    {
        var jsonObject = new JObject
        {
            { 
                nameof(DynamicMapPrototype.Requirement), 
                new JObject
                {
                    {
                        nameof(RequirementPrototype.Content),
                        new JObject
                        {
                            { "$type", new JValue("null") }
                        }
                    }
                }
            }
        };
        
        var json = jsonObject.ToString(Formatting.Indented);
        
        var subject = JsonContext.Deserialize<DynamicMapPrototype>(json);
        
        subject.Requirement
            .Should()
            .BeOfType<RequirementPrototype>()
            .Subject
            .Content
            .Should()
            .BeOfType<NullRequirementPrototype>();
    }
    
    [Fact]
    public void MetImage_ShouldDefaultAsExpected()
    {
        _subject.MetImage
            .Should()
            .Be(ImageId.Empty);
    }
    
    [Fact]
    public void MetImage_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.MetImage = ImageId.New();
        
        monitor.Should().RaisePropertyChangeFor(x => x.MetImage);
    }
    
    [Fact]
    public void MetImage_ShouldSerializeAsExpected()
    {
        var image = ImageId.New();
        _subject.MetImage = image;
        
        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);
        
        jsonObject[nameof(DynamicMapPrototype.MetImage)]?
            .Value<string>()
            .Should()
            .Be(image.Value.ToString());
    }

    [Fact]
    public void MetImage_ShouldDeserializeAsExpected()
    {
        var image = ImageId.New();
        var jsonObject = new JObject
        {
            { nameof(DynamicMapPrototype.MetImage), image.Value.ToString() }
        };
        
        var json = jsonObject.ToString(Formatting.Indented);
        
        var subject = JsonContext.Deserialize<DynamicMapPrototype>(json);
        
        subject.MetImage
            .Should()
            .Be(image);
    }
    
    [Fact]
    public void UnmetImage_ShouldDefaultAsExpected()
    {
        _subject.UnmetImage
            .Should()
            .Be(ImageId.Empty);
    }
    
    [Fact]
    public void UnmetImage_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.UnmetImage = ImageId.New();
        
        monitor.Should().RaisePropertyChangeFor(x => x.UnmetImage);
    }
    
    [Fact]
    public void UnmetImage_ShouldSerializeAsExpected()
    {
        var image = ImageId.New();
        _subject.UnmetImage = image;
        
        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);
        
        jsonObject[nameof(DynamicMapPrototype.UnmetImage)]?
            .Value<string>()
            .Should()
            .Be(image.Value.ToString());
    }
    
    [Fact]
    public void UnmetImage_ShouldDeserializeAsExpected()
    {
        var image = ImageId.New();
        var jsonObject = new JObject
        {
            { nameof(DynamicMapPrototype.UnmetImage), image.Value.ToString() }
        };
        
        var json = jsonObject.ToString(Formatting.Indented);
        
        var subject = JsonContext.Deserialize<DynamicMapPrototype>(json);
        
        subject.UnmetImage
            .Should()
            .Be(image);
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
    public void MakeValueEqualTo_ShouldSetValueEqualToOther()
    {
        var other = new DynamicMapPrototype
        {
            Requirement = new RequirementPrototype { Content = new AggregateRequirementPrototype() },
            MetImage = ImageId.New(),
            UnmetImage = ImageId.New()
        };

        ((IMakeValueEqual)_subject)
            .MakeValueEqualTo(other)
            .Should()
            .BeTrue();

        _subject.Requirement
            .Content
            .Should()
            .BeOfType<AggregateRequirementPrototype>();

        _subject.MetImage
            .Should()
            .Be(other.MetImage);

        _subject.UnmetImage
            .Should()
            .Be(other.UnmetImage);
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
            .BeOfType<DynamicMapPrototype>()
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
        var other = new DynamicMapPrototype
        {
            Requirement = new RequirementPrototype { Content = new AggregateRequirementPrototype() },
            MetImage = ImageId.New(),
            UnmetImage = ImageId.New()
        };

        _subject.Requirement.Content = new AggregateRequirementPrototype();
        _subject.MetImage = other.MetImage;
        _subject.UnmetImage = other.UnmetImage;

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenRequirementIsNotEqual()
    {
        var other = new DynamicMapPrototype
        {
            Requirement = new RequirementPrototype { Content = new AggregateRequirementPrototype() },
            MetImage = ImageId.New(),
            UnmetImage = ImageId.New()
        };
        
        _subject.Requirement.Content = new NullRequirementPrototype();
        _subject.MetImage = other.MetImage;
        _subject.UnmetImage = other.UnmetImage;

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenMetImageIsNotEqual()
    {
        var other = new DynamicMapPrototype
        {
            Requirement = new RequirementPrototype { Content = new AggregateRequirementPrototype() },
            MetImage = ImageId.New(),
            UnmetImage = ImageId.New()
        };
        
        _subject.Requirement.Content = new AggregateRequirementPrototype();
        _subject.MetImage = ImageId.New();
        _subject.UnmetImage = other.UnmetImage;

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenUnmetImageIsNotEqual()
    {
        var other = new DynamicMapPrototype
        {
            Requirement = new RequirementPrototype { Content = new AggregateRequirementPrototype() },
            MetImage = ImageId.New(),
            UnmetImage = ImageId.New()
        };
        
        _subject.Requirement.Content = new AggregateRequirementPrototype();
        _subject.MetImage = other.MetImage;
        _subject.UnmetImage = ImageId.New();

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