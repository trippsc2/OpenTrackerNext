using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using OpenTrackerNext.Editor.Tools;
using Xunit;

namespace OpenTrackerNext.Editor.UnitTests.Tools;

[ExcludeFromCodeCoverage]
public sealed class ToolPaneSideTests
{
    [Theory]
    [InlineData(0, nameof(ToolPaneSide.Left))]
    [InlineData(1, nameof(ToolPaneSide.Right))]
    public void Value_ShouldReturnExpected(int value, string name)
    {
        var side = ToolPaneSide.FromName(name);
        
        side.Value
            .Should()
            .Be(value);
    }
    
    [Theory]
    [InlineData(nameof(ToolBarPosition.TopLeft), nameof(ToolPaneSide.Left))]
    [InlineData(nameof(ToolBarPosition.TopRight), nameof(ToolPaneSide.Right))]
    public void Top_ShouldReturnExpected(string topName, string name)
    {
        var top = ToolBarPosition.FromName(topName);
        var side = ToolPaneSide.FromName(name);
        
        side.Top
            .Should()
            .Be(top);
    }
    
    [Theory]
    [InlineData(nameof(ToolBarPosition.BottomLeft), nameof(ToolPaneSide.Left))]
    [InlineData(nameof(ToolBarPosition.BottomRight), nameof(ToolPaneSide.Right))]
    public void Bottom_ShouldReturnExpected(string bottomName, string name)
    {
        var bottom = ToolBarPosition.FromName(bottomName);
        var side = ToolPaneSide.FromName(name);
        
        side.Bottom
            .Should()
            .Be(bottom);
    }
}