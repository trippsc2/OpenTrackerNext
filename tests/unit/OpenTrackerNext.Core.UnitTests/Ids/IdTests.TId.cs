using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTrackerNext.Core.Ids;
using OpenTrackerNext.Core.Json;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Ids;

[ExcludeFromCodeCoverage]
public abstract class IdTests<TId>(string folderName)
    where TId : struct, IGuidId<TId>
{
    [Fact]
    public void FolderName_ShouldReturnEntitiesFolderName()
    {
        TId.FolderName
            .Should()
            .Be(folderName);
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("11111111-1111-1111-1111-111111111111")]
    [InlineData("22222222-2222-2222-2222-222222222222")]
    public void Value_ShouldSerializeAsExpected(string expected)
    {
        var id = TId.Parse(expected);
        var json = JsonContext.Serialize(id);
        var jsonValue = JToken.Parse(json);
    
        jsonValue.Value<string>()
            .Should()
            .Be(expected);
    }
    
    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("11111111-1111-1111-1111-111111111111")]
    [InlineData("22222222-2222-2222-2222-222222222222")]
    public void Value_ShouldDeserializeAsExpected(string expected)
    {
        var jsonValue = new JValue(expected);
        var json = jsonValue.ToString(Formatting.Indented);
        var id = JsonContext.Deserialize<TId>(json);
        
        id.Value
            .Should()
            .Be(expected);
    }
    
    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("11111111-1111-1111-1111-111111111111")]
    [InlineData("22222222-2222-2222-2222-222222222222")]
    public void Parse_ShouldReturnEntityId_WhenValidValue(string value)
    {
        var id = TId.Parse(value);
        
        id.Value
            .Should()
            .Be(value);
    }
    
    [Fact]
    public void Parse_ShouldThrowFormatException_WhenInvalidValue()
    {
        FluentActions.Invoking(() => TId.Parse("invalid"))
            .Should()
            .Throw<FormatException>();
    }
}