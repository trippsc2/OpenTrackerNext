using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using Avalonia.Controls;
using FluentAssertions;
using OpenTrackerNext.Core.Requirements.UIPanel;
using OpenTrackerNext.Core.Settings;
using Splat;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Requirements.UIPanel;

[ExcludeFromCodeCoverage]
public sealed class UIPanelDockRequirementTests : IDisposable
{
    private readonly CompositeDisposable _disposables = new(); 
    private readonly LayoutSettings _layoutSettings = new();

    public void Dispose()
    {
        _disposables.Dispose();
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
    public void IsMet_ShouldReturnExpected(bool expected, string uiPanelDockName, Dock dock)
    {
        var uiPanelDock = UIPanelDock.FromName(uiPanelDockName);
        
        var subject = new UIPanelDockRequirement(_layoutSettings, uiPanelDock);
        
        subject.DisposeWith(_disposables);
        
        _layoutSettings.UIPanelDock = dock;

        subject.IsMet
            .Should()
            .Be(expected);
    }
    
    [Fact]
    public void IsMet_ShouldRaisePropertyChanged()
    {
        _layoutSettings.UIPanelDock = Dock.Bottom;

        var subject = new UIPanelDockRequirement(_layoutSettings, UIPanelDock.Left);
        
        subject.DisposeWith(_disposables);
        
        using var monitor = subject.Monitor();
        
        _layoutSettings.UIPanelDock = Dock.Left;
        
        monitor.Should().RaisePropertyChangeFor(x => x.IsMet);
    }
    
    [Fact]
    public void SplatResolve_ShouldResolveInterfaceFactory()
    {
        var factory = Locator.Current.GetService<UIPanelDockRequirement.Factory>()!;
        
        var subject1 = factory(UIPanelDock.Left);
        var subject2 = factory(UIPanelDock.Left);
        
        subject1.Should().BeOfType<UIPanelDockRequirement>();
        subject2.Should().BeOfType<UIPanelDockRequirement>();
        
        subject1.Should().NotBeSameAs(subject2);
    }
}