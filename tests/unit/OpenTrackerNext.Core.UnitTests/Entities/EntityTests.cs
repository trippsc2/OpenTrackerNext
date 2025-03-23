using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Nodes;
using FluentAssertions;
using OpenTrackerNext.Core.Entities;
using OpenTrackerNext.Core.Json;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Entities;

[ExcludeFromCodeCoverage]
public sealed class EntityTests
{
    [Fact]
    public void Minimum_ShouldReturnValueFromConstructor()
    {
        const int expected = -1;
        
        var subject = new Entity(expected, 0, 1);

        subject.Minimum
            .Should()
            .Be(expected);
    }

    [Fact]
    public void Minimum_ShouldNotSerialize()
    {
        var subject = new Entity(-1, 0, 1);

        var json = JsonContext.Serialize(subject);
        var jsonObject = JsonNode.Parse(json);
        
        jsonObject?[nameof(IEntity.Minimum)]
            .Should()
            .BeNull();
    }
    
    [Fact]
    public void Starting_ShouldReturnValueFromConstructor()
    {
        const int expected = 0;
        
        var subject = new Entity(-1, expected, 1);

        subject.Starting
            .Should()
            .Be(expected);
    }
    
    [Fact]
    public void Starting_ShouldNotSerialize()
    {
        var subject = new Entity(-1, 0, 1);

        var json = JsonContext.Serialize(subject);
        var jsonObject = JsonNode.Parse(json);
        
        jsonObject?[nameof(IEntity.Starting)]
            .Should()
            .BeNull();
    }
    
    [Fact]
    public void Maximum_ShouldReturnValueFromConstructor()
    {
        const int expected = 1;
        
        var subject = new Entity(-1, 0, expected);

        subject.Maximum
            .Should()
            .Be(expected);
    }
    
    [Fact]
    public void Maximum_ShouldNotSerialize()
    {
        var subject = new Entity(-1, 0, 1);

        var json = JsonContext.Serialize(subject);
        var jsonObject = JsonNode.Parse(json);
        
        jsonObject?[nameof(IEntity.Maximum)]
            .Should()
            .BeNull();
    }
    
    [Fact]
    public void Current_ShouldDefaultToStartingValueFromConstructor()
    {
        const int expected = 0;
        
        var subject = new Entity(-1, expected, 1);

        subject.Current
            .Should()
            .Be(expected);
    }

    [Fact]
    public void Current_ShouldRaisePropertyChanged()
    {
        var subject = new Entity(-1, 0, 1);
        
        using var monitor = subject.Monitor();
        
        subject.Current = 1;
        
        monitor.Should().RaisePropertyChangeFor(x => x.Current);
    }
    
    [Fact]
    public void Current_ShouldSerializeAsExpected()
    {
        const int expected = 1;
        
        var subject = new Entity(-1, 0, 1)
        {
            Current = expected
        };

        var json = JsonContext.Serialize(subject);
        var jsonObject = JsonNode.Parse(json);
        
        jsonObject?[nameof(IEntity.Current)]?
            .GetValue<int>()
            .Should()
            .Be(expected);
    }
    
    [Fact]
    public void Current_ShouldDeserializeAsExpected()
    {
        const int expected = 1;

        var jsonObject = new JsonObject
        {
            { nameof(IEntity.Current), JsonValue.Create(expected) }
        };
        
        var json = jsonObject.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
        
        var subject = JsonContext.Deserialize<Entity>(json);

        subject.Current
            .Should()
            .Be(expected);
    }
    
    [Fact]
    public void Reset_ShouldSetCurrentToStarting()
    {
        const int expected = 0;
        var subject = new Entity(-1, 0, 1)
        {
            Current = 1
        };

        subject.Reset();
        
        subject.Current
            .Should()
            .Be(expected);
    }
}