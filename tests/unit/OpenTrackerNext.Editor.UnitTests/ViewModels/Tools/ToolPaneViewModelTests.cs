using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Disposables;
using FluentAssertions;
using NSubstitute;
using OpenTrackerNext.Core.ViewModels;
using OpenTrackerNext.Editor.Tools;
using OpenTrackerNext.Editor.ViewModels.Tools;
using Splat;
using Xunit;

namespace OpenTrackerNext.Editor.UnitTests.ViewModels.Tools;

[ExcludeFromCodeCoverage]
public sealed class ToolPaneViewModelTests
{
    private sealed class MockViewModel : ViewModel;
    
    private readonly CompositeDisposable _disposables = new();
    private readonly IToolFactory _toolFactory = Substitute.For<IToolFactory>();
    private readonly ToolService _toolService;
    private readonly List<Tool> _createdTools =
    [
        new()
        {
            Id = new ToolId("top-left-1"),
            Title = "Top Left 1",
            Content = new MockViewModel(),
            Position = ToolBarPosition.TopLeft
        },
        new()
        {
            Id = new ToolId("top-left-2"),
            Title = "Top Left 2",
            Content = new MockViewModel(),
            Position = ToolBarPosition.TopLeft
        },
        new()
        {
            Id = new ToolId("top-right-1"),
            Title = "Top Right 1",
            Content = new MockViewModel(),
            Position = ToolBarPosition.TopRight
        },
        new()
        {
            Id = new ToolId("top-right-2"),
            Title = "Top Right 2",
            Content = new MockViewModel(),
            Position = ToolBarPosition.TopRight
        },
        new()
        {
            Id = new ToolId("bottom-left-1"),
            Title = "Bottom Left 1",
            Content = new MockViewModel(),
            Position = ToolBarPosition.BottomLeft
        },
        new()
        {
            Id = new ToolId("bottom-left-2"),
            Title = "Bottom Left 2",
            Content = new MockViewModel(),
            Position = ToolBarPosition.BottomLeft
        },
        new()
        {
            Id = new ToolId("bottom-right-1"),
            Title = "Bottom Right 1",
            Content = new MockViewModel(),
            Position = ToolBarPosition.BottomRight
        },
        new()
        {
            Id = new ToolId("bottom-right-2"),
            Title = "Bottom Right 2",
            Content = new MockViewModel(),
            Position = ToolBarPosition.BottomRight
        }
    ];
    
    public ToolPaneViewModelTests()
    {
        _toolFactory.CreateTools().Returns(_createdTools);
        _toolService = new ToolService(_toolFactory);
    }

    private ToolViewModel ToolFactory(Tool tool)
    {
        return new ToolViewModel(_toolService, tool);
    }

    [Theory]
    [InlineData(nameof(ToolPaneSide.Left))]
    [InlineData(nameof(ToolPaneSide.Right))]
    public void TopIsVisible_ShouldReturnExpected(string toolPaneSideName)
    {
        var toolPaneSide = ToolPaneSide.FromName(toolPaneSideName);
        
        var subject = new ToolPaneViewModel(_toolService, ToolFactory, toolPaneSide);

        subject.Activator
            .Activate()
            .DisposeWith(_disposables);
        
        subject.TopIsVisible
            .Should()
            .BeFalse();
        
        var topTool = _createdTools
            .FirstOrDefault(x => x.Position == toolPaneSide.Top)
            .Should()
            .NotBeNull()
            .And.BeOfType<Tool>()
            .Subject;
        
        _toolService.ActiveTools[topTool.Position].Value = topTool;
        
        subject.TopIsVisible
            .Should()
            .BeTrue();
        
        _toolService.ActiveTools[topTool.Position].Value = null;
        
        subject.TopIsVisible
            .Should()
            .BeFalse();
    }
    
    [Theory]
    [InlineData(nameof(ToolPaneSide.Left))]
    [InlineData(nameof(ToolPaneSide.Right))]
    public void BottomIsVisible_ShouldReturnExpected(string toolPaneSideName)
    {
        var toolPaneSide = ToolPaneSide.FromName(toolPaneSideName);
        
        var subject = new ToolPaneViewModel(_toolService, ToolFactory, toolPaneSide);

        subject.Activator
            .Activate()
            .DisposeWith(_disposables);
        
        subject.BottomIsVisible
            .Should()
            .BeFalse();
        
        var bottomTool = _createdTools
            .FirstOrDefault(x => x.Position == toolPaneSide.Bottom)
            .Should()
            .NotBeNull()
            .And.BeOfType<Tool>()
            .Subject;
        
        _toolService.ActiveTools[bottomTool.Position].Value = bottomTool;
        
        subject.BottomIsVisible
            .Should()
            .BeTrue();
        
        _toolService.ActiveTools[bottomTool.Position].Value = null;
        
        subject.BottomIsVisible
            .Should()
            .BeFalse();
    }
    
    [Theory]
    [InlineData(nameof(ToolPaneSide.Left))]
    [InlineData(nameof(ToolPaneSide.Right))]
    public void GridSplitterIsVisible_ShouldReturnExpected(string toolPaneSideName)
    {
        var toolPaneSide = ToolPaneSide.FromName(toolPaneSideName);
        
        var subject = new ToolPaneViewModel(_toolService, ToolFactory, toolPaneSide);

        subject.Activator
            .Activate()
            .DisposeWith(_disposables);
        
        subject.GridSplitterIsVisible
            .Should()
            .BeFalse();
        
        var topTool = _createdTools
            .FirstOrDefault(x => x.Position == toolPaneSide.Top)
            .Should()
            .NotBeNull()
            .And.BeOfType<Tool>()
            .Subject;
        
        _toolService.ActiveTools[topTool.Position].Value = topTool;
        
        subject.GridSplitterIsVisible
            .Should()
            .BeFalse();
        
        var bottomTool = _createdTools
            .FirstOrDefault(x => x.Position == toolPaneSide.Bottom)
            .Should()
            .NotBeNull()
            .And.BeOfType<Tool>()
            .Subject;
        
        _toolService.ActiveTools[bottomTool.Position].Value = bottomTool;
        
        subject.GridSplitterIsVisible
            .Should()
            .BeTrue();
        
        _toolService.ActiveTools[topTool.Position].Value = null;
        
        subject.GridSplitterIsVisible
            .Should()
            .BeFalse();
        
        _toolService.ActiveTools[bottomTool.Position].Value = null;
        
        subject.GridSplitterIsVisible
            .Should()
            .BeFalse();
    }
    
    [Theory]
    [InlineData(nameof(ToolPaneSide.Left))]
    [InlineData(nameof(ToolPaneSide.Right))]
    public void TopRowSpan_ShouldReturnExpected(string toolPaneSideName)
    {
        var toolPaneSide = ToolPaneSide.FromName(toolPaneSideName);
        
        var subject = new ToolPaneViewModel(_toolService, ToolFactory, toolPaneSide);

        subject.Activator
            .Activate()
            .DisposeWith(_disposables);
        
        subject.TopRowSpan
            .Should()
            .Be(3);
        
        var bottomTool = _createdTools
            .FirstOrDefault(x => x.Position == toolPaneSide.Bottom)
            .Should()
            .NotBeNull()
            .And.BeOfType<Tool>()
            .Subject;
        
        _toolService.ActiveTools[bottomTool.Position].Value = bottomTool;
        
        subject.TopRowSpan
            .Should()
            .Be(1);
        
        _toolService.ActiveTools[bottomTool.Position].Value = null;
        
        subject.TopRowSpan
            .Should()
            .Be(3);
    }
    
    [Theory]
    [InlineData(nameof(ToolPaneSide.Left))]
    [InlineData(nameof(ToolPaneSide.Right))]
    public void BottomRow_ShouldReturnExpected(string toolPaneSideName)
    {
        var toolPaneSide = ToolPaneSide.FromName(toolPaneSideName);
        
        var subject = new ToolPaneViewModel(_toolService, ToolFactory, toolPaneSide);

        subject.Activator
            .Activate()
            .DisposeWith(_disposables);
        
        subject.BottomRow
            .Should()
            .Be(0);
        
        var topTool = _createdTools
            .FirstOrDefault(x => x.Position == toolPaneSide.Top)
            .Should()
            .NotBeNull()
            .And.BeOfType<Tool>()
            .Subject;
        
        _toolService.ActiveTools[topTool.Position].Value = topTool;
        
        subject.BottomRow
            .Should()
            .Be(2);
        
        _toolService.ActiveTools[topTool.Position].Value = null;
        
        subject.BottomRow
            .Should()
            .Be(0);
    }
    
    [Theory]
    [InlineData(nameof(ToolPaneSide.Left))]
    [InlineData(nameof(ToolPaneSide.Right))]
    public void BottomRowSpan_ShouldReturnExpected(string toolPaneSideName)
    {
        var toolPaneSide = ToolPaneSide.FromName(toolPaneSideName);
        
        var subject = new ToolPaneViewModel(_toolService, ToolFactory, toolPaneSide);

        subject.Activator
            .Activate()
            .DisposeWith(_disposables);
        
        subject.BottomRowSpan
            .Should()
            .Be(3);
        
        var topTool = _createdTools
            .FirstOrDefault(x => x.Position == toolPaneSide.Top)
            .Should()
            .NotBeNull()
            .And.BeOfType<Tool>()
            .Subject;
        
        _toolService.ActiveTools[topTool.Position].Value = topTool;
        
        subject.BottomRowSpan
            .Should()
            .Be(1);
        
        _toolService.ActiveTools[topTool.Position].Value = null;
        
        subject.BottomRowSpan
            .Should()
            .Be(3);
    }
    
    [Theory]
    [InlineData(nameof(ToolPaneSide.Left))]
    [InlineData(nameof(ToolPaneSide.Right))]
    public void Top_ShouldReturnExpected(string toolPaneSideName)
    {
        var toolPaneSide = ToolPaneSide.FromName(toolPaneSideName);
        
        var subject = new ToolPaneViewModel(_toolService, ToolFactory, toolPaneSide);

        subject.Activator
            .Activate()
            .DisposeWith(_disposables);
        
        subject.Top
            .Should()
            .BeNull();
        
        var topTool = _createdTools
            .FirstOrDefault(x => x.Position == toolPaneSide.Top)
            .Should()
            .NotBeNull()
            .And.BeOfType<Tool>()
            .Subject;
        
        _toolService.ActiveTools[topTool.Position].Value = topTool;
        
        subject.Top
            .Should()
            .NotBeNull();
        
        _toolService.ActiveTools[topTool.Position].Value = null;
        
        subject.Top
            .Should()
            .BeNull();
    }
    
    [Theory]
    [InlineData(nameof(ToolPaneSide.Left))]
    [InlineData(nameof(ToolPaneSide.Right))]
    public void Bottom_ShouldReturnExpected(string toolPaneSideName)
    {
        var toolPaneSide = ToolPaneSide.FromName(toolPaneSideName);
        
        var subject = new ToolPaneViewModel(_toolService, ToolFactory, toolPaneSide);

        subject.Activator
            .Activate()
            .DisposeWith(_disposables);
        
        subject.Bottom
            .Should()
            .BeNull();
        
        var bottomTool = _createdTools
            .FirstOrDefault(x => x.Position == toolPaneSide.Bottom)
            .Should()
            .NotBeNull()
            .And.BeOfType<Tool>()
            .Subject;
        
        _toolService.ActiveTools[toolPaneSide.Bottom].Value = bottomTool;
        
        subject.Bottom
            .Should()
            .NotBeNull();
        
        _toolService.ActiveTools[toolPaneSide.Bottom].Value = null;
        
        subject.Bottom
            .Should()
            .BeNull();
    }
    
    [Fact]
    public void SplatResolve_ShouldResolveToFactory()
    {
        var factory = Locator.Current.GetService<ToolPaneViewModel.Factory>()!;
        
        var subject1 = factory(ToolPaneSide.Left);
        var subject2 = factory(ToolPaneSide.Left);
        
        subject1.Should().NotBeSameAs(subject2);
    }
}