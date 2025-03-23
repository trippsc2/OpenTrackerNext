using System;
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
public sealed class ToolButtonViewModelTests
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
    
    public ToolButtonViewModelTests()
    {
        _toolFactory.CreateTools().Returns(_createdTools);
        _toolService = new ToolService(_toolFactory);
    }
    
    [Theory]
    [InlineData("top-left-1")]
    [InlineData("top-left-2")]
    [InlineData("top-right-1")]
    [InlineData("top-right-2")]
    [InlineData("bottom-left-1")]
    [InlineData("bottom-left-2")]
    [InlineData("bottom-right-1")]
    [InlineData("bottom-right-2")]
    public void Tool_ShouldEqualExpected(string id)
    {
        var tool = _createdTools
            .FirstOrDefault(x => x.Id == new ToolId(id))
            .Should()
            .NotBeNull()
            .And.BeOfType<Tool>()
            .Subject;
        
        var subject = new ToolButtonViewModel(_toolService, tool);

        subject.Activator
            .Activate()
            .DisposeWith(_disposables);
        
        subject.Tool
            .Should()
            .Be(tool);
    }
    
    [Theory]
    [InlineData("top-left-1")]
    [InlineData("top-left-2")]
    [InlineData("top-right-1")]
    [InlineData("top-right-2")]
    [InlineData("bottom-left-1")]
    [InlineData("bottom-left-2")]
    [InlineData("bottom-right-1")]
    [InlineData("bottom-right-2")]
    public void Content_ShouldEqualExpected(string id)
    {
        var tool = _createdTools
            .FirstOrDefault(x => x.Id == new ToolId(id))
            .Should()
            .NotBeNull()
            .And.BeOfType<Tool>()
            .Subject;
        
        var subject = new ToolButtonViewModel(_toolService, tool);

        subject.Activator
            .Activate()
            .DisposeWith(_disposables);
        
        subject.Content
            .Should()
            .Be(tool.Title);
    }
    
    [Theory]
    [InlineData("top-left-1")]
    [InlineData("top-left-2")]
    [InlineData("top-right-1")]
    [InlineData("top-right-2")]
    [InlineData("bottom-left-1")]
    [InlineData("bottom-left-2")]
    [InlineData("bottom-right-1")]
    [InlineData("bottom-right-2")]
    public void ToolTipHeader_ShouldEqualExpected(string id)
    {
        var tool = _createdTools
            .FirstOrDefault(x => x.Id == new ToolId(id))
            .Should()
            .NotBeNull()
            .And.BeOfType<Tool>()
            .Subject;
        
        var subject = new ToolButtonViewModel(_toolService, tool);

        subject.Activator
            .Activate()
            .DisposeWith(_disposables);
        
        subject.ToolTipHeader
            .Should()
            .Be(tool.ToolTipHeader);
    }
    
    [Theory]
    [InlineData("top-left-1")]
    [InlineData("top-left-2")]
    [InlineData("top-right-1")]
    [InlineData("top-right-2")]
    [InlineData("bottom-left-1")]
    [InlineData("bottom-left-2")]
    [InlineData("bottom-right-1")]
    [InlineData("bottom-right-2")]
    public void ToolTipContent_ShouldEqualExpected(string id)
    {
        var tool = _createdTools
            .FirstOrDefault(x => x.Id == new ToolId(id))
            .Should()
            .NotBeNull()
            .And.BeOfType<Tool>()
            .Subject;
        
        var subject = new ToolButtonViewModel(_toolService, tool);

        subject.Activator
            .Activate()
            .DisposeWith(_disposables);
        
        subject.ToolTipContent
            .Should()
            .Be(tool.ToolTipContent);
    }

    [Theory]
    [InlineData("top-left-1")]
    [InlineData("top-left-2")]
    [InlineData("top-right-1")]
    [InlineData("top-right-2")]
    [InlineData("bottom-left-1")]
    [InlineData("bottom-left-2")]
    [InlineData("bottom-right-1")]
    [InlineData("bottom-right-2")]
    public void ToggleActivationCommand_ShouldToggleToolActivation(string id)
    {
        var tool = _createdTools
            .FirstOrDefault(x => x.Id == new ToolId(id))
            .Should()
            .NotBeNull()
            .And.BeOfType<Tool>()
            .Subject;
        
        var subject = new ToolButtonViewModel(_toolService, tool);

        subject.Activator
            .Activate()
            .DisposeWith(_disposables);
        
        subject.ToggleActivationCommand
            .Execute()
            .Subscribe()
            .DisposeWith(_disposables);

        _toolService.ActiveTools[tool.Position]
            .Value
            .Should()
            .Be(tool);
        
        subject.ToggleActivationCommand
            .Execute()
            .Subscribe()
            .DisposeWith(_disposables);
        
        _toolService.ActiveTools[tool.Position]
            .Value
            .Should()
            .BeNull();
    }

    [Theory]
    [InlineData(nameof(ToolBarPosition.TopLeft))]
    [InlineData(nameof(ToolBarPosition.TopRight))]
    [InlineData(nameof(ToolBarPosition.BottomLeft))]
    [InlineData(nameof(ToolBarPosition.BottomRight))]
    public void ButtonAngle_ShouldReturnExpected(string toolBarPositionName)
    {
        var toolBarPosition = ToolBarPosition.FromName(toolBarPositionName);

        var subject = new ToolButtonViewModel(_toolService, _createdTools[0]);
        
        subject.Activator
            .Activate()
            .DisposeWith(_disposables);
        
        _createdTools[0].Position = toolBarPosition;
        
        subject.ButtonAngle
            .Should()
            .Be(toolBarPosition.ButtonAngle);
    }

    [Theory]
    [InlineData(false, "top-left-1")]
    [InlineData(true, "top-left-1")]
    [InlineData(false, "top-left-2")]
    [InlineData(true, "top-left-2")]
    [InlineData(false, "top-right-1")]
    [InlineData(true, "top-right-1")]
    [InlineData(false, "top-right-2")]
    [InlineData(true, "top-right-2")]
    [InlineData(false, "bottom-left-1")]
    [InlineData(true, "bottom-left-1")]
    [InlineData(false, "bottom-left-2")]
    [InlineData(true, "bottom-left-2")]
    [InlineData(false, "bottom-right-1")]
    [InlineData(true, "bottom-right-1")]
    [InlineData(false, "bottom-right-2")]
    [InlineData(true, "bottom-right-2")]
    public void IsActive_ShouldReturnExpected(bool expected, string id)
    {
        var tool = _createdTools
            .FirstOrDefault(x => x.Id == new ToolId(id))
            .Should()
            .NotBeNull()
            .And.BeOfType<Tool>()
            .Subject;
        
        var subject = new ToolButtonViewModel(_toolService, tool);

        subject.Activator
            .Activate()
            .DisposeWith(_disposables);
        
        tool.IsActive = expected;
        
        subject.IsActive
            .Should()
            .Be(expected);
    }
    
    [Fact]
    public void SplatResolve_ShouldResolveToFactory()
    {
        var factory = Locator.Current.GetService<ToolButtonViewModel.Factory>()!;

        var tool = new Tool
        {
            Id = new ToolId("test"),
            Title = "Test",
            Content = new MockViewModel(),
            Position = ToolBarPosition.TopLeft
        };
        
        var subject1 = factory(tool);
        var subject2 = factory(tool);
        
        subject1.Should().NotBeSameAs(subject2);
    }
}