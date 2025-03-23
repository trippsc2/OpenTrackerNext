using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Nodes;
using FluentAssertions;
using FluentAssertions.Reactive;
using OpenTrackerNext.Core.Entities;
using OpenTrackerNext.Core.Equality;
using OpenTrackerNext.Core.Json;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Entities;

[ExcludeFromCodeCoverage]
public sealed class EntityPrototypeTests
{
    private readonly EntityPrototype _subject = new();
    
    [Fact]
    public void TitlePrefix_ShouldStartEqualToEntityPrefix()
    {
        EntityPrototype.TitlePrefix
            .Should()
            .Be(EntityPrototype.EntityTitlePrefix);
    }

    [Fact]
    public void DocumentChanges_ShouldEmitWhenAnyPropertyChanges()
    {
        using var observer = _subject.DocumentChanges.Observe();
        
        _subject.Minimum = 1;
        _subject.Starting = 2;
        _subject.Maximum = 3;

        observer.Should().Push(3);
    }
    
    [Fact]
    public void Minimum_ShouldStartEqualToZero()
    {
        _subject.Minimum
            .Should()
            .Be(0);
    }

    [Fact]
    public void Minimum_ShouldRaisePropertyChanged_WhenValueChanges()
    {
        var monitor = _subject.Monitor();
        
        _subject.Minimum = 1;
        
        monitor.Should().RaisePropertyChangeFor(x => x.Minimum);
    }

    [Fact]
    public void Minimum_ShouldSerializeAsExpected()
    {
        const int minimum = 1;

        _subject.Minimum = minimum;

        var json = JsonContext.Serialize(_subject);
        var jsonObject = JsonNode.Parse(json);
        
        jsonObject?[nameof(EntityPrototype.Minimum)]?
            .GetValue<int>()
            .Should()
            .Be(minimum);
    }

    [Fact]
    public void Minimum_ShouldDeserializeAsExpected()
    {
        const int minimum = 1;

        var jsonObject = new JsonObject
        {
            { nameof(EntityPrototype.Minimum), JsonValue.Create(minimum) }
        };
        
        var json = jsonObject.ToJsonString(new JsonSerializerOptions { WriteIndented = true });

        var subject = JsonContext.Deserialize<EntityPrototype>(json);

        subject.Minimum
            .Should()
            .Be(minimum);
    }

    [Fact]
    public void Starting_ShouldStartEqualToZero()
    {
        _subject.Starting
            .Should()
            .Be(0);
    }
    
    [Fact]
    public void Starting_ShouldRaisePropertyChanged_WhenValueChanges()
    {
        using var monitor = _subject.Monitor();
        
        _subject.Starting = 1;
        
        monitor.Should().RaisePropertyChangeFor(x => x.Starting);
    }

    [Fact]
    public void Starting_ShouldSerializeAsExpected()
    {
        const int starting = 1;

        _subject.Starting = starting;

        var json = JsonContext.Serialize(_subject);
        var jsonObject = JsonNode.Parse(json);
        
        jsonObject?[nameof(EntityPrototype.Starting)]?
            .GetValue<int>()
            .Should()
            .Be(starting);
    }

    [Fact]
    public void Starting_ShouldDeserializeAsExpected()
    {
        const int starting = 1;

        var jsonObject = new JsonObject
        {
            { nameof(EntityPrototype.Starting), JsonValue.Create(starting) }
        };
        
        var json = jsonObject.ToJsonString(new JsonSerializerOptions { WriteIndented = true });

        var subject = JsonContext.Deserialize<EntityPrototype>(json);

        subject.Starting
            .Should()
            .Be(starting);
    }

    [Fact]
    public void Maximum_ShouldStartEqualToOne()
    {
        _subject.Maximum
            .Should()
            .Be(1);
    }
    
    [Fact]
    public void Maximum_ShouldRaisePropertyChanged_WhenValueChanges()
    {
        using var monitor = _subject.Monitor();
        
        _subject.Maximum = 2;
        
        monitor.Should().RaisePropertyChangeFor(x => x.Maximum);
    }

    [Fact]
    public void Maximum_ShouldSerializeAsExpected()
    {
        const int maximum = 2;

        _subject.Maximum = maximum;

        var json = JsonContext.Serialize(_subject);
        var jsonObject = JsonNode.Parse(json);
        
        jsonObject?[nameof(EntityPrototype.Maximum)]?
            .GetValue<int>()
            .Should()
            .Be(maximum);
    }

    [Fact]
    public void Maximum_ShouldDeserializeAsExpected()
    {
        const int maximum = 2;

        var jsonObject = new JsonObject
        {
            { nameof(EntityPrototype.Maximum), JsonValue.Create(maximum) }
        };
        
        var json = jsonObject.ToJsonString(new JsonSerializerOptions { WriteIndented = true });

        var subject = JsonContext.Deserialize<EntityPrototype>(json);

        subject.Maximum
            .Should()
            .Be(maximum);
    }
    
    [Fact]
    public void MakeValueEqualTo_ShouldReturnTrueAndDoNothing_WhenReferenceEqual()
    {
        ((IMakeValueEqual) _subject)
            .MakeValueEqualTo(_subject)
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void MakeValueEqualTo_ShouldSetPropertiesValueEqual_WhenOtherIsSameType()
    {
        _subject.Minimum = 1;
        _subject.Starting = 2;
        _subject.Maximum = 3;

        var other = new EntityPrototype
        {
            Minimum = 4,
            Starting = 5,
            Maximum = 6
        };

        ((IMakeValueEqual) _subject)
            .MakeValueEqualTo(other)
            .Should()
            .BeTrue();
        
        _subject.Minimum
            .Should()
            .Be(other.Minimum);
        _subject.Starting
            .Should()
            .Be(other.Starting);
        _subject.Maximum
            .Should()
            .Be(other.Maximum);
    }
    
    [Fact]
    public void MakeValueEqualTo_ShouldReturnFalseAndDoNothing_WhenOtherIsDifferentType()
    {
        _subject.Minimum = 1;
        _subject.Starting = 2;
        _subject.Maximum = 3;

        ((IMakeValueEqual) _subject)
            .MakeValueEqualTo(new object())
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void Clone_ShouldReturnNewInstance()
    {
        _subject.Minimum = 1;
        _subject.Starting = 2;
        _subject.Maximum = 3;

        var clone = ((ICloneable) _subject).Clone();

        clone
            .Should()
            .BeOfType<EntityPrototype>()
            .And.NotBeSameAs(_subject);

        ((IValueEquatable) _subject)
            .ValueEquals(clone)
            .Should()
            .BeTrue();
    }

    [Fact]
    public void ValueEquals_ShouldReturnTrue_WhenReferenceEqual()
    {
        ((IValueEquatable)_subject)
            .ValueEquals(_subject)
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnTrue_WhenValuesAreEqual()
    {
        _subject.Minimum = 1;
        _subject.Starting = 2;
        _subject.Maximum = 3;

        var other = new EntityPrototype
        {
            Minimum = 1,
            Starting = 2,
            Maximum = 3
        };

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenMinimumValuesAreNotEqual()
    {
        _subject.Minimum = 1;
        _subject.Starting = 2;
        _subject.Maximum = 3;

        var other = new EntityPrototype
        {
            Minimum = 0,
            Starting = 2,
            Maximum = 3
        };

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenStartingValuesAreNotEqual()
    {
        _subject.Minimum = 1;
        _subject.Starting = 2;
        _subject.Maximum = 3;

        var other = new EntityPrototype
        {
            Minimum = 1,
            Starting = 1,
            Maximum = 3
        };

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenMaximumValuesAreNotEqual()
    {
        _subject.Minimum = 1;
        _subject.Starting = 2;
        _subject.Maximum = 3;

        var other = new EntityPrototype
        {
            Minimum = 1,
            Starting = 2,
            Maximum = 4
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