using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using FluentAssertions;
using OpenTrackerNext.Core.Requirements.UIPanel;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Requirements.UIPanel;

[ExcludeFromCodeCoverage]
public sealed class UIPanelDockTests
{
    [Theory]
    [InlineData("Left", nameof(UIPanelDock.Left))]
    [InlineData("Bottom", nameof(UIPanelDock.Bottom))]
    [InlineData("Right", nameof(UIPanelDock.Right))]
    [InlineData("Top", nameof(UIPanelDock.Top))]
    [InlineData("Left or Right", nameof(UIPanelDock.LeftOrRight))]
    [InlineData("Top or Bottom", nameof(UIPanelDock.TopOrBottom))]
    public void DisplayName_ShouldReturnExpected(string expected, string uiPanelDockName)
    {
        var uiPanelDock = UIPanelDock.FromName(uiPanelDockName);

        uiPanelDock.DisplayName
            .Should()
            .Be(expected);
    }

    [Theory]
    [InlineData(true, nameof(UIPanelDock.Left), Dock.Left)]
    [InlineData(false, nameof(UIPanelDock.Left), Dock.Bottom)]
    [InlineData(false, nameof(UIPanelDock.Left), Dock.Right)]
    [InlineData(false, nameof(UIPanelDock.Left), Dock.Top)]
    [InlineData(false, nameof(UIPanelDock.Bottom), Dock.Left)]
    [InlineData(true, nameof(UIPanelDock.Bottom), Dock.Bottom)]
    [InlineData(false, nameof(UIPanelDock.Bottom), Dock.Right)]
    [InlineData(false, nameof(UIPanelDock.Bottom), Dock.Top)]
    [InlineData(false, nameof(UIPanelDock.Right), Dock.Left)]
    [InlineData(false, nameof(UIPanelDock.Right), Dock.Bottom)]
    [InlineData(true, nameof(UIPanelDock.Right), Dock.Right)]
    [InlineData(false, nameof(UIPanelDock.Right), Dock.Top)]
    [InlineData(false, nameof(UIPanelDock.Top), Dock.Left)]
    [InlineData(false, nameof(UIPanelDock.Top), Dock.Bottom)]
    [InlineData(false, nameof(UIPanelDock.Top), Dock.Right)]
    [InlineData(true, nameof(UIPanelDock.Top), Dock.Top)]
    [InlineData(true, nameof(UIPanelDock.LeftOrRight), Dock.Left)]
    [InlineData(false, nameof(UIPanelDock.LeftOrRight), Dock.Bottom)]
    [InlineData(true, nameof(UIPanelDock.LeftOrRight), Dock.Right)]
    [InlineData(false, nameof(UIPanelDock.LeftOrRight), Dock.Top)]
    [InlineData(false, nameof(UIPanelDock.TopOrBottom), Dock.Left)]
    [InlineData(true, nameof(UIPanelDock.TopOrBottom), Dock.Bottom)]
    [InlineData(false, nameof(UIPanelDock.TopOrBottom), Dock.Right)]
    [InlineData(true, nameof(UIPanelDock.TopOrBottom), Dock.Top)]
    public void IsMatchingDock_ShouldReturnExpected(bool expected, string uiPanelDockName, Dock dock)
    {
        var uiPanelDock = UIPanelDock.FromName(uiPanelDockName);

        uiPanelDock.IsMatchingDock(dock)
            .Should()
            .Be(expected);
    }
}