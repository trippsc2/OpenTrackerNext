using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using FluentAssertions.Reactive;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTrackerNext.Core.Entities;
using OpenTrackerNext.Core.Equality;
using OpenTrackerNext.Core.Icons.SingleEntity;
using OpenTrackerNext.Core.Icons.States;
using OpenTrackerNext.Core.Images;
using OpenTrackerNext.Core.Json;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Icons.SingleEntity;

[ExcludeFromCodeCoverage]
public sealed class SingleEntityIconPrototypeTests
{
    private readonly SingleEntityIconPrototype _subject = new();
    
    [Fact]
    public void DocumentChanges_ShouldEmitWhenAnyPropertyChanges()
    {
        using var observer = _subject.DocumentChanges.Observe();
        
        _subject.Entity = EntityId.New();
        _subject.SwapClickActions = true;
        _subject.CycleEntityCounts = true;
        _subject.States.Add(new IconStatePrototype());
        _subject.States[0].Image = ImageId.New();
        _subject.States.RemoveAt(0);

        observer.Should().Push(6);
    }
    
    [Fact]
    public void Entity_ShouldInitializeToEmptyEntityId()
    {
        _subject.Entity
            .Should()
            .Be(EntityId.Empty);
    }

    [Fact]
    public void Entity_ShouldRaisePropertyChanged_WhenChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.Entity = EntityId.New();
        
        monitor.Should().RaisePropertyChangeFor(x => x.Entity);
    }

    [Fact]
    public void Entity_ShouldSerializeAsExpected()
    {
        var entity = EntityId.New();
        
        _subject.Entity = entity;

        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);
        
        jsonObject[nameof(SingleEntityIconPrototype.Entity)]?
            .Value<string>()
            .Should()
            .Be(entity.ToString());
    }
    
    [Fact]
    public void Entity_ShouldDeserializeAsExpected()
    {
        var entity = EntityId.New();
        
        var jsonObject = new JObject
        {
            { nameof(SingleEntityIconPrototype.Entity), new JValue(entity.Value.ToString()) }
        };
        
        var json = jsonObject.ToString(Formatting.Indented);
        
        var subject = JsonContext.Deserialize<SingleEntityIconPrototype>(json);
        
        subject.Entity
            .Should()
            .Be(entity);
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
        
        jsonObject[nameof(SingleEntityIconPrototype.States)]?
            .Value<JArray>()
            .Should()
            .HaveCount(3);
        
        jsonObject[nameof(SingleEntityIconPrototype.States)]?
            .Value<JArray>()?[0]
            .Value<JObject>()?[nameof(IconStatePrototype.Image)]?
            .Value<string>()
            .Should()
            .Be(_subject.States[0].Image.Value.ToString());
        
        jsonObject[nameof(SingleEntityIconPrototype.States)]?
            .Value<JArray>()?[1]
            .Value<JObject>()?[nameof(IconStatePrototype.Image)]?
            .Value<string>()
            .Should()
            .Be(_subject.States[1].Image.Value.ToString());
        
        jsonObject[nameof(SingleEntityIconPrototype.States)]?
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
                nameof(SingleEntityIconPrototype.States),
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
        
        var subject = JsonContext.Deserialize<SingleEntityIconPrototype>(json);
        
        subject.States
            .Should()
            .HaveCount(3)
            .And.Contain(x => x.Image == state1Image)
            .And.Contain(x => x.Image == state2Image)
            .And.Contain(x => x.Image == state3Image);
    }
    
    [Fact]
    public void SwapClickActions_ShouldInitializeToFalse()
    {
        _subject.SwapClickActions
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void SwapClickActions_ShouldRaisePropertyChanged_WhenChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.SwapClickActions = true;
        
        monitor.Should().RaisePropertyChangeFor(x => x.SwapClickActions);
    }
    
    [Fact]
    public void SwapClickActions_ShouldSerializeAsExpected()
    {
        _subject.SwapClickActions = true;

        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);
        
        jsonObject[nameof(SingleEntityIconPrototype.SwapClickActions)]?
            .Value<bool>()
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void SwapClickActions_ShouldDeserializeAsExpected()
    {
        var jsonObject = new JObject
        {
            { nameof(SingleEntityIconPrototype.SwapClickActions), new JValue(true) }
        };
        
        var json = jsonObject.ToString(Formatting.Indented);
        
        var subject = JsonContext.Deserialize<SingleEntityIconPrototype>(json);
        
        subject.SwapClickActions
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void CycleEntityCounts_ShouldInitializeToFalse()
    {
        _subject.CycleEntityCounts
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void CycleEntityCounts_ShouldRaisePropertyChanged_WhenChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.CycleEntityCounts = true;
        
        monitor.Should().RaisePropertyChangeFor(x => x.CycleEntityCounts);
    }
    
    [Fact]
    public void CycleEntityCounts_ShouldSerializeAsExpected()
    {
        _subject.CycleEntityCounts = true;

        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);
        
        jsonObject[nameof(SingleEntityIconPrototype.CycleEntityCounts)]?
            .Value<bool>()
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void CycleEntityCounts_ShouldDeserializeAsExpected()
    {
        var jsonObject = new JObject
        {
            { nameof(SingleEntityIconPrototype.CycleEntityCounts), new JValue(true) }
        };
        
        var json = jsonObject.ToString(Formatting.Indented);
        
        var subject = JsonContext.Deserialize<SingleEntityIconPrototype>(json);
        
        subject.CycleEntityCounts
            .Should()
            .BeTrue();
    }

    [Fact]
    public void MakeValueEqualTo_ShouldDoNothing_WhenReferenceEqual()
    {
        ((IMakeValueEqual)_subject)
            .MakeValueEqualTo(_subject)
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void MakeValueEqualTo_ShouldSetPropertyValuesEqual_WhenOtherIsSameType()
    {
        var entity = EntityId.New();
        
        _subject.Entity = EntityId.Empty;
        _subject.SwapClickActions = false;
        _subject.CycleEntityCounts = false;

        var other = new SingleEntityIconPrototype
        {
            Entity = entity,
            SwapClickActions = true,
            CycleEntityCounts = true
        };

        ((IMakeValueEqual) _subject)
            .MakeValueEqualTo(other)
            .Should()
            .BeTrue();
        
        _subject.Entity
            .Should()
            .Be(entity);
        
        _subject.SwapClickActions
            .Should()
            .BeTrue();
        
        _subject.CycleEntityCounts
            .Should()
            .BeTrue();
    }

    [Fact]
    public void MakeValueEqualTo_ShouldSetStatesEqual_WhenOtherIsSameTypeAndStateCountsAreEqual()
    {
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });

        var other = new SingleEntityIconPrototype
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

        var other = new SingleEntityIconPrototype
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

        var other = new SingleEntityIconPrototype
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
        var clone = ((ICloneable)_subject).Clone();

        clone.Should()
            .BeOfType<SingleEntityIconPrototype>()
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
    public void ValueEquals_ShouldReturnTrue_WhenOtherIsSameTypeAndPropertiesAreEqual()
    {
        var entity = EntityId.New();
        
        _subject.Entity = entity;
        _subject.SwapClickActions = true;
        _subject.CycleEntityCounts = true;
        
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });

        var other = new SingleEntityIconPrototype
        {
            Entity = entity,
            SwapClickActions = true,
            CycleEntityCounts = true,
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
    public void ValueEquals_ShouldReturnFalse_WhenOtherIsSameTypeAndEntityValuesAreNotEqual()
    {
        _subject.Entity = EntityId.Empty;
        _subject.SwapClickActions = false;
        _subject.CycleEntityCounts = false;
        
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });

        var other = new SingleEntityIconPrototype
        {
            Entity = EntityId.New(),
            SwapClickActions = false,
            CycleEntityCounts = false,
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
    public void ValueEquals_ShouldReturnFalse_WhenOtherIsSameTypeAndSwapClickActionsValuesAreNotEqual()
    {
        _subject.Entity = EntityId.Empty;
        _subject.SwapClickActions = false;
        _subject.CycleEntityCounts = false;
        
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });

        var other = new SingleEntityIconPrototype
        {
            Entity = EntityId.Empty,
            SwapClickActions = true,
            CycleEntityCounts = false,
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
    public void ValueEquals_ShouldReturnFalse_WhenOtherIsSameTypeAndCycleEntityCountsValuesAreNotEqual()
    {
        _subject.Entity = EntityId.Empty;
        _subject.SwapClickActions = false;
        _subject.CycleEntityCounts = false;
        
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });

        var other = new SingleEntityIconPrototype
        {
            Entity = EntityId.Empty,
            SwapClickActions = false,
            CycleEntityCounts = true,
            States =
            [
                new IconStatePrototype { Image = _subject.States[0].Image },
                new IconStatePrototype { Image = _subject.States[1].Image },
                new IconStatePrototype { Image = _subject.States[2].Image }
            ]
        };

        ((IValueEquatable)_subject).ValueEquals(other)
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenOtherIsSameTypeAndStateCountsAreNotEqual()
    {
        _subject.Entity = EntityId.Empty;
        _subject.SwapClickActions = false;
        _subject.CycleEntityCounts = false;
        
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });

        var other = new SingleEntityIconPrototype
        {
            Entity = EntityId.Empty,
            SwapClickActions = false,
            CycleEntityCounts = false,
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
    public void ValueEquals_ShouldReturnFalse_WhenOtherIsSameTypeAndStateValuesAreNotEqual()
    {
        _subject.Entity = EntityId.Empty;
        _subject.SwapClickActions = false;
        _subject.CycleEntityCounts = false;
        
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });
        _subject.States.Add(new IconStatePrototype { Image = ImageId.New() });

        var other = new SingleEntityIconPrototype
        {
            Entity = EntityId.Empty,
            SwapClickActions = false,
            CycleEntityCounts = false,
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