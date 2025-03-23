using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using FluentAssertions.Reactive;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTrackerNext.Core.Entities;
using OpenTrackerNext.Core.Equality;
using OpenTrackerNext.Core.Json;
using OpenTrackerNext.Core.Requirements;
using OpenTrackerNext.Core.Requirements.Aggregate;
using OpenTrackerNext.Core.Requirements.Alternative;
using OpenTrackerNext.Core.Requirements.Entity.AtLeast;
using OpenTrackerNext.Core.Requirements.Entity.AtMost;
using OpenTrackerNext.Core.Requirements.Entity.Exact;
using OpenTrackerNext.Core.Requirements.Entity.Not;
using OpenTrackerNext.Core.Requirements.UIPanel;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Requirements;

[ExcludeFromCodeCoverage]
public sealed class RequirementPrototypeTests
{
    private readonly RequirementPrototype _subject = new();
    
    [Fact]
    public void DocumentChanges_ShouldEmitWhenPropertyChanges()
    {
        using var observer = _subject.DocumentChanges.Observe();

        var content = new EntityExactRequirementPrototype();
        _subject.Content = content;
        content.Entity = EntityId.New();

        observer.Should().Push(2);
    }

    [Fact]
    public void Content_ShouldDefaultToNullRequirementPrototype()
    {
        _subject.Content
            .Should()
            .BeOfType<NullRequirementPrototype>();
    }
    
    [Fact]
    public void Content_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.Content = new EntityExactRequirementPrototype();
        
        monitor.Should().RaisePropertyChangeFor(x => x.Content);
    }

    [Theory]
    [InlineData("null", typeof(NullRequirementPrototype))]
    [InlineData("aggregate", typeof(AggregateRequirementPrototype))]
    [InlineData("alternative", typeof(AlternativeRequirementPrototype))]
    [InlineData("entity_at_least", typeof(EntityAtLeastRequirementPrototype))]
    [InlineData("entity_at_most", typeof(EntityAtMostRequirementPrototype))]
    [InlineData("entity_exact", typeof(EntityExactRequirementPrototype))]
    [InlineData("entity_not", typeof(EntityNotRequirementPrototype))]
    [InlineData("ui_panel_dock", typeof(UIPanelDockRequirementPrototype))]
    public void Content_ShouldSerializeAsExpected(string expected, Type type)
    {
        var content = (IRequirementSubtypePrototype)Activator.CreateInstance(type)!;
        _subject.Content = content;

        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(RequirementPrototype.Content)]?
            .Value<JObject>()?["$type"]?
            .Value<string>()
            .Should()
            .Be(expected);
    }
    
    [Theory]
    [InlineData(typeof(NullRequirementPrototype), "null")]
    [InlineData(typeof(AggregateRequirementPrototype), "aggregate")]
    [InlineData(typeof(AlternativeRequirementPrototype), "alternative")]
    [InlineData(typeof(EntityAtLeastRequirementPrototype), "entity_at_least")]
    [InlineData(typeof(EntityAtMostRequirementPrototype), "entity_at_most")]
    [InlineData(typeof(EntityExactRequirementPrototype), "entity_exact")]
    [InlineData(typeof(EntityNotRequirementPrototype), "entity_not")]
    [InlineData(typeof(UIPanelDockRequirementPrototype), "ui_panel_dock")]
    public void Content_ShouldDeserializeAsExpected(Type expected, string typeDiscriminator)
    {
        var jsonObject = new JObject
        {
            {
                nameof(RequirementPrototype.Content), new JObject
                {
                    { "$type", new JValue(typeDiscriminator) }
                }
            }
        };
        
        var json = jsonObject.ToString(Formatting.Indented);
        var subject = JsonContext.Deserialize<RequirementPrototype>(json);

        subject.Content
            .Should()
            .BeOfType(expected);
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
    public void MakeValueEqualTo_ShouldSetValueEqual_WhenOtherIsSameType()
    {
        const int requiredValue = 1;
        
        var entity = EntityId.New();
        
        var other = new RequirementPrototype
        {
            Content = new EntityExactRequirementPrototype
            {
                Entity = entity,
                RequiredValue = requiredValue
            }
        };
        
        ((IMakeValueEqual)_subject)
            .MakeValueEqualTo(other)
            .Should()
            .BeTrue();

        var content = _subject.Content
            .Should()
            .BeOfType<EntityExactRequirementPrototype>()
            .Subject;
        
        content.Entity
            .Should()
            .Be(entity);

        content.RequiredValue
            .Should()
            .Be(requiredValue);
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
        const int requiredValue = 1;
        
        var entity = EntityId.New();
        
        _subject.Content = new EntityExactRequirementPrototype
        {
            Entity = entity,
            RequiredValue = requiredValue
        };
        
        var clone = ((ICloneable)_subject).Clone();
        
        clone.Should()
            .NotBeSameAs(_subject)
            .And.BeOfType<RequirementPrototype>();

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
    public void ValueEquals_ShouldReturnFalse_WhenContentIsDifferentType()
    {
        _subject.Content = new EntityExactRequirementPrototype();
        
        var other = new RequirementPrototype { Content = new EntityAtLeastRequirementPrototype() };
        
        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenContentHasDifferentValues()
    {
        _subject.Content = new EntityExactRequirementPrototype { Entity = EntityId.New() };
        
        var other = new RequirementPrototype
        {
            Content = new EntityExactRequirementPrototype { Entity = EntityId.New() }
        };
        
        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnTrue_WhenContentHasSameValues()
    {
        var entity = EntityId.New();
        
        _subject.Content = new EntityExactRequirementPrototype { Entity = entity };
        
        var other = new RequirementPrototype
        {
            Content = new EntityExactRequirementPrototype { Entity = entity }
        };
        
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