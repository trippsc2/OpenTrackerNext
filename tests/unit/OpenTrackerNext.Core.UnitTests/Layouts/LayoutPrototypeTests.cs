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
using OpenTrackerNext.Core.Layouts.IconGrid;
using OpenTrackerNext.Core.Layouts.UniformGrid;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Layouts;

[ExcludeFromCodeCoverage]
public sealed class LayoutPrototypeTests
{
    private readonly LayoutPrototype _subject = new();
    
    [Fact]
    public void TitlePrefix_ShouldReturnExpected()
    {
        LayoutPrototype.TitlePrefix
            .Should()
            .Be(LayoutPrototype.LayoutTitlePrefix);
    }
    
    [Fact]
    public void DocumentChanges_ShouldEmitWhenAnyPropertyChanges()
    {
        using var observer = _subject.DocumentChanges.Observe();
        
        var iconGridLayout = new IconGridLayoutPrototype();
        _subject.Content = iconGridLayout;
        iconGridLayout.HorizontalSpacing = 1;
        
        observer.Should().Push(2);
    }
    
    [Fact]
    public void Content_ShouldInitializeToIconGridLayout()
    {
        _subject.Content
            .Should()
            .BeOfType<IconGridLayoutPrototype>();
    }
    
    [Fact]
    public void Content_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.Content = new IconGridLayoutPrototype();
        
        monitor.Should().RaisePropertyChangeFor(x => x.Content);
    }
    
    [Theory]
    [InlineData("dynamic", typeof(DynamicLayoutPrototype))]
    [InlineData("icon_grid", typeof(IconGridLayoutPrototype))]
    [InlineData("uniform_grid", typeof(UniformGridLayoutPrototype))]
    public void Content_ShouldSerializeAsExpected(string expected, Type contentType)
    {
        _subject.Content = (ILayoutSubtypePrototype)Activator.CreateInstance(contentType)!;
        
        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(LayoutPrototype.Content)]?
            .Value<JObject>()?["$type"]?
            .Value<string>()
            .Should()
            .Be(expected);
    }
    
    [Theory]
    [InlineData(typeof(DynamicLayoutPrototype), "dynamic")]
    [InlineData(typeof(IconGridLayoutPrototype), "icon_grid")]
    [InlineData(typeof(UniformGridLayoutPrototype), "uniform_grid")]
    public void Content_ShouldDeserializeAsExpected_WhenIconGridLayoutPrototype(Type expected, string typeDiscriminator)
    {
        var jsonObject = new JObject
        {
            { nameof(LayoutPrototype.Content), new JObject
                {
                    { "$type", new JValue(typeDiscriminator) }
                }
            }
        };
        
        var json = jsonObject.ToString(Formatting.Indented);
        
        var subject = JsonContext.Deserialize<LayoutPrototype>(json);
        
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
    public void MakeValueEqualTo_ShouldSetPropertyValues()
    {
        const int horizontalSpacing = 1;
        const int verticalSpacing = 2;
        
        var other = new LayoutPrototype
        {
            Content = new IconGridLayoutPrototype
            {
                HorizontalSpacing = horizontalSpacing,
                VerticalSpacing = verticalSpacing
            }
        };
        
        ((IMakeValueEqual)_subject)
            .MakeValueEqualTo(other)
            .Should()
            .BeTrue();

        _subject.Content
            .Should()
            .BeOfType<IconGridLayoutPrototype>();

        _subject.Content
            .As<IconGridLayoutPrototype>()
            .HorizontalSpacing
            .Should()
            .Be(horizontalSpacing);

        _subject.Content
            .As<IconGridLayoutPrototype>()
            .VerticalSpacing
            .Should()
            .Be(verticalSpacing);
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
        var clone = ((ICloneable)_subject).Clone();
        
        clone.Should()
            .BeOfType<LayoutPrototype>()
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
    public void ValueEquals_ShouldReturnTrue_WhenOtherIsEquivalent()
    {
        const int horizontalSpacing = 1;
        const int verticalSpacing = 2;
        
        _subject.Content = new IconGridLayoutPrototype
        {
            HorizontalSpacing = horizontalSpacing,
            VerticalSpacing = verticalSpacing
        };
        
        var other = new LayoutPrototype
        {
            Content = new IconGridLayoutPrototype
            {
                HorizontalSpacing = horizontalSpacing,
                VerticalSpacing = verticalSpacing
            }
        };

        ((IValueEquatable) _subject)
            .ValueEquals(other)
            .Should()
            .BeTrue();
    }

    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenContentTypeIsDifferent()
    {
        _subject.Content = new IconGridLayoutPrototype
        {
            HorizontalSpacing = 1,
            VerticalSpacing = 2
        };
        
        var other = new LayoutPrototype { Content = new UniformGridLayoutPrototype() };

        ((IValueEquatable) _subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenContentValueIsDifferent()
    {
        const int verticalSpacing = 2;
        
        _subject.Content = new IconGridLayoutPrototype
        {
            HorizontalSpacing = 1,
            VerticalSpacing = verticalSpacing
        };
        
        var other = new LayoutPrototype
        {
            Content = new IconGridLayoutPrototype { VerticalSpacing = verticalSpacing }
        };

        ((IValueEquatable) _subject)
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