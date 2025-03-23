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
public sealed class ToolBarViewModelTests
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
    
    public ToolBarViewModelTests()
    {
        _toolFactory.CreateTools().Returns(_createdTools);
        _toolService = new ToolService(_toolFactory);
    }

    private ToolButtonViewModel ToolButtonFactory(Tool tool)
    {
        return new ToolButtonViewModel(_toolService, tool);
    }

    [Theory]
    [InlineData(nameof(ToolBarPosition.TopLeft))]
    [InlineData(nameof(ToolBarPosition.TopRight))]
    [InlineData(nameof(ToolBarPosition.BottomLeft))]
    [InlineData(nameof(ToolBarPosition.BottomRight))]
    public void HorizontalAlignment_ShouldReturnExpected(string toolBarPositionName)
    {
        var toolBarPosition = ToolBarPosition.FromName(toolBarPositionName);

        var subject = new ToolBarViewModel(_toolService, ToolButtonFactory, toolBarPosition);

        subject.Activator
            .Activate()
            .DisposeWith(_disposables);

        subject.HorizontalAlignment
            .Should()
            .Be(toolBarPosition.HorizontalAlignment);
    }
    
    [Theory]
    [InlineData(nameof(ToolBarPosition.TopLeft))]
    [InlineData(nameof(ToolBarPosition.TopRight))]
    [InlineData(nameof(ToolBarPosition.BottomLeft))]
    [InlineData(nameof(ToolBarPosition.BottomRight))]
    public void VerticalAlignment_ShouldReturnExpected(string toolBarPositionName)
    {
        var toolBarPosition = ToolBarPosition.FromName(toolBarPositionName);

        var subject = new ToolBarViewModel(_toolService, ToolButtonFactory, toolBarPosition);

        subject.Activator
            .Activate()
            .DisposeWith(_disposables);

        subject.VerticalAlignment
            .Should()
            .Be(toolBarPosition.VerticalAlignment);
    }
    
    [Theory]
    [InlineData(nameof(ToolBarPosition.TopLeft))]
    [InlineData(nameof(ToolBarPosition.TopRight))]
    [InlineData(nameof(ToolBarPosition.BottomLeft))]
    [InlineData(nameof(ToolBarPosition.BottomRight))]
    public void FlowDirection_ShouldReturnExpected(string toolBarPositionName)
    {
        var toolBarPosition = ToolBarPosition.FromName(toolBarPositionName);

        var subject = new ToolBarViewModel(_toolService, ToolButtonFactory, toolBarPosition);

        subject.Activator
            .Activate()
            .DisposeWith(_disposables);

        subject.FlowDirection
            .Should()
            .Be(toolBarPosition.FlowDirection);
    }
    
    [Theory]
    [InlineData(nameof(ToolBarPosition.TopLeft))]
    [InlineData(nameof(ToolBarPosition.TopRight))]
    [InlineData(nameof(ToolBarPosition.BottomLeft))]
    [InlineData(nameof(ToolBarPosition.BottomRight))]
    public void Buttons_ShouldReturnExpected(string toolBarPositionName)
    {
        var toolBarPosition = ToolBarPosition.FromName(toolBarPositionName);

        var subject = new ToolBarViewModel(_toolService, ToolButtonFactory, toolBarPosition);

        subject.Activator
            .Activate()
            .DisposeWith(_disposables);

        var tools = _createdTools
            .Where(x => x.Position == toolBarPosition)
            .ToList();

        subject.Buttons
            .Should()
            .HaveCount(tools.Count)
            .And.Contain(x => x.Tool == tools[0])
            .And.Contain(x => x.Tool == tools[1]);
    }
    
    [Fact]
    public void SplatResolve_ShouldResolveToFactory()
    {
        var factory = Locator.Current.GetService<ToolBarViewModel.Factory>()!;
        
        var subject1 = factory(ToolBarPosition.TopLeft);
        var subject2 = factory(ToolBarPosition.TopLeft);

        subject1.Should().NotBeSameAs(subject2);
    }
}