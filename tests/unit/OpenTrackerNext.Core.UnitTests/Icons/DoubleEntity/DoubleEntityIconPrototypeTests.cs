using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using FluentAssertions.Reactive;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTrackerNext.Core.Entities;
using OpenTrackerNext.Core.Equality;
using OpenTrackerNext.Core.Icons.DoubleEntity;
using OpenTrackerNext.Core.Icons.States;
using OpenTrackerNext.Core.Images;
using OpenTrackerNext.Core.Json;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Icons.DoubleEntity;

[ExcludeFromCodeCoverage]
public sealed class DoubleEntityIconPrototypeTests
{
    private readonly DoubleEntityIconPrototype _subject = new();
    
    [Fact]
    public void DocumentChanges_ShouldEmitWhenAnyPropertyChanges()
    {
        using var observer = _subject.DocumentChanges.Observe();
        
        _subject.Entity1 = EntityId.New();
        _subject.Entity2 = EntityId.New();
        _subject.States.Add(new IconStatePrototype());
        _subject.States[0].Image = ImageId.New();
        _subject.States.RemoveAt(0);
        
        observer.Should().Push(5);
    }
    
    [Fact]
    public void Entity1_ShouldInitializeToEmptyEntityId()
    {
        _subject.Entity1
            .Should()
            .Be(EntityId.Empty);
    }

    [Fact]
    public void Entity1_ShouldRaisePropertyChanged_WhenChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.Entity1 = EntityId.New();
        
        monitor.Should().RaisePropertyChangeFor(x => x.Entity1);
    }

    [Fact]
    public void Entity1_ShouldSerializeAsExpected()
    {
        var entity1 = EntityId.New();
        
        _subject.Entity1 = entity1;

        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);
        
        jsonObject[nameof(DoubleEntityIconPrototype.Entity1)]?
            .Value<string>()
            .Should()
            .Be(entity1.ToString());
    }
    
    [Fact]
    public void Entity1_ShouldDeserializeAsExpected()
    {
        var entity1 = EntityId.New();
        
        var jsonObject = new JObject
        {
            { nameof(DoubleEntityIconPrototype.Entity1), new JValue(entity1.Value.ToString()) }
        };
        
        var json = jsonObject.ToString(Formatting.Indented);
        
        var subject = JsonContext.Deserialize<DoubleEntityIconPrototype>(json);
        
        subject.Entity1
            .Should()
            .Be(entity1);
    }

    [Fact]
    public void Entity2_ShouldInitializeToEmptyEntityId()
    {
        _subject.Entity2
            .Should()
            .Be(EntityId.Empty);
    }

    [Fact]
    public void Entity2_ShouldRaisePropertyChanged_WhenChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.Entity2 = EntityId.New();
        
        monitor.Should().RaisePropertyChangeFor(x => x.Entity2);
    }

    [Fact]
    public void Entity2_ShouldSerializeAsExpected()
    {
        var entity2 = EntityId.New();
        
        _subject.Entity2 = entity2;

        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);
        
        jsonObject[nameof(DoubleEntityIconPrototype.Entity2)]?
            .Value<string>()
            .Should()
            .Be(entity2.ToString());
    }
    
    [Fact]
    public void Entity2_ShouldDeserializeAsExpected()
    {
        var entity2 = EntityId.New();
        
        var jsonObject = new JObject
        {
            { nameof(DoubleEntityIconPrototype.Entity2), new JValue(entity2.Value.ToString()) }
        };
        
        var json = jsonObject.ToString(Formatting.Indented);
        
        var subject = JsonContext.Deserialize<DoubleEntityIconPrototype>(json);
        
        subject.Entity2
            .Should()
            .Be(entity2);
    }
    
    [Fact]
    public void States_ShouldInitializeToEmptyObservableCollection()
    {
        _subject.States
            .Should()
            .BeEmpty();
    }
    
    [Fact]
    public void States_ShouldSerializeAsExpected()
    {
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });

        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);
        
        jsonObject[nameof(DoubleEntityIconPrototype.States)]?
            .Value<JArray>()
            .Should()
            .HaveCount(3);
        
        jsonObject[nameof(DoubleEntityIconPrototype.States)]?
            .Value<JArray>()?[0]
            .Value<JObject>()?[nameof(IconStatePrototype.Image)]?
            .Value<string>()
            .Should()
            .Be(_subject.States[0].Image.Value.ToString());
        
        jsonObject[nameof(DoubleEntityIconPrototype.States)]?
            .Value<JArray>()?[1]
            .Value<JObject>()?[nameof(IconStatePrototype.Image)]?
            .Value<string>()
            .Should()
            .Be(_subject.States[1].Image.Value.ToString());
        
        jsonObject[nameof(DoubleEntityIconPrototype.States)]?
            .Value<JArray>()?[2]
            .Value<JObject>()?[nameof(IconStatePrototype.Image)]?
            .Value<string>()
            .Should()
            .Be(_subject.States[2].Image.Value.ToString());
    }
    
    [Fact]
    public void States_ShouldDeserializeAsExpected()
    {
        var state1Image = ImageId.New();
        var state2Image = ImageId.New();
        var state3Image = ImageId.New();
        
        var jsonObject = new JObject
        {
            {
                nameof(DoubleEntityIconPrototype.States),
                new JArray
                {
                    new JObject
                    {
                        { nameof(IconStatePrototype.Image), new JValue(state1Image.Value.ToString()) }
                    },
                    new JObject
                    {
                        { nameof(IconStatePrototype.Image), new JValue(state2Image.Value.ToString()) }
                    },
                    new JObject
                    {
                        { nameof(IconStatePrototype.Image), new JValue(state3Image.Value.ToString()) }
                    }
                }
            }
        };
        
        var json = jsonObject.ToString(Formatting.Indented);
        
        var subject = JsonContext.Deserialize<DoubleEntityIconPrototype>(json);

        subject.States
            .Should()
            .HaveCount(3);

        subject.States[0]
            .Image
            .Should()
            .Be(state1Image);

        subject.States[1]
            .Image
            .Should()
            .Be(state2Image);

        subject.States[2]
            .Image
            .Should()
            .Be(state3Image);
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
    public void MakeValueEqualTo_ShouldSetPropertyValuesEqual_WhenOtherIsSameType()
    {
        var entity1 = EntityId.New();
        var entity2 = EntityId.New();
        
        _subject.Entity1 = EntityId.Empty;
        _subject.Entity2 = EntityId.Empty;

        var other = new DoubleEntityIconPrototype
        {
            Entity1 = entity1,
            Entity2 = entity2
        };

        ((IMakeValueEqual) _subject)
            .MakeValueEqualTo(other)
            .Should()
            .BeTrue();
        
        _subject.Entity1
            .Should()
            .Be(entity1);
        
        _subject.Entity2
            .Should()
            .Be(entity2);
    }

    [Fact]
    public void MakeValueEqualTo_ShouldSetStatesEqual_WhenOtherIsSameTypeAndStateImageCountsAreEqual()
    {
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });
        
        var other = new DoubleEntityIconPrototype
        {
            States =
            [
                new IconStatePrototype { Image = ImageId.New() },
                new IconStatePrototype { Image = ImageId.New() },
                new IconStatePrototype { Image = ImageId.New() }
            ]
        };

        ((IMakeValueEqual) _subject)
            .MakeValueEqualTo(other)
            .Should()
            .BeTrue();
        
        _subject.States
            .Should()
            .BeEquivalentTo(other.States);
    }
    
    [Fact]
    public void MakeValueEqualTo_ShouldSetStatesEqual_WhenOtherIsSameTypeAndHasMoreStates()
    {
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });

        var other = new DoubleEntityIconPrototype
        {
            States =
            [
                new IconStatePrototype { Image = ImageId.New() },
                new IconStatePrototype { Image = ImageId.New() }
            ]
        };

        ((IMakeValueEqual) _subject)
            .MakeValueEqualTo(other)
            .Should()
            .BeTrue();
        
        _subject.States
            .Should()
            .BeEquivalentTo(other.States);
    }
    
    [Fact]
    public void MakeValueEqualTo_ShouldSetStatesEqual_WhenOtherIsSameTypeAndHasFewerStates()
    {
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });

        var other = new DoubleEntityIconPrototype
        {
            States =
            [
                new IconStatePrototype { Image = ImageId.New() },
                new IconStatePrototype { Image = ImageId.New() },
                new IconStatePrototype { Image = ImageId.New() }
            ]
        };

        ((IMakeValueEqual) _subject)
            .MakeValueEqualTo(other)
            .Should()
            .BeTrue();
        
        _subject.States
            .Should()
            .BeEquivalentTo(other.States);
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
        var clone = ((ICloneable) _subject).Clone();

        clone
            .Should()
            .BeOfType<DoubleEntityIconPrototype>()
            .And.NotBeSameAs(_subject);

        ((IValueEquatable)_subject).ValueEquals(clone)
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
    public void ValueEquals_ShouldReturnTrue_WhenOtherIsSameTypeAndPropertiesAreEqual()
    {
        var entity1 = EntityId.New();
        var entity2 = EntityId.New();

        _subject.Entity1 = entity1;
        _subject.Entity2 = entity2;
        
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });

        var other = new DoubleEntityIconPrototype
        {
            Entity1 = entity1,
            Entity2 = entity2,
            States =
            [
                new IconStatePrototype { Image = _subject.States[0].Image },
                new IconStatePrototype { Image = _subject.States[1].Image },
                new IconStatePrototype { Image = _subject.States[2].Image }
            ]
        };

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenOtherIsSameTypeAndEntity1ValuesAreNotEqual()
    {
        _subject.Entity1 = EntityId.Empty;
        _subject.Entity2 = EntityId.Empty;
        
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });

        var other = new DoubleEntityIconPrototype
        {
            Entity1 = EntityId.New(),
            Entity2 = EntityId.Empty,
            States =
            [
                new IconStatePrototype { Image = _subject.States[0].Image },
                new IconStatePrototype { Image = _subject.States[1].Image },
                new IconStatePrototype { Image = _subject.States[2].Image }
            ]
        };

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenOtherIsSameTypeAndEntity2ValuesAreNotEqual()
    {
        _subject.Entity1 = EntityId.Empty;
        _subject.Entity2 = EntityId.Empty;
        
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });

        var other = new DoubleEntityIconPrototype
        {
            Entity1 = EntityId.Empty,
            Entity2 = EntityId.New(),
            States =
            [
                new IconStatePrototype { Image = _subject.States[0].Image },
                new IconStatePrototype { Image = _subject.States[1].Image },
                new IconStatePrototype { Image = _subject.States[2].Image }
            ]
        };

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenOtherIsSameTypeAndStateImageCountsAreNotEqual()
    {
        _subject.Entity1 = EntityId.Empty;
        _subject.Entity2 = EntityId.Empty;
        
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });

        var other = new DoubleEntityIconPrototype
        {
            Entity1 = EntityId.Empty,
            Entity2 = EntityId.Empty,
            States =
            [
                new IconStatePrototype { Image = _subject.States[0].Image },
                new IconStatePrototype { Image = _subject.States[1].Image }
            ]
        };

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenOtherIsSameTypeAndStateImageValuesAreNotEqual()
    {
        _subject.Entity1 = EntityId.Empty;
        _subject.Entity2 = EntityId.Empty;
        
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });

        var other = new DoubleEntityIconPrototype
        {
            Entity1 = EntityId.Empty,
            Entity2 = EntityId.Empty,
            States =
            [
                new IconStatePrototype { Image = ImageId.New() },
                new IconStatePrototype { Image = ImageId.New() },
                new IconStatePrototype()
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