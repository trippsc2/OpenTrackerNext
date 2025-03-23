using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using FluentAssertions.Reactive;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTrackerNext.Core.Equality;
using OpenTrackerNext.Core.Ids;
using OpenTrackerNext.Core.Json;
using OpenTrackerNext.Core.Layouts;
using OpenTrackerNext.Core.Requirements;
using OpenTrackerNext.Core.Requirements.Entity.Exact;
using OpenTrackerNext.Core.UIPanes;
using OpenTrackerNext.Core.UIPanes.Popup;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.UIPanes;

[ExcludeFromCodeCoverage]
public sealed class UIPanePrototypeTests
{
    private readonly UIPanePrototype _subject = new();

    [Fact]
    public void TitlePrefix_ShouldReturnExpected()
    {
        UIPanePrototype.TitlePrefix
            .Should()
            .Be(UIPanePrototype.UIPaneTitlePrefix);
    }
    
    [Fact]
    public void DocumentChanges_ShouldEmitWhenPropertiesChange()
    {
        using var observer = _subject.DocumentChanges.Observe();

        _subject.Visibility.Content = new EntityExactRequirementPrototype();
        var popup = new UIPanePopupPrototype();
        _subject.Popup = popup;
        popup.Body = LayoutId.New();
        _subject.Title = "Test";
        _subject.Body = LayoutId.New();
        
        observer.Should().Push(5);
    }
    
    [Fact]
    public void Visibility_ShouldDefaultToDefaultRequirement()
    {
        _subject.Visibility
            .Content
            .Should()
            .BeOfType<NullRequirementPrototype>();
    }
    
    [Fact]
    public void Visibility_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.Visibility = new RequirementPrototype();
        
        monitor.Should().RaisePropertyChangeFor(x => x.Visibility);
    }

    [Fact]
    public void Visibility_ShouldSerializeAsExpected()
    {
        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(UIPanePrototype.Visibility)]
            .Should()
            .BeOfType<JObject>();
    }
    
    [Fact]
    public void Visibility_ShouldDeserializeAsExpected()
    {
        var jsonObject = new JObject
        {
            { nameof(UIPanePrototype.Visibility), new JObject() }
        };
        
        var json = jsonObject.ToString(Formatting.Indented);
        
        var subject = JsonContext.Deserialize<UIPanePrototype>(json);

        subject.Visibility
            .Should()
            .BeOfType<RequirementPrototype>();
    }
    
    [Fact]
    public void Popup_ShouldDefaultToNullUIPanePopupPrototype()
    {
        _subject.Popup
            .Should()
            .BeOfType<NullUIPanePopupPrototype>();
    }
    
    [Fact]
    public void Popup_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.Popup = new UIPanePopupPrototype();
        
        monitor.Should().RaisePropertyChangeFor(x => x.Popup);
    }
    
    [Theory]
    [InlineData("null", typeof(NullUIPanePopupPrototype))]
    [InlineData("default", typeof(UIPanePopupPrototype))]
    public void Popup_ShouldSerializeAsExpected(string expected, Type popupType)
    {
        _subject.Popup = (IUIPanePopupPrototype)Activator.CreateInstance(popupType)!;
        
        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(UIPanePrototype.Popup)]?
            .Value<JObject>()?["$type"]?
            .Value<string>()
            .Should()
            .Be(expected);
    }
    
    [Theory]
    [InlineData(typeof(NullUIPanePopupPrototype), "null")]
    [InlineData(typeof(UIPanePopupPrototype), "default")]
    public void Popup_ShouldDeserializeAsExpected(Type expected, string typeDiscriminator)
    {
        var jsonObject = new JObject
        {
            {
                nameof(UIPanePrototype.Popup), new JObject
                {
                    { "$type", new JValue(typeDiscriminator) }
                }
            }
        };
        
        var json = jsonObject.ToString(Formatting.Indented);
        
        var subject = JsonContext.Deserialize<UIPanePrototype>(json);

        subject.Popup
            .Should()
            .BeOfType(expected);
    }
    
    [Fact]
    public void Title_ShouldDefaultToEmpty()
    {
        _subject.Title
            .Should()
            .BeEmpty();
    }
    
    [Fact]
    public void Title_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.Title = "Test";
        
        monitor.Should().RaisePropertyChangeFor(x => x.Title);
    }
    
    [Theory]
    [InlineData("test")]
    [InlineData("test2")]
    public void Title_ShouldSerializeAsExpected(string expected)
    {
        _subject.Title = expected;
        
        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(UIPanePrototype.Title)]?
            .Value<string>()
            .Should()
            .Be(expected);
    }
    
    [Theory]
    [InlineData("test")]
    [InlineData("test2")]
    public void Title_ShouldDeserializeAsExpected(string expected)
    {
        var jsonObject = new JObject
        {
            { nameof(UIPanePrototype.Title), new JValue(expected) }
        };
        
        var json = jsonObject.ToString(Formatting.Indented);
        
        var subject = JsonContext.Deserialize<UIPanePrototype>(json);

        subject.Title
            .Should()
            .Be(expected);
    }
    
    [Fact]
    public void Body_ShouldDefaultToNull()
    {
        _subject.Body
            .Should()
            .Be(LayoutId.Empty);
    }
    
    [Fact]
    public void Body_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.Body = LayoutId.New();
        
        monitor.Should().RaisePropertyChangeFor(x => x.Body);
    }
    
    [Fact]
    public void Body_ShouldSerializeAsExpected()
    {
        _subject.Body = LayoutId.New();
        
        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(UIPanePrototype.Body)]?
            .Value<string>()
            .Should()
            .Be(_subject.Body.ToString());
    }
    
    [Fact]
    public void Body_ShouldDeserializeAsExpected()
    {
        var expected = LayoutId.New();
        
        var jsonObject = new JObject
        {
            { nameof(UIPanePrototype.Body), new JValue(expected.ToLowercaseString()) }
        };
        
        var json = jsonObject.ToString(Formatting.Indented);
        
        var subject = JsonContext.Deserialize<UIPanePrototype>(json);

        subject.Body
            .Should()
            .Be(expected);
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
    public void MakeValueEqualTo_ShouldSetPropertyValues_WhenOtherIsSameType()
    {
        var popupBody = LayoutId.New();
        
        var other = new UIPanePrototype
        {
            Visibility = new RequirementPrototype
            {
                Content = new EntityExactRequirementPrototype()
            },
            Popup = new UIPanePopupPrototype
            {
                Body = popupBody
            },
            Title = "Test",
            Body = LayoutId.New()
        };

        ((IMakeValueEqual)_subject)
            .MakeValueEqualTo(other)
            .Should()
            .BeTrue();

        _subject.Visibility
            .Content
            .Should()
            .BeOfType<EntityExactRequirementPrototype>();

        _subject.Popup
            .Should()
            .BeOfType<UIPanePopupPrototype>()
            .Subject
            .Body
            .Should()
            .Be(popupBody);

        _subject.Title
            .Should()
            .Be(other.Title);

        _subject.Body
            .Should()
            .Be(other.Body);
    }

    [Fact]
    public void MakeValueEqualTo_ShouldDoNothingAndReturnFalse_WhenOtherIsDifferentType()
    {
        ((IMakeValueEqual)_subject)
            .MakeValueEqualTo(new object())
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void Clone_ShouldReturnNewEquivalentInstance()
    {
        _subject.Visibility = new RequirementPrototype
        {
            Content = new EntityExactRequirementPrototype()
        };
        
        _subject.Popup = new UIPanePopupPrototype { Body = LayoutId.New() };
        _subject.Title = "Test";
        _subject.Body = LayoutId.New();

        var clone = ((ICloneable)_subject).Clone();

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
        const string title = "Test";
        
        var popupBody = LayoutId.New();
        var body = LayoutId.New();
        
        _subject.Visibility = new RequirementPrototype
        {
            Content = new EntityExactRequirementPrototype()
        };
        
        _subject.Popup = new UIPanePopupPrototype { Body = popupBody };
        _subject.Title = title;
        _subject.Body = body;
        
        var other = new UIPanePrototype
        {
            Visibility = new RequirementPrototype
            {
                Content = new EntityExactRequirementPrototype()
            },
            Popup = new UIPanePopupPrototype
            {
                Body = popupBody
            },
            Title = title,
            Body = body
        };

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenVisibilityIsNotEqual()
    {
        const string title = "Test";
        
        var popupBody = LayoutId.New();
        var body = LayoutId.New();
        
        _subject.Visibility = new RequirementPrototype
        {
            Content = new NullRequirementPrototype()
        };
        
        _subject.Popup = new UIPanePopupPrototype { Body = popupBody };
        _subject.Title = title;
        _subject.Body = body;
        
        var other = new UIPanePrototype
        {
            Visibility = new RequirementPrototype
            {
                Content = new EntityExactRequirementPrototype()
            },
            Popup = new UIPanePopupPrototype
            {
                Body = popupBody
            },
            Title = title,
            Body = body
        };

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenPopupIsNotEqual()
    {
        const string title = "Test";
        
        var body = LayoutId.New();
        
        _subject.Visibility = new RequirementPrototype
        {
            Content = new EntityExactRequirementPrototype()
        };

        _subject.Popup = new NullUIPanePopupPrototype();
        _subject.Title = title;
        _subject.Body = body;
        
        var other = new UIPanePrototype
        {
            Visibility = new RequirementPrototype
            {
                Content = new EntityExactRequirementPrototype()
            },
            Popup = new UIPanePopupPrototype
            {
                Body = LayoutId.New()
            },
            Title = title,
            Body = body
        };

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenTitleIsNotEqual()
    {
        var popupBody = LayoutId.New();
        var body = LayoutId.New();
        
        _subject.Visibility = new RequirementPrototype
        {
            Content = new EntityExactRequirementPrototype()
        };
        
        _subject.Popup = new UIPanePopupPrototype { Body = popupBody };
        _subject.Title = "Test";
        _subject.Body = body;
        
        var other = new UIPanePrototype
        {
            Visibility = new RequirementPrototype
            {
                Content = new EntityExactRequirementPrototype()
            },
            Popup = new UIPanePopupPrototype
            {
                Body = popupBody
            },
            Title = "Test2",
            Body = body
        };

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenBodyIsNotEqual()
    {
        const string title = "Test";
        
        var popupBody = LayoutId.New();
        
        _subject.Visibility = new RequirementPrototype
        {
            Content = new EntityExactRequirementPrototype()
        };
        
        _subject.Popup = new UIPanePopupPrototype { Body = popupBody };
        _subject.Title = title;
        _subject.Body = LayoutId.New();
        
        var other = new UIPanePrototype
        {
            Visibility = new RequirementPrototype
            {
                Content = new EntityExactRequirementPrototype()
            },
            Popup = new UIPanePopupPrototype
            {
                Body = popupBody
            },
            Title = title,
            Body = LayoutId.New()
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