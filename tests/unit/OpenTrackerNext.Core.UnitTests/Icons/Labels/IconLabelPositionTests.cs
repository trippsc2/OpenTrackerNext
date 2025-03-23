using System.Diagnostics.CodeAnalysis;
using Avalonia.Layout;
using FluentAssertions;
using OpenTrackerNext.Core.Icons.Labels;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Icons.Labels;

[ExcludeFromCodeCoverage]
public sealed class IconLabelPositionTests
{
    [Theory]
    [InlineData("Top Left", nameof(IconLabelPosition.TopLeft))]
    [InlineData("Top Center", nameof(IconLabelPosition.TopCenter))]
    [InlineData("Top Right", nameof(IconLabelPosition.TopRight))]
    [InlineData("Center Left", nameof(IconLabelPosition.CenterLeft))]
    [InlineData("Center", nameof(IconLabelPosition.Center))]
    [InlineData("Center Right", nameof(IconLabelPosition.CenterRight))]
    [InlineData("Bottom Left", nameof(IconLabelPosition.BottomLeft))]
    [InlineData("Bottom Center", nameof(IconLabelPosition.BottomCenter))]
    [InlineData("Bottom Right", nameof(IconLabelPosition.BottomRight))]
    public void DisplayName_ShouldReturnExpected(string expected, string iconLabelPositionName)
    {
        var iconLabelPosition = IconLabelPosition.FromName(iconLabelPositionName);

        iconLabelPosition.DisplayName
            .Should()
            .Be(expected);
    }
    
    [Theory]
    [InlineData(HorizontalAlignment.Left, nameof(IconLabelPosition.TopLeft))]
    [InlineData(HorizontalAlignment.Center, nameof(IconLabelPosition.TopCenter))]
    [InlineData(HorizontalAlignment.Right, nameof(IconLabelPosition.TopRight))]
    [InlineData(HorizontalAlignment.Left, nameof(IconLabelPosition.CenterLeft))]
    [InlineData(HorizontalAlignment.Center, nameof(IconLabelPosition.Center))]
    [InlineData(HorizontalAlignment.Right, nameof(IconLabelPosition.CenterRight))]
    [InlineData(HorizontalAlignment.Left, nameof(IconLabelPosition.BottomLeft))]
    [InlineData(HorizontalAlignment.Center, nameof(IconLabelPosition.BottomCenter))]
    [InlineData(HorizontalAlignment.Right, nameof(IconLabelPosition.BottomRight))]
    public void HorizontalAlignment_ShouldReturnExpected(HorizontalAlignment expected, string iconLabelPositionName)
    {
        var iconLabelPosition = IconLabelPosition.FromName(iconLabelPositionName);

        iconLabelPosition.HorizontalAlignment
            .Should()
            .Be(expected);
    }
    
    [Theory]
    [InlineData(VerticalAlignment.Top, nameof(IconLabelPosition.TopLeft))]
    [InlineData(VerticalAlignment.Top, nameof(IconLabelPosition.TopCenter))]
    [InlineData(VerticalAlignment.Top, nameof(IconLabelPosition.TopRight))]
    [InlineData(VerticalAlignment.Center, nameof(IconLabelPosition.CenterLeft))]
    [InlineData(VerticalAlignment.Center, nameof(IconLabelPosition.Center))]
    [InlineData(VerticalAlignment.Center, nameof(IconLabelPosition.CenterRight))]
    [InlineData(VerticalAlignment.Bottom, nameof(IconLabelPosition.BottomLeft))]
    [InlineData(VerticalAlignment.Bottom, nameof(IconLabelPosition.BottomCenter))]
    [InlineData(VerticalAlignment.Bottom, nameof(IconLabelPosition.BottomRight))]
    public void VerticalAlignment_ShouldReturnExpected(VerticalAlignment expected, string iconLabelPositionName)
    {
        var iconLabelPosition = IconLabelPosition.FromName(iconLabelPositionName);

        iconLabelPosition.VerticalAlignment
            .Should()
            .Be(expected);
    }
}