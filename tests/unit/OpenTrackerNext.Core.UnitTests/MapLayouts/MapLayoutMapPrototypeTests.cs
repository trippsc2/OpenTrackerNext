using System;
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
public sealed class MapLayoutMapPrototypeTests
{
    private readonly MapLayoutMapPrototype _subject = new();

    [Fact]
    public void DocumentChanges_ShouldEmitWhenPropertyChanges()
    {
        using var observer = _subject.DocumentChanges.Observe();
        
        _subject.Map = MapId.New();

        observer.Should().Push(1);
    }
    
    [Fact]
    public void Map_ShouldDefaultToEmpty()
    {
        _subject.Map
            .Should()
            .Be(MapId.Empty);
    }
    
    [Fact]
    public void Map_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.Map = MapId.New();
        
        monitor.Should().RaisePropertyChangeFor(x => x.Map);
    }

    [Fact]
    public void Map_ShouldSerializeAsExpected()
    {
        var map = MapId.New();
        _subject.Map = map;

        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(MapLayoutMapPrototype.Map)]?
            .Value<string>()
            .Should()
            .Be(map.Value.ToString());
    }
    
    [Fact]
    public void Map_ShouldDeserializeAsExpected()
    {
        var map = MapId.New();
        var jsonObject = new JObject
        {
            { nameof(MapLayoutMapPrototype.Map), new JValue(map.Value.ToString()) }
        };

        var json = jsonObject.ToString(Formatting.Indented);

        var subject = JsonContext.Deserialize<MapLayoutMapPrototype>(json);
        
        subject.Map
            .Should()
            .Be(map);
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
        var other = new MapLayoutMapPrototype { Map = MapId.New() };
        
        ((IMakeValueEqual)_subject)
            .MakeValueEqualTo(other)
            .Should()
            .BeTrue();

        _subject.Map
            .Should()
            .Be(other.Map);
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
        _subject.Map = MapId.New();
        
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
        _subject.Map = MapId.New();
        
        var other = new MapLayoutMapPrototype { Map = _subject.Map };
        
        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenOtherIsNotValueEqual()
    {
        _subject.Map = MapId.New();
        
        var other = new MapLayoutMapPrototype { Map = MapId.New() };
        
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
            .ValueEquals(null!)
            .Should()
            .BeFalse();
    }
}