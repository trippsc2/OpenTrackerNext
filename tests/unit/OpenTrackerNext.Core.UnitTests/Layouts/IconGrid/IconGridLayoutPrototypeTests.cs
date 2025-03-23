using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using FluentAssertions.Reactive;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTrackerNext.Core.Equality;
using OpenTrackerNext.Core.Icons;
using OpenTrackerNext.Core.Json;
using OpenTrackerNext.Core.Layouts.IconGrid;
using OpenTrackerNext.Core.Layouts.IconGrid.Static;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Layouts.IconGrid;

[ExcludeFromCodeCoverage]
public sealed class IconGridLayoutPrototypeTests
{
    private readonly IconGridLayoutPrototype _subject = new();
    
    [Fact]
    public void DocumentChanges_ShouldEmitWhenAnyPropertyChanges()
    {
        using var observer = _subject.DocumentChanges.Observe();
        
        _subject.LeftMargin = 1;
        _subject.TopMargin = 2;
        _subject.RightMargin = 3;
        _subject.BottomMargin = 4;
        _subject.HorizontalSpacing = 1;
        _subject.VerticalSpacing = 2;
        _subject.IconHeight = 3;
        _subject.IconWidth = 4;
        _subject.Rows.Add(new IconGridRowPrototype());
        _subject.Rows[0].Icons.Add(new IconLayoutPrototype());
        _subject.Rows.RemoveAt(0);

        observer.Should().Push(10);
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

        jsonObject[nameof(IconGridLayoutPrototype.LeftMargin)]?
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
            { nameof(IconGridLayoutPrototype.LeftMargin), new JValue(expected) }
        };

        var json = jsonObject.ToString(Formatting.Indented);

        var subject = JsonContext.Deserialize<IconGridLayoutPrototype>(json);

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

        jsonObject[nameof(IconGridLayoutPrototype.TopMargin)]?
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
            { nameof(IconGridLayoutPrototype.TopMargin), new JValue(expected) }
        };

        var json = jsonObject.ToString(Formatting.Indented);

        var subject = JsonContext.Deserialize<IconGridLayoutPrototype>(json);

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

        jsonObject[nameof(IconGridLayoutPrototype.RightMargin)]?
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
            { nameof(IconGridLayoutPrototype.RightMargin), new JValue(expected) }
        };

        var json = jsonObject.ToString(Formatting.Indented);

        var subject = JsonContext.Deserialize<IconGridLayoutPrototype>(json);

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

        jsonObject[nameof(IconGridLayoutPrototype.BottomMargin)]?
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
            { nameof(IconGridLayoutPrototype.BottomMargin), new JValue(expected) }
        };

        var json = jsonObject.ToString(Formatting.Indented);

        var subject = JsonContext.Deserialize<IconGridLayoutPrototype>(json);

        subject.BottomMargin
            .Should()
            .Be(expected);
    }
    
    [Fact]
    public void HorizontalSpacing_ShouldDefaultToZero()
    {
        _subject.HorizontalSpacing
            .Should()
            .Be(0);
    }
    
    [Fact]
    public void HorizontalSpacing_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.HorizontalSpacing = 1;

        monitor.Should().RaisePropertyChangeFor(x => x.HorizontalSpacing);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void HorizontalSpacing_ShouldSerializeAsExpected(int expected)
    {
        _subject.HorizontalSpacing = expected;

        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(IconGridLayoutPrototype.HorizontalSpacing)]?
            .Value<double>()
            .Should()
            .Be(expected);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void HorizontalSpacing_ShouldDeserializeAsExpected(int expected)
    {
        var jsonObject = new JObject
        {
            { nameof(IconGridLayoutPrototype.HorizontalSpacing), new JValue(expected) }
        };
        
        var json = jsonObject.ToString(Formatting.Indented);

        var subject = JsonContext.Deserialize<IconGridLayoutPrototype>(json);

        subject.HorizontalSpacing
            .Should()
            .Be(expected);
    }
    
    [Fact]
    public void VerticalSpacing_ShouldDefaultToZero()
    {
        _subject.VerticalSpacing
            .Should()
            .Be(0);
    }
    
    [Fact]
    public void VerticalSpacing_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.VerticalSpacing = 1;

        monitor.Should().RaisePropertyChangeFor(x => x.VerticalSpacing);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void VerticalSpacing_ShouldSerializeAsExpected(int expected)
    {
        _subject.VerticalSpacing = expected;

        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(IconGridLayoutPrototype.VerticalSpacing)]?
            .Value<double>()
            .Should()
            .Be(expected);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void VerticalSpacing_ShouldDeserializeAsExpected(int expected)
    {
        var jsonObject = new JObject
        {
            { nameof(IconGridLayoutPrototype.VerticalSpacing), new JValue(expected) }
        };
        
        var json = jsonObject.ToString(Formatting.Indented);

        var subject = JsonContext.Deserialize<IconGridLayoutPrototype>(json);

        subject.VerticalSpacing
            .Should()
            .Be(expected);
    }
    
    [Fact]
    public void ItemHeight_ShouldDefaultTo16()
    {
        _subject.IconHeight
            .Should()
            .Be(32);
    }
    
    [Fact]
    public void ItemHeight_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.IconHeight = 1;

        monitor.Should().RaisePropertyChangeFor(x => x.IconHeight);
    }
    
    [Theory]
    [InlineData(16)]
    [InlineData(32)]
    [InlineData(64)]
    public void ItemHeight_ShouldSerializeAsExpected(int expected)
    {
        _subject.IconHeight = expected;

        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(IconGridLayoutPrototype.IconHeight)]?
            .Value<int>()
            .Should()
            .Be(expected);
    }
    
    [Theory]
    [InlineData(16)]
    [InlineData(32)]
    [InlineData(64)]
    public void ItemHeight_ShouldDeserializeAsExpected(int expected)
    {
        var jsonObject = new JObject
        {
            { nameof(IconGridLayoutPrototype.IconHeight), new JValue(expected) }
        };
        
        var json = jsonObject.ToString(Formatting.Indented);

        var subject = JsonContext.Deserialize<IconGridLayoutPrototype>(json);

        subject.IconHeight
            .Should()
            .Be(expected);
    }
    
    [Fact]
    public void ItemWidth_ShouldDefaultTo16()
    {
        _subject.IconWidth
            .Should()
            .Be(32);
    }
    
    [Fact]
    public void ItemWidth_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.IconWidth = 1;

        monitor.Should().RaisePropertyChangeFor(x => x.IconWidth);
    }
    
    [Theory]
    [InlineData(16)]
    [InlineData(32)]
    [InlineData(64)]
    public void ItemWidth_ShouldSerializeAsExpected(int expected)
    {
        _subject.IconWidth = expected;

        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(IconGridLayoutPrototype.IconWidth)]?
            .Value<int>()
            .Should()
            .Be(expected);
    }
    
    [Theory]
    [InlineData(16)]
    [InlineData(32)]
    [InlineData(64)]
    public void ItemWidth_ShouldDeserializeAsExpected(int expected)
    {
        var jsonObject = new JObject
        {
            { nameof(IconGridLayoutPrototype.IconWidth), new JValue(expected) }
        };
        
        var json = jsonObject.ToString(Formatting.Indented);

        var subject = JsonContext.Deserialize<IconGridLayoutPrototype>(json);

        subject.IconWidth
            .Should()
            .Be(expected);
    }
    
    [Fact]
    public void Rows_ShouldDefaultToEmpty()
    {
        _subject.Rows
            .Should()
            .BeEmpty();
    }
    
    [Fact]
    public void Rows_ShouldSerializeAsExpected()
    {
        _subject.Rows.Add(new IconGridRowPrototype());
        _subject.Rows.Add(new IconGridRowPrototype());

        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(IconGridLayoutPrototype.Rows)]?
            .Value<JArray>()?
            .Should()
            .HaveCount(2);
    }
    
    [Fact]
    public void Rows_ShouldDeserializeAsExpected()
    {
        var jsonObject = new JObject
        {
            {
                nameof(IconGridLayoutPrototype.Rows), new JArray
                {
                    new JObject(),
                    new JObject()
                }
            }
        };
        
        var json = jsonObject.ToString(Formatting.Indented);

        var subject = JsonContext.Deserialize<IconGridLayoutPrototype>(json);

        subject.Rows
            .Should()
            .HaveCount(2);
    }
    
    [Fact]
    public void MakeValueEqualTo_ShouldDoNothing_WhenOtherIsReferenceEqual()
    {
        ((IMakeValueEqual) _subject)
            .MakeValueEqualTo(_subject)
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void MakeValueEqualTo_ShouldMakePropertiesValueEqual_WhenOtherIsSameType()
    {
        var other = new IconGridLayoutPrototype
        {
            HorizontalSpacing = 1,
            VerticalSpacing = 2,
            IconHeight = 3,
            IconWidth = 4,
            Rows =
            [
                new IconGridRowPrototype(),
                new IconGridRowPrototype()
            ]
        };

        ((IMakeValueEqual) _subject)
            .MakeValueEqualTo(other)
            .Should()
            .BeTrue();

        _subject.HorizontalSpacing
            .Should()
            .Be(other.HorizontalSpacing);
        
        _subject.VerticalSpacing
            .Should()
            .Be(other.VerticalSpacing);
        
        _subject.IconHeight
            .Should()
            .Be(other.IconHeight);
        
        _subject.IconWidth
            .Should()
            .Be(other.IconWidth);
        
        _subject.Rows
            .Should()
            .BeEquivalentTo(other.Rows);
    }
    
    [Fact]
    public void MakeValueEqualTo_ShouldReturnFalseAndDoNothing_WhenOtherIsDifferentType()
    {
        ((IMakeValueEqual) _subject)
            .MakeValueEqualTo(new object())
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void Clone_ShouldReturnNewEquivalentInstance()
    {
        var clone = ((ICloneable)_subject).Clone();

        clone.Should()
            .BeOfType<IconGridLayoutPrototype>()
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
    public void ValueEquals_ShouldReturnTrue_WhenOtherPropertyValuesAreEqual()
    {
        const int horizontalSpacing = 1;
        const int verticalSpacing = 2;
        const int itemHeight = 3;
        const int itemWidth = 4;
        
        _subject.HorizontalSpacing = horizontalSpacing;
        _subject.VerticalSpacing = verticalSpacing;
        _subject.IconHeight = itemHeight;
        _subject.IconWidth = itemWidth;
        
        _subject.Rows.Add(new IconGridRowPrototype());
        _subject.Rows.Add(new IconGridRowPrototype());
        
        var icon = IconId.New();

        var iconLayout = new IconLayoutPrototype
        {
            Content = new StaticIconLayoutPrototype { Icon = icon }
        };
        
        _subject.Rows[0].Icons.Add(iconLayout);
        
        var other = new IconGridLayoutPrototype
        {
            HorizontalSpacing = horizontalSpacing,
            VerticalSpacing = verticalSpacing,
            IconHeight = itemHeight,
            IconWidth = itemWidth,
            Rows =
            [
                new IconGridRowPrototype
                {
                    Icons = 
                    [
                        new IconLayoutPrototype
                        {
                            Content = new StaticIconLayoutPrototype { Icon = icon }
                        }
                    ]
                },
                new IconGridRowPrototype()
            ]
        };

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenHorizontalSpacingIsNotEqual()
    {
        _subject.HorizontalSpacing = 1;
        
        
        var other = new IconGridLayoutPrototype { HorizontalSpacing = 2 };

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenVerticalSpacingIsNotEqual()
    {
        _subject.VerticalSpacing = 1;
        
        var other = new IconGridLayoutPrototype { VerticalSpacing = 2 };

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenItemHeightIsNotEqual()
    {
        _subject.IconHeight = 1;
        
        var other = new IconGridLayoutPrototype { IconHeight = 2 };

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenItemWidthIsNotEqual()
    {
        _subject.IconWidth = 1;
        
        var other = new IconGridLayoutPrototype { IconWidth = 2 };

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenHasLessRowsThanOther()
    {
        var other = new IconGridLayoutPrototype
        {
            Rows =
            [
                new IconGridRowPrototype()
            ]
        };

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenHasMoreRowsThanOther()
    {
        _subject.Rows.Add(new IconGridRowPrototype());
        _subject.Rows.Add(new IconGridRowPrototype());
        
        var other = new IconGridLayoutPrototype
        {
            Rows =
            [
                new IconGridRowPrototype()
            ]
        };

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenRowValuesAreNotEqual()
    {
        _subject.Rows.Add(new IconGridRowPrototype());
        _subject.Rows.Add(new IconGridRowPrototype());

        var iconLayout = new IconLayoutPrototype
        {
            Content = new StaticIconLayoutPrototype { Icon = IconId.New() }
        };
        
        _subject.Rows[0].Icons.Add(iconLayout);
        
        var other = new IconGridLayoutPrototype
        {
            Rows =
            [
                new IconGridRowPrototype(),
                new IconGridRowPrototype()
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