using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using FluentAssertions.Reactive;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTrackerNext.Core.Equality;
using OpenTrackerNext.Core.Json;
using OpenTrackerNext.Core.Layouts;
using OpenTrackerNext.Core.Layouts.UniformGrid;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Layouts.UniformGrid;

[ExcludeFromCodeCoverage]
public sealed class UniformGridLayoutPrototypeTests
{
    private readonly UniformGridLayoutPrototype _subject = new();

    [Fact]
    public void DocumentChanges_ShouldEmitWhenAnyPropertyChanges()
    {
        using var observer = _subject.DocumentChanges.Observe();
        
        _subject.LeftMargin = 1;
        _subject.TopMargin = 2;
        _subject.RightMargin = 3;
        _subject.BottomMargin = 4;
        _subject.Columns = 2;
        _subject.Rows = 3;
        var item = new UniformGridMemberPrototype();
        _subject.Items.Add(item);
        item.Layout = LayoutId.New();
        _subject.Items.Remove(item);

        observer.Should().Push(9);
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

        jsonObject[nameof(UniformGridLayoutPrototype.LeftMargin)]?
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
            { nameof(UniformGridLayoutPrototype.LeftMargin), new JValue(expected) }
        };

        var json = jsonObject.ToString(Formatting.Indented);

        var subject = JsonContext.Deserialize<UniformGridLayoutPrototype>(json);

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

        jsonObject[nameof(UniformGridLayoutPrototype.TopMargin)]?
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
            { nameof(UniformGridLayoutPrototype.TopMargin), new JValue(expected) }
        };

        var json = jsonObject.ToString(Formatting.Indented);

        var subject = JsonContext.Deserialize<UniformGridLayoutPrototype>(json);

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

        jsonObject[nameof(UniformGridLayoutPrototype.RightMargin)]?
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
            { nameof(UniformGridLayoutPrototype.RightMargin), new JValue(expected) }
        };

        var json = jsonObject.ToString(Formatting.Indented);

        var subject = JsonContext.Deserialize<UniformGridLayoutPrototype>(json);

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

        jsonObject[nameof(UniformGridLayoutPrototype.BottomMargin)]?
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
            { nameof(UniformGridLayoutPrototype.BottomMargin), new JValue(expected) }
        };

        var json = jsonObject.ToString(Formatting.Indented);

        var subject = JsonContext.Deserialize<UniformGridLayoutPrototype>(json);

        subject.BottomMargin
            .Should()
            .Be(expected);
    }
    
    [Fact]
    public void Columns_ShouldDefaultToOne()
    {
        _subject.Columns
            .Should()
            .Be(1);
    }
    
    [Fact]
    public void Columns_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.Columns = 2;

        monitor.Should().RaisePropertyChangeFor(x => x.Columns);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Columns_ShouldSerializeAsExpected(int expected)
    {
        _subject.Columns = expected;

        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(UniformGridLayoutPrototype.Columns)]?
            .Value<double>()
            .Should()
            .Be(expected);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Columns_ShouldDeserializeAsExpected(int expected)
    {
        var jsonObject = new JObject
        {
            { nameof(UniformGridLayoutPrototype.Columns), new JValue(expected) }
        };
        
        var json = jsonObject.ToString(Formatting.Indented);

        var subject = JsonContext.Deserialize<UniformGridLayoutPrototype>(json);

        subject.Columns
            .Should()
            .Be(expected);
    }
    
    [Fact]
    public void Rows_ShouldDefaultToOne()
    {
        _subject.Rows
            .Should()
            .Be(1);
    }
    
    [Fact]
    public void Rows_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.Rows = 2;

        monitor.Should().RaisePropertyChangeFor(x => x.Rows);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Rows_ShouldSerializeAsExpected(int expected)
    {
        _subject.Rows = expected;

        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(UniformGridLayoutPrototype.Rows)]?
            .Value<double>()
            .Should()
            .Be(expected);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Rows_ShouldDeserializeAsExpected(int expected)
    {
        var jsonObject = new JObject
        {
            { nameof(UniformGridLayoutPrototype.Rows), new JValue(expected) }
        };
        
        var json = jsonObject.ToString(Formatting.Indented);

        var subject = JsonContext.Deserialize<UniformGridLayoutPrototype>(json);

        subject.Rows
            .Should()
            .Be(expected);
    }
    
    [Fact]
    public void Items_ShouldDefaultAsEmpty()
    {
        _subject.Items
            .Should()
            .BeEmpty();
    }

    [Fact]
    public void Items_ShouldSerializeAsExpected_WhenEmpty()
    {
        _subject.Items.Clear();
        
        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(UniformGridLayoutPrototype.Items)]?
            .Value<JArray>()
            .Should()
            .BeEmpty();
    }
    
    [Fact]
    public void Items_ShouldSerializeAsExpected_WhenNotEmpty()
    {
        _subject.Items.Add(new UniformGridMemberPrototype());
        _subject.Items.Add(new UniformGridMemberPrototype());
        
        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(UniformGridLayoutPrototype.Items)]?
            .Value<JArray>()
            .Should()
            .HaveCount(2);
    }
    
    [Fact]
    public void Items_ShouldDeserializeAsExpected_WhenEmpty()
    {
        var jsonObject = new JObject
        {
            { nameof(UniformGridLayoutPrototype.Items), new JArray() }
        };
        
        var json = jsonObject.ToString(Formatting.Indented);

        var subject = JsonContext.Deserialize<UniformGridLayoutPrototype>(json);

        subject.Items
            .Should()
            .BeEmpty();
    }
    
    [Fact]
    public void Items_ShouldDeserializeAsExpected_WhenNotEmpty()
    {
        var jsonObject = new JObject
        {
            {
                nameof(UniformGridLayoutPrototype.Items), new JArray
                {
                    new JObject(),
                    new JObject()
                }
            }
        };
        
        var json = jsonObject.ToString(Formatting.Indented);

        var subject = JsonContext.Deserialize<UniformGridLayoutPrototype>(json);

        subject.Items
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
    public void MakeValueEqualTo_ShouldMakePropertiesValueEqual_WhenOtherValueIsSameType()
    {
        var other = new UniformGridLayoutPrototype
        {
            LeftMargin = 1,
            TopMargin = 2,
            RightMargin = 3,
            BottomMargin = 4,
            Columns = 2,
            Rows = 3,
            Items =
            [
                new UniformGridMemberPrototype(),
                new UniformGridMemberPrototype()
            ]
        };
        
        ((IMakeValueEqual)_subject)
            .MakeValueEqualTo(other)
            .Should()
            .BeTrue();

        _subject.LeftMargin
            .Should()
            .Be(other.LeftMargin);
        
        _subject.TopMargin
            .Should()
            .Be(other.TopMargin);
        
        _subject.RightMargin
            .Should()
            .Be(other.RightMargin);
        
        _subject.BottomMargin
            .Should()
            .Be(other.BottomMargin);

        _subject.Columns
            .Should()
            .Be(other.Columns);

        _subject.Rows
            .Should()
            .Be(other.Rows);

        _subject.Items
            .Should()
            .BeEquivalentTo(other.Items);
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
            .BeOfType<UniformGridLayoutPrototype>()
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
        const double leftMargin = 1;
        const double topMargin = 2;
        const double rightMargin = 3;
        const double bottomMargin = 4;
        const int columns = 2;
        const int rows = 3;
        
        var member1Layout = LayoutId.New();
        var member2Layout = LayoutId.New();
        
        _subject.LeftMargin = leftMargin;
        _subject.TopMargin = topMargin;
        _subject.RightMargin = rightMargin;
        _subject.BottomMargin = bottomMargin;
        
        _subject.Columns = columns;
        _subject.Rows = rows;
        
        _subject.Items.Add(new UniformGridMemberPrototype { Layout = member1Layout });
        _subject.Items.Add(new UniformGridMemberPrototype { Layout = member2Layout });
        
        var other = new UniformGridLayoutPrototype
        {
            LeftMargin = leftMargin,
            TopMargin = topMargin,
            RightMargin = rightMargin,
            BottomMargin = bottomMargin,
            Columns = columns,
            Rows = rows,
            Items =
            [
                new UniformGridMemberPrototype { Layout = member1Layout },
                new UniformGridMemberPrototype { Layout = member2Layout }
            ]
        };
        
        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenMarginIsNotEqual()
    {
        const int columns = 2;
        const int rows = 3;
        
        var member1Layout = LayoutId.New();
        var member2Layout = LayoutId.New();
        
        _subject.LeftMargin = 1;
        _subject.TopMargin = 2;
        _subject.RightMargin = 3;
        _subject.BottomMargin = 4;
        
        _subject.Columns = columns;
        _subject.Rows = rows;
        
        _subject.Items.Add(new UniformGridMemberPrototype { Layout = member1Layout });
        _subject.Items.Add(new UniformGridMemberPrototype { Layout = member2Layout });
        
        var other = new UniformGridLayoutPrototype
        {
            LeftMargin = 5,
            TopMargin = 6,
            RightMargin = 7,
            BottomMargin = 8,
            Columns = columns,
            Rows = rows,
            Items =
            [
                new UniformGridMemberPrototype { Layout = member1Layout },
                new UniformGridMemberPrototype { Layout = member2Layout }
            ]
        };
        
        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }
    
        
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenColumnsIsNotEqual()
    {
        const double leftMargin = 1;
        const double topMargin = 2;
        const double rightMargin = 3;
        const double bottomMargin = 4;
        const int rows = 3;
        
        var member1Layout = LayoutId.New();
        var member2Layout = LayoutId.New();
        
        _subject.LeftMargin = leftMargin;
        _subject.TopMargin = topMargin;
        _subject.RightMargin = rightMargin;
        _subject.BottomMargin = bottomMargin;
        
        _subject.Columns = 2;
        _subject.Rows = rows;
        
        _subject.Items.Add(new UniformGridMemberPrototype { Layout = member1Layout });
        _subject.Items.Add(new UniformGridMemberPrototype { Layout = member2Layout });
        
        var other = new UniformGridLayoutPrototype
        {
            LeftMargin = leftMargin,
            TopMargin = topMargin,
            RightMargin = rightMargin,
            BottomMargin = bottomMargin,
            Columns = 4,
            Rows = rows,
            Items =
            [
                new UniformGridMemberPrototype { Layout = member1Layout },
                new UniformGridMemberPrototype { Layout = member2Layout }
            ]
        };
        
        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }
    
        
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenRowsIsNotEqual()
    {
        const double leftMargin = 1;
        const double topMargin = 2;
        const double rightMargin = 3;
        const double bottomMargin = 4;
        const int columns = 2;

        var member1Layout = LayoutId.New();
        var member2Layout = LayoutId.New();

        _subject.LeftMargin = leftMargin;
        _subject.TopMargin = topMargin;
        _subject.RightMargin = rightMargin;
        _subject.BottomMargin = bottomMargin;
        
        _subject.Columns = columns;
        _subject.Rows = 3;
        
        _subject.Items.Add(new UniformGridMemberPrototype { Layout = member1Layout });
        _subject.Items.Add(new UniformGridMemberPrototype { Layout = member2Layout });
        
        var other = new UniformGridLayoutPrototype
        {
            LeftMargin = leftMargin,
            TopMargin = topMargin,
            RightMargin = rightMargin,
            BottomMargin = bottomMargin,
            Columns = columns,
            Rows = 4,
            Items =
            [
                new UniformGridMemberPrototype { Layout = member1Layout },
                new UniformGridMemberPrototype { Layout = member2Layout }
            ]
        };
        
        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenOtherHasFewerItems()
    {
        const double leftMargin = 1;
        const double topMargin = 2;
        const double rightMargin = 3;
        const double bottomMargin = 4;
        const int columns = 2;
        const int rows = 3;
        
        var member1Layout = LayoutId.New();
        var member2Layout = LayoutId.New();

        _subject.LeftMargin = leftMargin;
        _subject.TopMargin = topMargin;
        _subject.RightMargin = rightMargin;
        _subject.BottomMargin = bottomMargin;
        
        _subject.Columns = columns;
        _subject.Rows = rows;
        
        _subject.Items.Add(new UniformGridMemberPrototype { Layout = member1Layout });
        _subject.Items.Add(new UniformGridMemberPrototype { Layout = member2Layout });
        
        var other = new UniformGridLayoutPrototype
        {
            LeftMargin = leftMargin,
            TopMargin = topMargin,
            RightMargin = rightMargin,
            BottomMargin = bottomMargin,
            Columns = columns,
            Rows = rows,
            Items =
            [
                new UniformGridMemberPrototype { Layout = member1Layout }
            ]
        };
        
        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenOtherHasMoreItems()
    {
        const double leftMargin = 1;
        const double topMargin = 2;
        const double rightMargin = 3;
        const double bottomMargin = 4;
        const int columns = 2;
        const int rows = 3;
        
        var member1Layout = LayoutId.New();
        var member2Layout = LayoutId.New();

        _subject.LeftMargin = leftMargin;
        _subject.TopMargin = topMargin;
        _subject.RightMargin = rightMargin;
        _subject.BottomMargin = bottomMargin;
        _subject.Columns = columns;
        _subject.Rows = rows;
        
        _subject.Items.Add(new UniformGridMemberPrototype { Layout = member1Layout });
        
        var other = new UniformGridLayoutPrototype
        {
            LeftMargin = leftMargin,
            TopMargin = topMargin,
            RightMargin = rightMargin,
            BottomMargin = bottomMargin,
            Columns = columns,
            Rows = rows,
            Items =
            [
                new UniformGridMemberPrototype { Layout = member1Layout },
                new UniformGridMemberPrototype { Layout = member2Layout }
            ]
        };
        
        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenOtherItemsAreDifferent()
    {
        const double leftMargin = 1;
        const double topMargin = 2;
        const double rightMargin = 3;
        const double bottomMargin = 4;
        const int columns = 2;
        const int rows = 3;


        _subject.LeftMargin = leftMargin;
        _subject.TopMargin = topMargin;
        _subject.RightMargin = rightMargin;
        _subject.BottomMargin = bottomMargin;
        
        _subject.Columns = columns;
        _subject.Rows = rows;
        
        var subjectMember1Layout = LayoutId.New();
        var subjectMember2Layout = LayoutId.New();

        _subject.Items.Add(new UniformGridMemberPrototype { Layout = subjectMember1Layout });
        _subject.Items.Add(new UniformGridMemberPrototype { Layout = subjectMember2Layout });
        
        var otherMember1Layout = LayoutId.New();
        var otherMember2Layout = LayoutId.New();
        
        var other = new UniformGridLayoutPrototype
        {
            LeftMargin = leftMargin,
            TopMargin = topMargin,
            RightMargin = rightMargin,
            BottomMargin = bottomMargin,
            Columns = columns,
            Rows = rows,
            Items =
            [
                new UniformGridMemberPrototype { Layout = otherMember1Layout },
                new UniformGridMemberPrototype { Layout = otherMember2Layout }
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