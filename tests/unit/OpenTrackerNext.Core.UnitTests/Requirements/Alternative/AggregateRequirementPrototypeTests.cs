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
using OpenTrackerNext.Core.Requirements.Alternative;
using OpenTrackerNext.Core.Requirements.Entity.AtLeast;
using OpenTrackerNext.Core.Requirements.Entity.Exact;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Requirements.Alternative;

[ExcludeFromCodeCoverage]
public sealed class AlternativeRequirementPrototypeTests
{
    private readonly AlternativeRequirementPrototype _subject = new();
    
    [Fact]
    public void DocumentChanges_ShouldEmitWhenPropertiesChange()
    {
        using var observer = _subject.DocumentChanges.Observe();

        var requirement = new RequirementPrototype();
        _subject.Requirements.Add(requirement);
        requirement.Content = new AlternativeRequirementPrototype();
        _subject.Requirements.RemoveAt(0);
        
        observer.Should().Push(3);
    }
    
    [Fact]
    public void Requirements_ShouldDefaultToEmpty()
    {
        _subject.Requirements
            .Should()
            .BeEmpty();
    }

    [Fact]
    public void Requirements_ShouldSerializeAsExpected()
    {
        _subject.Requirements.Add(new RequirementPrototype());
        _subject.Requirements.Add(new RequirementPrototype());

        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(AlternativeRequirementPrototype.Requirements)]?
            .Values<JObject>()
            .Should()
            .HaveCount(2);
    }
    
    [Fact]
    public void Requirements_ShouldDeserializeAsExpected()
    {
        var jsonObject = new JObject
        {
            {
                nameof(AlternativeRequirementPrototype.Requirements),
                new JArray
                {
                    new JObject(),
                    new JObject()
                }
            }
        };

        var json = jsonObject.ToString(Formatting.Indented);
        
        var requirement = JsonContext.Deserialize<AlternativeRequirementPrototype>(json);

        requirement.Requirements
            .Should()
            .HaveCount(2);
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
    public void MakeValueEqualTo_ShouldSetValuesEqual_WhenRequirementsCountIsSameAndSameType()
    {
        var requirement1 = new RequirementPrototype { Content = new EntityAtLeastRequirementPrototype() };
        var requirement2 = new RequirementPrototype { Content = new EntityExactRequirementPrototype() };
        
        _subject.Requirements.Add(requirement1);
        _subject.Requirements.Add(requirement2);

        var otherRequirement1 = new RequirementPrototype
        {
            Content = new EntityAtLeastRequirementPrototype
            {
                Entity = EntityId.New(),
                RequiredValue = 1
            }
        };

        var otherRequirement2 = new RequirementPrototype
        {
            Content = new EntityExactRequirementPrototype
            {
                Entity = EntityId.New(),
                RequiredValue = 2
            }
        };
        
        var other = new AlternativeRequirementPrototype
        {
            Requirements = 
            [
                otherRequirement1,
                otherRequirement2
            ]
        };
        
        ((IMakeValueEqual)_subject)
            .MakeValueEqualTo(other)
            .Should()
            .BeTrue();

        _subject.Requirements
            .Should()
            .HaveCount(2);

        _subject.Requirements[0]
            .ValueEquals(otherRequirement1)
            .Should()
            .BeTrue();

        _subject.Requirements[1]
            .ValueEquals(otherRequirement2)
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void MakeValueEqualTo_ShouldSetValuesEqual_WhenRequirementsCountIsSameAndDifferentType()
    {
        var requirement1 = new RequirementPrototype { Content = new EntityAtLeastRequirementPrototype() };
        var requirement2 = new RequirementPrototype { Content = new EntityExactRequirementPrototype() };
        
        _subject.Requirements.Add(requirement1);
        _subject.Requirements.Add(requirement2);

        var otherRequirement1 = new RequirementPrototype
        {
            Content = new EntityExactRequirementPrototype
            {
                Entity = EntityId.New(),
                RequiredValue = 1
            }
        };

        var otherRequirement2 = new RequirementPrototype
        {
            Content = new EntityAtLeastRequirementPrototype 
            {
                Entity = EntityId.New(),
                RequiredValue = 2
            }
        };
        
        var other = new AlternativeRequirementPrototype
        {
            Requirements =
            [
                otherRequirement1,
                otherRequirement2
            ]
        };
        
        ((IMakeValueEqual)_subject)
            .MakeValueEqualTo(other)
            .Should()
            .BeTrue();

        _subject.Requirements
            .Should()
            .HaveCount(2);

        _subject.Requirements[0]
            .ValueEquals(otherRequirement1)
            .Should()
            .BeTrue();

        _subject.Requirements[1]
            .ValueEquals(otherRequirement2)
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void MakeValueEqualTo_ShouldSetValuesEqual_WhenHasFewerRequirements()
    {
        var requirement1 = new RequirementPrototype { Content = new EntityAtLeastRequirementPrototype() };
        
        _subject.Requirements.Add(requirement1);

        var otherRequirement1 = new RequirementPrototype
        {
            Content = new EntityAtLeastRequirementPrototype
            {
                Entity = EntityId.New(),
                RequiredValue = 1
            }
        };

        var otherRequirement2 = new RequirementPrototype
        {
            Content = new EntityExactRequirementPrototype
            {
                Entity = EntityId.New(),
                RequiredValue = 2
            }
        };
        
        var other = new AlternativeRequirementPrototype
        {
            Requirements =
            [
                otherRequirement1,
                otherRequirement2
            ]
        };
        
        ((IMakeValueEqual)_subject)
            .MakeValueEqualTo(other)
            .Should()
            .BeTrue();

        _subject.Requirements
            .Should()
            .HaveCount(2);

        _subject.Requirements[0]
            .ValueEquals(otherRequirement1)
            .Should()
            .BeTrue();

        _subject.Requirements[1]
            .ValueEquals(otherRequirement2)
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void MakeValueEqualTo_ShouldSetValuesEqual_WhenHasMoreRequirements()
    {
        var requirement1 = new RequirementPrototype { Content = new EntityAtLeastRequirementPrototype() };
        var requirement2 = new RequirementPrototype { Content = new EntityExactRequirementPrototype() };
        
        _subject.Requirements.Add(requirement1);
        _subject.Requirements.Add(requirement2);

        var otherRequirement1 = new RequirementPrototype
        {
            Content = new EntityAtLeastRequirementPrototype
            {
                Entity = EntityId.New(),
                RequiredValue = 1
            }
        };
        
        var other = new AlternativeRequirementPrototype
        {
            Requirements =
            [
                otherRequirement1
            ]
        };
        
        ((IMakeValueEqual)_subject)
            .MakeValueEqualTo(other)
            .Should()
            .BeTrue();

        _subject.Requirements
            .Should()
            .HaveCount(1);

        _subject.Requirements[0]
            .ValueEquals(otherRequirement1)
            .Should()
            .BeTrue();
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
        var requirement1 = new RequirementPrototype { Content = new EntityAtLeastRequirementPrototype() };
        var requirement2 = new RequirementPrototype { Content = new EntityExactRequirementPrototype() };
        
        _subject.Requirements.Add(requirement1);
        _subject.Requirements.Add(requirement2);
        
        var clone = ((ICloneable)_subject).Clone();

        clone.Should()
            .BeOfType<AlternativeRequirementPrototype>()
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
        var requirement1 = new RequirementPrototype { Content = new EntityAtLeastRequirementPrototype() };
        var requirement2 = new RequirementPrototype { Content = new EntityExactRequirementPrototype() };
        
        _subject.Requirements.Add(requirement1);
        _subject.Requirements.Add(requirement2);
        
        var otherRequirement1 = new RequirementPrototype { Content = new EntityAtLeastRequirementPrototype() };
        var otherRequirement2 = new RequirementPrototype { Content = new EntityExactRequirementPrototype() };
        
        var other = new AlternativeRequirementPrototype
        {
            Requirements = 
            [
                otherRequirement1,
                otherRequirement2
            ]
        };
        
        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenHasFewerRequirements()
    {
        var requirement1 = new RequirementPrototype { Content = new EntityAtLeastRequirementPrototype() };
        
        _subject.Requirements.Add(requirement1);
        
        var otherRequirement1 = new RequirementPrototype { Content = new EntityAtLeastRequirementPrototype() };
        var otherRequirement2 = new RequirementPrototype { Content = new EntityExactRequirementPrototype() };
        
        var other = new AlternativeRequirementPrototype
        {
            Requirements =
            [
                otherRequirement1,
                otherRequirement2
            ]
        };
        
        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }

    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenHasMoreRequirements()
    {
        var requirement1 = new RequirementPrototype { Content = new EntityAtLeastRequirementPrototype() };
        var requirement2 = new RequirementPrototype { Content = new EntityExactRequirementPrototype() };
        
        _subject.Requirements.Add(requirement1);
        _subject.Requirements.Add(requirement2);
        
        var otherRequirement1 = new RequirementPrototype { Content = new EntityAtLeastRequirementPrototype() };
        
        var other = new RequirementPrototype
        {
            Content = new AlternativeRequirementPrototype
            {
                Requirements =
                [
                    otherRequirement1
                ]
            }
        };
        
        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }

    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenRequirementsAreDifferentTypes()
    {
        var requirement1 = new RequirementPrototype { Content = new EntityAtLeastRequirementPrototype() };
        
        _subject.Requirements.Add(requirement1);
        
        var otherRequirement1 = new RequirementPrototype { Content = new EntityExactRequirementPrototype() };
        
        var other = new AlternativeRequirementPrototype
        {
            Requirements =
            [
                otherRequirement1
            ]
        };
        
        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }

    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenRequirementsAreNotValueEqual()
    {
        var requirement1 = new RequirementPrototype
        {
            Content = new EntityAtLeastRequirementPrototype
            {
                Entity = EntityId.New(),
                RequiredValue = 1
            }
        };
        
        _subject.Requirements.Add(requirement1);

        var otherRequirement1 = new RequirementPrototype
        {
            Content = new EntityAtLeastRequirementPrototype
            {
                Entity = EntityId.New(),
                RequiredValue = 2
            }
        };
        
        var other = new AlternativeRequirementPrototype
        {
            Requirements =
            [
                otherRequirement1
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