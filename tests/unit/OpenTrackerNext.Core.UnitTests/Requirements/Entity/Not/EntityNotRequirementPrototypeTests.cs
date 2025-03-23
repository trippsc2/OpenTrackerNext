using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using FluentAssertions.Reactive;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTrackerNext.Core.Entities;
using OpenTrackerNext.Core.Equality;
using OpenTrackerNext.Core.Json;
using OpenTrackerNext.Core.Requirements.Entity.Not;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Requirements.Entity.Not;

[ExcludeFromCodeCoverage]
public sealed class EntityNotRequirementPrototypeTests
{
    private readonly EntityNotRequirementPrototype _subject = new();
    
    [Fact]
    public void DocumentChanges_ShouldEmitWhenPropertiesChange()
    {
        using var observer = _subject.DocumentChanges.Observe();
        
        _subject.Entity = EntityId.New();
        _subject.ExcludedValue = 1;

        observer.Should().Push(2);
    }
    
    [Fact]
    public void Entity_ShouldDefaultToEmpty()
    {
        _subject.Entity
            .Should()
            .Be(EntityId.Empty);
    }

    [Fact]
    public void Entity_ShouldRaisePropertyChanged()
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

        jsonObject[nameof(EntityNotRequirementPrototype.Entity)]?
            .Value<string>()
            .Should()
            .Be(entity.Value.ToString());
    }
    
    [Fact]
    public void Entity_ShouldDeserializeAsExpected()
    {
        var entity = EntityId.New();

        var jsonObject = new JObject
        {
            { nameof(EntityNotRequirementPrototype.Entity), new JValue(entity.Value.ToString()) }
        };

        var json = jsonObject.ToString(Formatting.Indented);

        var subject = JsonContext.Deserialize<EntityNotRequirementPrototype>(json);
        
        subject.Entity
            .Should()
            .Be(entity);
    }
    
    [Fact]
    public void ExcludedValue_ShouldDefaultToZero()
    {
        _subject.ExcludedValue
            .Should()
            .Be(0);
    }
    
    [Fact]
    public void ExcludedValue_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.ExcludedValue = 1;
        
        monitor.Should().RaisePropertyChangeFor(x => x.ExcludedValue);
    }
    
    [Fact]
    public void ExcludedValue_ShouldSerializeAsExpected()
    {
        const int excludedValue = 1;
        
        _subject.ExcludedValue = excludedValue;

        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(EntityNotRequirementPrototype.ExcludedValue)]?
            .Value<int>()
            .Should()
            .Be(excludedValue);
    }
    
    [Fact]
    public void ExcludedValue_ShouldDeserializeAsExpected()
    {
        const int excludedValue = 1;

        var jsonObject = new JObject
        {
            { nameof(EntityNotRequirementPrototype.ExcludedValue), new JValue(excludedValue) }
        };

        var json = jsonObject.ToString(Formatting.Indented);

        var subject = JsonContext.Deserialize<EntityNotRequirementPrototype>(json);
        
        subject.ExcludedValue
            .Should()
            .Be(excludedValue);
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
    public void MakeValueEqualTo_ShouldSetPropertiesValueEqualToOther()
    {
        const int excludedValue = 1;
        
        var entity = EntityId.New();
        
        var other = new EntityNotRequirementPrototype
        {
            Entity = entity,
            ExcludedValue = excludedValue
        };
        
        ((IMakeValueEqual)_subject)
            .MakeValueEqualTo(other)
            .Should()
            .BeTrue();

        _subject.Entity
            .Should()
            .Be(entity);

        _subject.ExcludedValue
            .Should()
            .Be(excludedValue);
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
        _subject.Entity = EntityId.New();
        _subject.ExcludedValue = 1;
        
        var clone = ((ICloneable)_subject).Clone();

        clone.Should()
            .BeOfType<EntityNotRequirementPrototype>()
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
        _subject.Entity = EntityId.New();
        _subject.ExcludedValue = 1;
        
        var other = new EntityNotRequirementPrototype
        {
            Entity = _subject.Entity,
            ExcludedValue = _subject.ExcludedValue
        };

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenEntityIsNotEqual()
    {
        _subject.Entity = EntityId.New();
        _subject.ExcludedValue = 1;
        
        var other = new EntityNotRequirementPrototype
        {
            Entity = EntityId.New(),
            ExcludedValue = _subject.ExcludedValue
        };

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenRequiredValueIsNotEqual()
    {
        _subject.Entity = EntityId.New();
        _subject.ExcludedValue = 1;
        
        var other = new EntityNotRequirementPrototype
        {
            Entity = _subject.Entity,
            ExcludedValue = 2
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