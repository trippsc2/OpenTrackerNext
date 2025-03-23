using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using FluentAssertions.Reactive;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTrackerNext.Core.Entities;
using OpenTrackerNext.Core.Equality;
using OpenTrackerNext.Core.Json;
using OpenTrackerNext.Core.Requirements.Entity.Exact;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Requirements.Entity.Exact;

[ExcludeFromCodeCoverage]
public sealed class EntityExactRequirementPrototypeTests
{
    private readonly EntityExactRequirementPrototype _subject = new();
    
    [Fact]
    public void DocumentChanges_ShouldEmitWhenPropertiesChange()
    {
        using var observer = _subject.DocumentChanges.Observe();
        
        _subject.Entity = EntityId.New();
        _subject.RequiredValue = 1;

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

        jsonObject[nameof(EntityExactRequirementPrototype.Entity)]?
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
            { nameof(EntityExactRequirementPrototype.Entity), new JValue(entity.Value.ToString()) }
        };

        var json = jsonObject.ToString(Formatting.Indented);

        var subject = JsonContext.Deserialize<EntityExactRequirementPrototype>(json);
        
        subject.Entity
            .Should()
            .Be(entity);
    }
    
    [Fact]
    public void RequiredValue_ShouldDefaultToZero()
    {
        _subject.RequiredValue
            .Should()
            .Be(0);
    }
    
    [Fact]
    public void RequiredValue_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.RequiredValue = 1;
        
        monitor.Should().RaisePropertyChangeFor(x => x.RequiredValue);
    }
    
    [Fact]
    public void RequiredValue_ShouldSerializeAsExpected()
    {
        const int requiredValue = 1;
        
        _subject.RequiredValue = requiredValue;

        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(EntityExactRequirementPrototype.RequiredValue)]?
            .Value<int>()
            .Should()
            .Be(requiredValue);
    }
    
    [Fact]
    public void RequiredValue_ShouldDeserializeAsExpected()
    {
        const int requiredValue = 1;

        var jsonObject = new JObject
        {
            { nameof(EntityExactRequirementPrototype.RequiredValue), new JValue(requiredValue) }
        };

        var json = jsonObject.ToString(Formatting.Indented);

        var subject = JsonContext.Deserialize<EntityExactRequirementPrototype>(json);
        
        subject.RequiredValue
            .Should()
            .Be(requiredValue);
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
        const int requiredValue = 1;
        
        var entity = EntityId.New();
        
        var other = new EntityExactRequirementPrototype
        {
            Entity = entity,
            RequiredValue = requiredValue
        };
        
        ((IMakeValueEqual)_subject)
            .MakeValueEqualTo(other)
            .Should()
            .BeTrue();

        _subject.Entity
            .Should()
            .Be(entity);

        _subject.RequiredValue
            .Should()
            .Be(requiredValue);
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
        _subject.RequiredValue = 1;
        
        var clone = ((ICloneable)_subject).Clone();

        clone.Should()
            .BeOfType<EntityExactRequirementPrototype>()
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
        _subject.RequiredValue = 1;
        
        var other = new EntityExactRequirementPrototype
        {
            Entity = _subject.Entity,
            RequiredValue = _subject.RequiredValue
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
        _subject.RequiredValue = 1;
        
        var other = new EntityExactRequirementPrototype
        {
            Entity = EntityId.New(),
            RequiredValue = _subject.RequiredValue
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
        _subject.RequiredValue = 1;
        
        var other = new EntityExactRequirementPrototype
        {
            Entity = _subject.Entity,
            RequiredValue = 2
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