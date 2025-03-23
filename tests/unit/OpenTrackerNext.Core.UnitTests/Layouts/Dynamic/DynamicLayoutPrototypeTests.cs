using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using FluentAssertions.Reactive;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTrackerNext.Core.Equality;
using OpenTrackerNext.Core.Json;
using OpenTrackerNext.Core.Layouts;
using OpenTrackerNext.Core.Layouts.Dynamic;
using OpenTrackerNext.Core.Requirements;
using OpenTrackerNext.Core.Requirements.Aggregate;
using OpenTrackerNext.Core.Requirements.Alternative;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Layouts.Dynamic;

[ExcludeFromCodeCoverage]
public sealed class DynamicLayoutPrototypeTests
{
    private readonly DynamicLayoutPrototype _subject = new();

    [Fact]
    public void DocumentChanges_ShouldEmitWhenPropertyChanges()
    {
        using var observer = _subject.DocumentChanges.Observe();

        _subject.LeftMargin = 1;
        _subject.TopMargin = 2;
        _subject.RightMargin = 3;
        _subject.BottomMargin = 4;
        _subject.Requirement.Content = new AggregateRequirementPrototype();
        _subject.MetLayout = LayoutId.New();
        _subject.UnmetLayout = LayoutId.New();

        observer.Should().Push(7);
    }
    
    [Fact]
    public void LeftMargin_ShouldDefaultToZero()
    {
        _subject.LeftMargin
            .Should()
            .Be(0);
    }

    [Fact]
    public void LeftMargin_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.LeftMargin = 1;
        
        monitor.Should().RaisePropertyChangeFor(x => x.LeftMargin);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void LeftMargin_ShouldSerializeAsExpected(double expected)
    {
        _subject.LeftMargin = expected;

        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(DynamicLayoutPrototype.LeftMargin)]?
            .Value<double>()
            .Should()
            .Be(expected);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void LeftMargin_ShouldDeserializeAsExpected(double expected)
    {
        var jsonObject = new JObject
        {
            { nameof(DynamicLayoutPrototype.LeftMargin), new JValue(expected) }
        };

        var json = jsonObject.ToString(Formatting.Indented);

        var subject = JsonContext.Deserialize<DynamicLayoutPrototype>(json);

        subject.LeftMargin
            .Should()
            .Be(expected);
    }
        
    [Fact]
    public void TopMargin_ShouldDefaultToZero()
    {
        _subject.TopMargin
            .Should()
            .Be(0);
    }
    
    [Fact]
    public void TopMargin_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.TopMargin = 1;
        
        monitor.Should().RaisePropertyChangeFor(x => x.TopMargin);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void TopMargin_ShouldSerializeAsExpected(double expected)
    {
        _subject.TopMargin = expected;

        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(DynamicLayoutPrototype.TopMargin)]?
            .Value<double>()
            .Should()
            .Be(expected);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void TopMargin_ShouldDeserializeAsExpected(double expected)
    {
        var jsonObject = new JObject
        {
            { nameof(DynamicLayoutPrototype.TopMargin), new JValue(expected) }
        };

        var json = jsonObject.ToString(Formatting.Indented);

        var subject = JsonContext.Deserialize<DynamicLayoutPrototype>(json);

        subject.TopMargin
            .Should()
            .Be(expected);
    }

    [Fact]
    public void RightMargin_ShouldDefaultToZero()
    {
        _subject.RightMargin
            .Should()
            .Be(0);
    }
    
    [Fact]
    public void RightMargin_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.RightMargin = 1;
        
        monitor.Should().RaisePropertyChangeFor(x => x.RightMargin);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void RightMargin_ShouldSerializeAsExpected(double expected)
    {
        _subject.RightMargin = expected;

        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(DynamicLayoutPrototype.RightMargin)]?
            .Value<double>()
            .Should()
            .Be(expected);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void RightMargin_ShouldDeserializeAsExpected(double expected)
    {
        var jsonObject = new JObject
        {
            { nameof(DynamicLayoutPrototype.RightMargin), new JValue(expected) }
        };

        var json = jsonObject.ToString(Formatting.Indented);

        var subject = JsonContext.Deserialize<DynamicLayoutPrototype>(json);

        subject.RightMargin
            .Should()
            .Be(expected);
    }

    [Fact]
    public void BottomMargin_ShouldDefaultToZero()
    {
        _subject.BottomMargin
            .Should()
            .Be(0);
    }

    [Fact]
    public void BottomMargin_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.BottomMargin = 1;
        
        monitor.Should().RaisePropertyChangeFor(x => x.BottomMargin);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void BottomMargin_ShouldSerializeAsExpected(double expected)
    {
        _subject.BottomMargin = expected;

        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(DynamicLayoutPrototype.BottomMargin)]?
            .Value<double>()
            .Should()
            .Be(expected);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void BottomMargin_ShouldDeserializeAsExpected(double expected)
    {
        var jsonObject = new JObject
        {
            { nameof(DynamicLayoutPrototype.BottomMargin), new JValue(expected) }
        };

        var json = jsonObject.ToString(Formatting.Indented);

        var subject = JsonContext.Deserialize<DynamicLayoutPrototype>(json);

        subject.BottomMargin
            .Should()
            .Be(expected);
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

        jsonObject[nameof(DynamicLayoutPrototype.Requirement)]?
            .Value<JObject>()
            .Should()
            .NotBeNull();
    }
    
    [Fact]
    public void Requirement_ShouldDeserializeAsExpected()
    {
        var jsonObject = new JObject
        {
            { nameof(DynamicLayoutPrototype.Requirement), new JObject() }
        };

        var json = jsonObject.ToString(Formatting.Indented);

        var subject = JsonContext.Deserialize<DynamicLayoutPrototype>(json);

        subject.Requirement
            .Content
            .Should()
            .BeOfType<NullRequirementPrototype>();
    }
    
    [Fact]
    public void MetLayout_ShouldDefaultToEmpty()
    {
        _subject.MetLayout
            .Should()
            .Be(LayoutId.Empty);
    }
    
    [Fact]
    public void MetLayout_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.MetLayout = LayoutId.New();
        
        monitor.Should().RaisePropertyChangeFor(x => x.MetLayout);
    }
    
    [Fact]
    public void MetLayout_ShouldSerializeAsExpected()
    {
        _subject.MetLayout = LayoutId.New();
        
        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(DynamicLayoutPrototype.MetLayout)]?
            .Value<string>()
            .Should()
            .Be(_subject.MetLayout.Value.ToString());
    }
    
    [Fact]
    public void MetLayout_ShouldDeserializeAsExpected()
    {
        var metLayout = LayoutId.New();
        
        var jsonObject = new JObject
        {
            { nameof(DynamicLayoutPrototype.MetLayout), new JValue(metLayout.Value.ToString()) }
        };

        var json = jsonObject.ToString(Formatting.Indented);

        var subject = JsonContext.Deserialize<DynamicLayoutPrototype>(json);

        subject.MetLayout
            .Should()
            .Be(metLayout);
    }
    
    [Fact]
    public void UnmetLayout_ShouldDefaultToEmpty()
    {
        _subject.UnmetLayout
            .Should()
            .Be(LayoutId.Empty);
    }
    
    [Fact]
    public void UnmetLayout_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.UnmetLayout = LayoutId.New();
        
        monitor.Should().RaisePropertyChangeFor(x => x.UnmetLayout);
    }
    
    [Fact]
    public void UnmetLayout_ShouldSerializeAsExpected()
    {
        _subject.UnmetLayout = LayoutId.New();
        
        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(DynamicLayoutPrototype.UnmetLayout)]?
            .Value<string>()
            .Should()
            .Be(_subject.UnmetLayout.Value.ToString());
    }
    
    [Fact]
    public void UnmetLayout_ShouldDeserializeAsExpected()
    {
        var unmetLayout = LayoutId.New();
        
        var jsonObject = new JObject
        {
            { nameof(DynamicLayoutPrototype.UnmetLayout), new JValue(unmetLayout.Value.ToString()) }
        };

        var json = jsonObject.ToString(Formatting.Indented);

        var subject = JsonContext.Deserialize<DynamicLayoutPrototype>(json);

        subject.UnmetLayout
            .Should()
            .Be(unmetLayout);
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
        var other = new DynamicLayoutPrototype
        {
            Requirement = new RequirementPrototype { Content = new AggregateRequirementPrototype() },
            MetLayout = LayoutId.New(),
            UnmetLayout = LayoutId.New()
        };
        
        ((IMakeValueEqual)_subject)
            .MakeValueEqualTo(other)
            .Should()
            .BeTrue();

        _subject.Requirement
            .Content
            .Should()
            .BeOfType<AggregateRequirementPrototype>();

        _subject.MetLayout
            .Should()
            .Be(other.MetLayout);

        _subject.UnmetLayout
            .Should()
            .Be(other.UnmetLayout);
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
        _subject.MetLayout = LayoutId.New();
        _subject.UnmetLayout = LayoutId.New();
        
        var clone = ((ICloneable)_subject).Clone();

        clone.Should()
            .BeOfType<DynamicLayoutPrototype>()
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
        _subject.Requirement.Content = new AggregateRequirementPrototype();
        _subject.MetLayout = LayoutId.New();
        _subject.UnmetLayout = LayoutId.New();
        
        var other = new DynamicLayoutPrototype
        {
            Requirement = new RequirementPrototype
            {
                Content = new AggregateRequirementPrototype()
            },
            MetLayout = _subject.MetLayout,
            UnmetLayout = _subject.UnmetLayout
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
        _subject.MetLayout = LayoutId.New();
        _subject.UnmetLayout = LayoutId.New();
        
        var other = new DynamicLayoutPrototype
        {
            Requirement = new RequirementPrototype { Content = new AlternativeRequirementPrototype() },
            MetLayout = _subject.MetLayout,
            UnmetLayout = _subject.UnmetLayout
        };
        
        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenMetLayoutIsNotEqual()
    {
        _subject.Requirement.Content = new AggregateRequirementPrototype();
        _subject.MetLayout = LayoutId.New();
        _subject.UnmetLayout = LayoutId.New();
        
        var other = new DynamicLayoutPrototype
        {
            Requirement = new RequirementPrototype { Content = new AggregateRequirementPrototype() },
            MetLayout = LayoutId.New(),
            UnmetLayout = _subject.UnmetLayout
        };
        
        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenUnmetLayoutIsNotEqual()
    {
        _subject.Requirement.Content = new AggregateRequirementPrototype();
        _subject.MetLayout = LayoutId.New();
        _subject.UnmetLayout = LayoutId.New();
        
        var other = new DynamicLayoutPrototype
        {
            Requirement = new RequirementPrototype { Content = new AggregateRequirementPrototype() },
            MetLayout = _subject.MetLayout,
            UnmetLayout = LayoutId.New()
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