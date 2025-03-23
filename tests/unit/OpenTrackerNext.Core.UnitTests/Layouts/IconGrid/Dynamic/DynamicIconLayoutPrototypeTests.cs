using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using FluentAssertions.Reactive;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTrackerNext.Core.Entities;
using OpenTrackerNext.Core.Equality;
using OpenTrackerNext.Core.Icons;
using OpenTrackerNext.Core.Json;
using OpenTrackerNext.Core.Layouts.IconGrid.Dynamic;
using OpenTrackerNext.Core.Requirements;
using OpenTrackerNext.Core.Requirements.Aggregate;
using OpenTrackerNext.Core.Requirements.Alternative;
using OpenTrackerNext.Core.Requirements.Entity.Exact;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Layouts.IconGrid.Dynamic;

[ExcludeFromCodeCoverage]
public sealed class DynamicIconLayoutPrototypeTests
{
    private readonly DynamicIconLayoutPrototype _subject = new();

    [Fact]
    public void DocumentChanges_ShouldEmitWhenPropertyChanges()
    {
        using var observer = _subject.DocumentChanges.Observe();
        
        _subject.MetIcon = IconId.New();
        _subject.UnmetIcon = IconId.New();
        _subject.Requirement.Content = new EntityExactRequirementPrototype();
        
        observer.Should().Push(3);
    }

    [Fact]
    public void Requirement_ShouldDefaultToDefaultRequirementPrototype()
    {
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

        jsonObject[nameof(DynamicIconLayoutPrototype.Requirement)]?
            .Value<JObject>()
            .Should()
            .NotBeNull();
    }

    [Fact]
    public void Requirement_ShouldDeserializeAsExpected()
    {
        var jsonObject = new JObject
        {
            { nameof(DynamicIconLayoutPrototype.Requirement), new JObject() }
        };

        var json = jsonObject.ToString(Formatting.Indented);

        var subject = JsonContext.Deserialize<DynamicIconLayoutPrototype>(json);
        
        subject.Requirement
            .Content
            .Should()
            .BeOfType<NullRequirementPrototype>();
    }
    
    [Fact]
    public void MetIcon_ShouldDefaultToEmpty()
    {
        _subject.MetIcon
            .Should()
            .Be(IconId.Empty);
    }
    
    [Fact]
    public void MetIcon_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.MetIcon = IconId.New();
        
        monitor.Should().RaisePropertyChangeFor(x => x.MetIcon);
    }
    
    [Fact]
    public void MetIcon_ShouldSerializeAsExpected()
    {
        var metIcon = IconId.New();
        _subject.MetIcon = metIcon;
        
        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(DynamicIconLayoutPrototype.MetIcon)]?
            .Value<string>()
            .Should()
            .Be(metIcon.Value.ToString());
    }
    
    [Fact]
    public void MetIcon_ShouldDeserializeAsExpected()
    {
        var metIcon = IconId.New();
        
        var jsonObject = new JObject
        {
            { nameof(DynamicIconLayoutPrototype.MetIcon), new JValue(metIcon.Value.ToString()) }
        };

        var json = jsonObject.ToString(Formatting.Indented);

        var subject = JsonContext.Deserialize<DynamicIconLayoutPrototype>(json);
        
        subject.MetIcon
            .Should()
            .Be(metIcon);
    }
    
    [Fact]
    public void UnmetIcon_ShouldDefaultToEmpty()
    {
        _subject.UnmetIcon
            .Should()
            .Be(IconId.Empty);
    }
    
    [Fact]
    public void UnmetIcon_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.UnmetIcon = IconId.New();
        
        monitor.Should().RaisePropertyChangeFor(x => x.UnmetIcon);
    }
    
    [Fact]
    public void UnmetIcon_ShouldSerializeAsExpected()
    {
        var unmetIcon = IconId.New();
        _subject.UnmetIcon = unmetIcon;
        
        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(DynamicIconLayoutPrototype.UnmetIcon)]?
            .Value<string>()
            .Should()
            .Be(unmetIcon.Value.ToString());
    }
    
    [Fact]
    public void UnmetIcon_ShouldDeserializeAsExpected()
    {
        var unmetIcon = IconId.New();
        var jsonObject = new JObject
        {
            { nameof(DynamicIconLayoutPrototype.UnmetIcon), new JValue(unmetIcon.Value.ToString()) }
        };

        var json = jsonObject.ToString(Formatting.Indented);

        var subject = JsonContext.Deserialize<DynamicIconLayoutPrototype>(json);
        
        subject.UnmetIcon
            .Should()
            .Be(unmetIcon);
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
    public void MakeValueEqualTo_ShouldSetValueEqual()
    {
        var other = new DynamicIconLayoutPrototype
        {
            MetIcon = IconId.New(),
            UnmetIcon = IconId.New(),
            Requirement = new RequirementPrototype
            {
                Content = new EntityExactRequirementPrototype
                {
                    Entity = EntityId.New(),
                    RequiredValue = 7
                }
            }
        };
        
        ((IMakeValueEqual)_subject)
            .MakeValueEqualTo(other)
            .Should()
            .BeTrue();

        _subject.MetIcon
            .Should()
            .Be(other.MetIcon);

        _subject.UnmetIcon
            .Should()
            .Be(other.UnmetIcon);

        _subject.Requirement
            .Content
            .Should()
            .BeOfType<EntityExactRequirementPrototype>();
    }
    
    [Fact]
    public void MakeValueEqualTo_ShouldDoNothing_WhenOtherIsDifferentType()
    {
        ((IMakeValueEqual)_subject)
            .MakeValueEqualTo(new object())
            .Should()
            .BeFalse();
    }

    [Fact]
    public void Clone_ShouldReturnNewEquivalentInstance()
    {
        _subject.Requirement.Content = new AggregateRequirementPrototype();
        _subject.MetIcon = IconId.New();
        _subject.UnmetIcon = IconId.New();
        
        var clone = ((ICloneable)_subject).Clone();

        ((IValueEquatable)_subject)
            .ValueEquals(clone)
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnTrue_WhenRequirementIsEqual()
    {
        ((IValueEquatable)_subject)
            .ValueEquals(_subject)
            .Should()
            .BeTrue();
    }

    [Fact]
    public void ValueEquals_ShouldReturnTrue_WhenAllPropertiesAreValueEqual()
    {
        _subject.Requirement.Content = new AggregateRequirementPrototype();
        _subject.MetIcon = IconId.New();
        _subject.UnmetIcon = IconId.New();
        
        var other = new DynamicIconLayoutPrototype
        {
            Requirement = new RequirementPrototype { Content = new AggregateRequirementPrototype() },
            MetIcon = _subject.MetIcon,
            UnmetIcon = _subject.UnmetIcon
        };

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenRequirementIsNotEqual()
    {
        _subject.Requirement.Content = new AggregateRequirementPrototype();
        _subject.MetIcon = IconId.New();
        _subject.UnmetIcon = IconId.New();
        
        var other = new DynamicIconLayoutPrototype
        {
            Requirement = new RequirementPrototype { Content = new AlternativeRequirementPrototype() },
            MetIcon = _subject.MetIcon,
            UnmetIcon = _subject.UnmetIcon
        };

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenMetIconIsNotEqual()
    {
        _subject.Requirement.Content = new AggregateRequirementPrototype();
        _subject.MetIcon = IconId.New();
        _subject.UnmetIcon = IconId.New();
        
        var other = new DynamicIconLayoutPrototype
        {
            Requirement = new RequirementPrototype { Content = new AggregateRequirementPrototype() },
            MetIcon = IconId.New(),
            UnmetIcon = _subject.UnmetIcon
        };

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenUnmetIconIsNotEqual()
    {
        _subject.Requirement.Content = new AggregateRequirementPrototype();
        _subject.MetIcon = IconId.New();
        _subject.UnmetIcon = IconId.New();
        
        var other = new DynamicIconLayoutPrototype
        {
            Requirement = new RequirementPrototype { Content = new AggregateRequirementPrototype() },
            MetIcon = _subject.MetIcon,
            UnmetIcon = IconId.New()
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