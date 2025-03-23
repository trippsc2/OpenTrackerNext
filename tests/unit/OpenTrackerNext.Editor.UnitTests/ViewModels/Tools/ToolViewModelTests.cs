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
public sealed class ToolViewModelTests
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
    
    public ToolViewModelTests()
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
    public void Activation_ShouldSetIsActiveAsExpected(string id)
    {
        var tool = _createdTools
            .FirstOrDefault(x => x.Id == new ToolId(id))
            .Should()
            .NotBeNull()
            .And.BeOfType<Tool>()
            .Subject;
        
        var subject = new ToolViewModel(_toolService, tool);

        tool.IsActive
            .Should()
            .BeFalse();

        var activation = subject.Activator.Activate();

        tool.IsActive
            .Should()
            .BeTrue();
        
        activation.Dispose();
        
        tool.IsActive
            .Should()
            .BeFalse();
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
    public void Tool_ShouldReturnExpected(string id)
    {
        var tool = _createdTools
            .FirstOrDefault(x => x.Id == new ToolId(id))
            .Should()
            .NotBeNull()
            .And.BeOfType<Tool>()
            .Subject;

        var subject = new ToolViewModel(_toolService, tool);

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
    public void Title_ShouldReturnExpected(string id)
    {
        var tool = _createdTools
            .FirstOrDefault(x => x.Id == new ToolId(id))
            .Should()
            .NotBeNull()
            .And.BeOfType<Tool>()
            .Subject;

        var subject = new ToolViewModel(_toolService, tool);

        subject.Activator
            .Activate()
            .DisposeWith(_disposables);
        
        subject.Title
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
    public void Content_ShouldReturnExpected(string id)
    {
        var tool = _createdTools
            .FirstOrDefault(x => x.Id == new ToolId(id))
            .Should()
            .NotBeNull()
            .And.BeOfType<Tool>()
            .Subject;

        var subject = new ToolViewModel(_toolService, tool);

        subject.Activator
            .Activate()
            .DisposeWith(_disposables);
        
        subject.Content
            .Should()
            .Be(tool.Content);
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
    public void DeactivateCommand_ShouldDeactivateTool(string id)
    {
        var tool = _createdTools
            .FirstOrDefault(x => x.Id == new ToolId(id))
            .Should()
            .NotBeNull()
            .And.BeOfType<Tool>()
            .Subject;
        
        _toolService.ActiveTools[tool.Position].Value = tool;

        var subject = new ToolViewModel(_toolService, tool);

        subject.Activator
            .Activate()
            .DisposeWith(_disposables);
        
        subject.DeactivateCommand
            .Execute()
            .Subscribe()
            .DisposeWith(_disposables);

        _toolService.ActiveTools[tool.Position]
            .Value
            .Should()
            .BeNull();
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
    public void EnableDragHitBoxesCommand_ShouldSetDragHitBoxesAreActiveToTrue(string id)
    {
        var tool = _createdTools
            .FirstOrDefault(x => x.Id == new ToolId(id))
            .Should()
            .NotBeNull()
            .And.BeOfType<Tool>()
            .Subject;
        
        var subject = new ToolViewModel(_toolService, tool);

        subject.Activator
            .Activate()
            .DisposeWith(_disposables);
        
        subject.EnableDragHitBoxesCommand
            .Execute()
            .Subscribe()
            .DisposeWith(_disposables);

        _toolService.DragHitBoxesAreActive
            .Should()
            .BeTrue();
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
    public void DisableDragHitBoxesCommand_ShouldSetDragHitBoxesAreActiveToFalse(string id)
    {
        var tool = _createdTools
            .FirstOrDefault(x => x.Id == new ToolId(id))
            .Should()
            .NotBeNull()
            .And.BeOfType<Tool>()
            .Subject;
        
        var subject = new ToolViewModel(_toolService, tool);

        subject.Activator
            .Activate()
            .DisposeWith(_disposables);
        
        _toolService.DragHitBoxesAreActive = true;
        
        subject.DisableDragHitBoxesCommand
            .Execute()
            .Subscribe()
            .DisposeWith(_disposables);

        _toolService.DragHitBoxesAreActive
            .Should()
            .BeFalse();
    }
    
    [Theory]
    [InlineData("top-left-1", nameof(ToolBarPosition.TopRight))]
    [InlineData("top-left-2", nameof(ToolBarPosition.TopRight))]
    [InlineData("top-right-1", nameof(ToolBarPosition.TopLeft))]
    [InlineData("top-right-2", nameof(ToolBarPosition.TopLeft))]
    [InlineData("bottom-left-1", nameof(ToolBarPosition.BottomRight))]
    [InlineData("bottom-left-2", nameof(ToolBarPosition.BottomRight))]
    [InlineData("bottom-right-1", nameof(ToolBarPosition.BottomLeft))]
    [InlineData("bottom-right-2", nameof(ToolBarPosition.BottomLeft))]
    public void MoveCommand_ShouldMoveTool(string id, string newPositionName)
    {
        var newPosition = ToolBarPosition.FromName(newPositionName);
        
        var tool = _createdTools
            .FirstOrDefault(x => x.Id == new ToolId(id))
            .Should()
            .NotBeNull()
            .And.BeOfType<Tool>()
            .Subject;
        
        var oldPosition = tool.Position;
        
        _toolService.ActiveTools[oldPosition].Value = tool;
        
        var subject = new ToolViewModel(_toolService, tool);

        subject.Activator
            .Activate()
            .DisposeWith(_disposables);
        
        subject.MoveCommand
            .Execute(newPosition)
            .Subscribe()
            .DisposeWith(_disposables);

        _toolService.ActiveTools[oldPosition]
            .Value
            .Should()
            .BeNull();
        
        _toolService.ActiveTools[newPosition]
            .Value
            .Should()
            .Be(tool);
    }
    
    [Fact]
    public void SplatResolve_ShouldResolveToFactory()
    {
        var factory = Locator.Current.GetService<ToolViewModel.Factory>()!;
        
        var tool = new Tool
        {
            Id = new ToolId("test"),
            Title = "Test",
            Content = new MockViewModel(),
            Position = ToolBarPosition.TopLeft
        };
        
        var subject1 = factory(tool);
        var subject2 = factory(tool);

        subject1.Should().BeOfType<ToolViewModel>();
        subject2.Should().BeOfType<ToolViewModel>();

        subject1.Should().NotBeSameAs(subject2);
    }
}