using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using FluentAssertions.Reactive;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTrackerNext.Core.Equality;
using OpenTrackerNext.Core.Json;
using OpenTrackerNext.Core.Requirements.UIPanel;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Requirements.UIPanel;

[ExcludeFromCodeCoverage]
public sealed class UIPanelDockRequirementPrototypeTests
{
    private readonly UIPanelDockRequirementPrototype _subject = new();
    
    [Fact]
    public void DocumentChanges_ShouldEmitWhenPropertyChanges()
    {
        using var observer = _subject.DocumentChanges.Observe();
        
        _subject.UIPanelDock = UIPanelDock.Bottom;

        observer.Should().Push(1);
    }
    
    [Fact]
    public void UIPanelDock_ShouldDefaultToLeft()
    {
        _subject.UIPanelDock
            .Should()
            .Be(UIPanelDock.Left);
    }
    
    [Fact]
    public void UIPanelDock_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.UIPanelDock = UIPanelDock.Bottom;
        
        monitor.Should().RaisePropertyChangeFor(x => x.UIPanelDock);
    }
    
    [Fact]
    public void UIPanelDock_ShouldSerializeAsExpected()
    {
        _subject.UIPanelDock = UIPanelDock.Bottom;
        
        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(UIPanelDockRequirementPrototype.UIPanelDock)]?
            .Value<string>()
            .Should()
            .Be(nameof(UIPanelDock.Bottom));
    }
    
    [Fact]
    public void UIPanelDock_ShouldDeserializeAsExpected()
    {
        var jsonObject = new JObject
        {
            { nameof(UIPanelDockRequirementPrototype.UIPanelDock), new JValue(nameof(UIPanelDock.Bottom)) }
        };

        var json = jsonObject.ToString(Formatting.Indented);

        var subject = JsonContext.Deserialize<UIPanelDockRequirementPrototype>(json);
        
        subject.UIPanelDock
            .Should()
            .Be(UIPanelDock.Bottom);
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
        var other = new UIPanelDockRequirementPrototype
        {
            UIPanelDock = UIPanelDock.Bottom
        };
        
        ((IMakeValueEqual)_subject)
            .MakeValueEqualTo(other)
            .Should()
            .BeTrue();
        
        _subject.UIPanelDock
            .Should()
            .Be(other.UIPanelDock);
    }
    
    [Fact]
    public void MakeValueEqualTo_ShouldReturnFalse_WhenOtherIsDifferentType()
    {
        ((IMakeValueEqual)_subject)
            .MakeValueEqualTo(new object())
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void Clone_ShouldReturnNewEquivalentInstance()
    {
        _subject.UIPanelDock = UIPanelDock.Bottom;
        
        var clone = ((ICloneable)_subject).Clone();
        
        clone.Should()
            .BeOfType<UIPanelDockRequirementPrototype>()
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
        _subject.UIPanelDock = UIPanelDock.Bottom;
        
        var other = new UIPanelDockRequirementPrototype
        {
            UIPanelDock = UIPanelDock.Bottom
        };
        
        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenOtherIsNotValueEqual()
    {
        _subject.UIPanelDock = UIPanelDock.Bottom;
        
        var other = new UIPanelDockRequirementPrototype
        {
            UIPanelDock = UIPanelDock.Left
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