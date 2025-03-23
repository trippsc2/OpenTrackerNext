using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using FluentAssertions.Reactive;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTrackerNext.Core.Equality;
using OpenTrackerNext.Core.Json;
using OpenTrackerNext.Core.MapLayouts;
using OpenTrackerNext.Core.Maps;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.MapLayouts;

[ExcludeFromCodeCoverage]
public sealed class MapLayoutPrototypeTests
{
    private readonly MapLayoutPrototype _subject = new();

    [Fact]
    public void TitlePrefix_ShouldReturnExpected()
    {
        MapLayoutPrototype.TitlePrefix
            .Should()
            .Be(MapLayoutPrototype.MapLayoutTitlePrefix);
    }
    
    [Fact]
    public void DocumentChanges_ShouldEmitWhenAnyPropertyChanges()
    {
        using var observer = _subject.DocumentChanges.Observe();
        
        _subject.Name = "Test";
        _subject.Maps.Add(new MapLayoutMapPrototype());
        _subject.Maps[0].Map = MapId.New();
        _subject.Maps.RemoveAt(0);

        observer.Should().Push(4);
    }

    [Fact]
    public void Name_ShouldInitializeToDefault()
    {
        _subject.Name
            .Should()
            .BeEmpty();
    }

    [Fact]
    public void Name_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.Name = "Test";
        
        monitor.Should().RaisePropertyChangeFor(x => x.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Test")]
    public void Name_ShouldSerializeAsExpected(string expected)
    {
        _subject.Name = expected;
        
        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);
        
        jsonObject[nameof(MapLayoutPrototype.Name)]?
            .Value<string>()
            .Should()
            .Be(expected);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("Test")]
    public void Name_ShouldDeserializeAsExpected(string expected)
    {
        var jsonObject = new JObject
        {
            { nameof(MapLayoutPrototype.Name), new JValue(expected) }
        };
        
        var json = jsonObject.ToString(Formatting.Indented);

        var subject = JsonContext.Deserialize<MapLayoutPrototype>(json);
        
        subject.Name
            .Should()
            .Be(expected);
    }
    
    [Fact]
    public void Maps_ShouldDefaultToEmpty()
    {
        _subject.Maps
            .Should()
            .BeEmpty();
    }

    [Fact]
    public void Maps_ShouldSerializeAsExpected()
    {
        _subject.Maps.Add(new MapLayoutMapPrototype { Map = MapId.New() });
        _subject.Maps.Add(new MapLayoutMapPrototype { Map = MapId.New() });
        
        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(MapLayoutPrototype.Maps)]?
            .Value<JArray>()?[0]
            .Value<JObject>()?[nameof(MapLayoutMapPrototype.Map)]?
            .Value<string>()
            .Should()
            .Be(_subject.Maps[0].Map.ToString());
        jsonObject[nameof(MapLayoutPrototype.Maps)]?
            .Value<JArray>()?[1]
            .Value<JObject>()?[nameof(MapLayoutMapPrototype.Map)]?
            .Value<string>()
            .Should()
            .Be(_subject.Maps[1].Map.ToString());
    }
    
    [Fact]
    public void Maps_ShouldDeserializeAsExpected()
    {
        var maps = new List<MapLayoutMapPrototype>
        {
            new() { Map = MapId.New() },
            new() { Map = MapId.New() }
        };
        
        var jsonObject = new JObject
        {
            {
                nameof(MapLayoutPrototype.Maps), new JArray
                {
                    new JObject
                    {
                        { nameof(MapLayoutMapPrototype.Map), new JValue(maps[0].Map.Value.ToString()) }
                    },
                    new JObject
                    {
                        { nameof(MapLayoutMapPrototype.Map), new JValue(maps[1].Map.Value.ToString()) }
                    }
                }
            }
        };
        
        var json = jsonObject.ToString(Formatting.Indented);

        var subject = JsonContext.Deserialize<MapLayoutPrototype>(json);
        
        subject.Maps
            .Should()
            .BeEquivalentTo(maps);
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
    public void MakeValueEqualTo_ShouldSetValuesToBeValueEqual()
    {
        _subject.Maps.Add(new MapLayoutMapPrototype { Map = MapId.New() });
        
        var other = new MapLayoutPrototype
        {
            Name = "Test",
            Maps =
            [
                new MapLayoutMapPrototype { Map = MapId.New() }
            ]
        };

        ((IMakeValueEqual) _subject)
            .MakeValueEqualTo(other)
            .Should()
            .BeTrue();

        _subject.Name
            .Should()
            .Be(other.Name);
        _subject.Maps
            .Should()
            .BeEquivalentTo(other.Maps);
    }
    
    [Fact]
    public void MakeValueEqualTo_ShouldAddMaps_WhenMapsHasLessObjectsThanOther()
    {
        _subject.Maps.Add(new MapLayoutMapPrototype { Map = MapId.New() });
        
        var other = new MapLayoutPrototype
        {
            Name = "Test",
            Maps =
            [
                new MapLayoutMapPrototype { Map = MapId.New() },
                new MapLayoutMapPrototype { Map = MapId.New() }
            ]
        };

        ((IMakeValueEqual) _subject)
            .MakeValueEqualTo(other)
            .Should()
            .BeTrue();

        _subject.Name
            .Should()
            .Be(other.Name);
        _subject.Maps
            .Should()
            .BeEquivalentTo(other.Maps);
    }
    
    [Fact]
    public void MakeValueEqualTo_ShouldAddMaps_WhenMapsHasMoreObjectsThanOther()
    {
        _subject.Maps.Add(new MapLayoutMapPrototype { Map = MapId.New() });
        _subject.Maps.Add(new MapLayoutMapPrototype { Map = MapId.New() });
        _subject.Maps.Add(new MapLayoutMapPrototype { Map = MapId.New() });
        
        var other = new MapLayoutPrototype
        {
            Name = "Test",
            Maps =
            [
                new MapLayoutMapPrototype { Map = MapId.New() },
                new MapLayoutMapPrototype { Map = MapId.New() }
            ]
        };

        ((IMakeValueEqual) _subject)
            .MakeValueEqualTo(other)
            .Should()
            .BeTrue();

        _subject.Name
            .Should()
            .Be(other.Name);
        
        _subject.Maps
            .Should()
            .BeEquivalentTo(other.Maps);
    }
    
    [Fact]
    public void MakeValueEqualTo_ShouldDoNothingAndReturnFalse_WhenOtherIsNotSameType()
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
            .BeOfType<MapLayoutPrototype>()
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
    public void ValueEquals_ShouldReturnTrue_WhenPropertyValuesAreValueEqual()
    {
        const string name = "Test";
        
        _subject.Name = name;
        _subject.Maps.Add(new MapLayoutMapPrototype { Map = MapId.New() });
        
        var other = new MapLayoutPrototype
        {
            Name = name,
            Maps =
            [
                new MapLayoutMapPrototype { Map = _subject.Maps[0].Map }
            ]
        };
        
        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenNameValuesAreNotEqual()
    {
        _subject.Name = "Test";
        
        var other = new MapLayoutPrototype { Name = "Other" };
        
        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenMapsValuesAreNotEqual()
    {
        _subject.Maps.Add(new MapLayoutMapPrototype { Map = MapId.New() });
        
        var other = new MapLayoutPrototype
        {
            Maps =
            [
                new MapLayoutMapPrototype { Map = MapId.New() }
            ]
        };
        
        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenMapsHaveDifferentCounts()
    {
        _subject.Maps.Add(new MapLayoutMapPrototype { Map = MapId.New() });
        _subject.Maps.Add(new MapLayoutMapPrototype { Map = MapId.New() });
        
        var other = new MapLayoutPrototype
        {
            Maps =
            [
                new MapLayoutMapPrototype { Map = MapId.New() }
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