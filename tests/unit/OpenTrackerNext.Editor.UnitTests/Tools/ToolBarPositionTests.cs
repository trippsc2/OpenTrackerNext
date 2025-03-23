using System.Diagnostics.CodeAnalysis;
using Avalonia.Layout;
using Avalonia.Media;
using FluentAssertions;
using OpenTrackerNext.Editor.Tools;
using Xunit;

namespace OpenTrackerNext.Editor.UnitTests.Tools;

[ExcludeFromCodeCoverage]
public sealed class ToolBarPositionTests
{
    [Theory]
    [InlineData(0, nameof(ToolBarPosition.TopLeft))]
    [InlineData(1, nameof(ToolBarPosition.TopRight))]
    [InlineData(2, nameof(ToolBarPosition.BottomLeft))]
    [InlineData(3, nameof(ToolBarPosition.BottomRight))]
    public void Value_ShouldReturnExpected(int expected, string name)
    {
        var subject = ToolBarPosition.FromName(name);
        
        subject.Value
            .Should()
            .Be(expected);
    }
    
    [Theory]
    [InlineData(HorizontalAlignment.Left, nameof(ToolBarPosition.TopLeft))]
    [InlineData(HorizontalAlignment.Right, nameof(ToolBarPosition.TopRight))]
    [InlineData(HorizontalAlignment.Left, nameof(ToolBarPosition.BottomLeft))]
    [InlineData(HorizontalAlignment.Right, nameof(ToolBarPosition.BottomRight))]
    public void HorizontalAlignment_ShouldReturnExpected(HorizontalAlignment expected, string name)
    {
        var subject = ToolBarPosition.FromName(name);
        
        subject.HorizontalAlignment
            .Should()
            .Be(expected);
    }
    
    [Theory]
    [InlineData(VerticalAlignment.Top, nameof(ToolBarPosition.TopLeft))]
    [InlineData(VerticalAlignment.Top, nameof(ToolBarPosition.TopRight))]
    [InlineData(VerticalAlignment.Bottom, nameof(ToolBarPosition.BottomLeft))]
    [InlineData(VerticalAlignment.Bottom, nameof(ToolBarPosition.BottomRight))]
    public void VerticalAlignment_ShouldReturnExpected(VerticalAlignment expected, string name)
    {
        var subject = ToolBarPosition.FromName(name);
        
        subject.VerticalAlignment
            .Should()
            .Be(expected);
    }
    
    [Theory]
    [InlineData(FlowDirection.LeftToRight, nameof(ToolBarPosition.TopLeft))]
    [InlineData(FlowDirection.LeftToRight, nameof(ToolBarPosition.TopRight))]
    [InlineData(FlowDirection.RightToLeft, nameof(ToolBarPosition.BottomLeft))]
    [InlineData(FlowDirection.RightToLeft, nameof(ToolBarPosition.BottomRight))]
    public void FlowDirection_ShouldReturnExpected(FlowDirection expected, string name)
    {
        var subject = ToolBarPosition.FromName(name);
        
        subject.FlowDirection
            .Should()
            .Be(expected);
    }
    
    [Theory]
    [InlineData(-90.0, nameof(ToolBarPosition.TopLeft))]
    [InlineData(90.0, nameof(ToolBarPosition.TopRight))]
    [InlineData(-90.0, nameof(ToolBarPosition.BottomLeft))]
    [InlineData(90.0, nameof(ToolBarPosition.BottomRight))]
    public void ButtonAngle_ShouldReturnExpected(double expected, string name)
    {
        var subject = ToolBarPosition.FromName(name);
        
        subject.ButtonAngle
            .Should()
            .Be(expected);
    }
}