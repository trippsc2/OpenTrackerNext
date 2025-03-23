using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Disposables;
using DynamicData;
using FluentAssertions;
using NSubstitute;
using OpenTrackerNext.Core.ViewModels;
using OpenTrackerNext.Editor.Tools;
using Splat;
using Xunit;

namespace OpenTrackerNext.Editor.UnitTests.Tools;

[ExcludeFromCodeCoverage]
public sealed class ToolServiceTests
{
    private sealed class MockViewModel : ViewModel;
    
    private readonly CompositeDisposable _disposables = new();
    
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
    
    private readonly IToolFactory _toolFactory = Substitute.For<IToolFactory>();
    
    private readonly ReadOnlyObservableCollection<Tool> _derivedList;
    
    private readonly ToolService _subject;

    public ToolServiceTests()
    {
        _toolFactory.CreateTools().Returns(_createdTools);
        
        _subject = new ToolService(_toolFactory);

        _subject.Connect()
            .Bind(out _derivedList)
            .Subscribe()
            .DisposeWith(_disposables);
    }

    [Fact]
    public void DragHitBoxesAreActive_ShouldInitializeToFalse()
    {
        _subject.DragHitBoxesAreActive
            .Should()
            .BeFalse();
    }

    [Fact]
    public void DragHitBoxesAreActive_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.DragHitBoxesAreActive = true;
        
        monitor.Should().RaisePropertyChangeFor(x => x.DragHitBoxesAreActive);
    }

    [Fact]
    public void Connect_ShouldProvideObservableToCreateDerivedList()
    {
        _derivedList.Should().Contain(_createdTools);
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
    public void ToggleToolActivation_ShouldActivateTool_WhenToolIsNotActive(string toolId)
    {
        var tool = _createdTools.First(x => x.Id.Value == toolId);
        
        _subject.ToggleToolActivation(tool);
        
        _subject.ActiveTools[tool.Position]
            .Value
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
    public void ToggleToolActivation_ShouldDeactivateTool_WhenToolIsActive(string toolId)
    {
        var tool = _createdTools.First(x => x.Id.Value == toolId);
        
        _subject.ToggleToolActivation(tool);
        
        _subject.ToggleToolActivation(tool);
        
        _subject.ActiveTools[tool.Position]
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
    public void DeactivateTool_ShouldDeactivateTool_WhenToolIsActive(string toolId)
    {
        var tool = _createdTools.First(x => x.Id.Value == toolId);
        
        _subject.ToggleToolActivation(tool);
        
        _subject.DeactivateTool(tool);
        
        _subject.ActiveTools[tool.Position]
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
    public void DeactivateTool_ShouldDoNothing_WhenNoToolIsActiveInPosition(string toolId)
    {
        var tool = _createdTools.First(x => x.Id.Value == toolId);
        
        _subject.DeactivateTool(tool);
        
        _subject.ActiveTools[tool.Position]
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
    public void DeactivateTool_ShouldDoNothing_WhenAnotherToolIsActiveInPosition(string toolId)
    {
        var tool = _createdTools.First(x => x.Id.Value == toolId);
        var activeTool = _createdTools.First(x => x != tool && x.Position == tool.Position);
        
        _subject.ToggleToolActivation(activeTool);
        
        _subject.DeactivateTool(tool);

        _subject.ActiveTools[tool.Position]
            .Value
            .Should()
            .Be(activeTool);
    }

    [Theory]
    [InlineData("top-left-1", nameof(ToolBarPosition.BottomLeft))]
    [InlineData("top-left-1", nameof(ToolBarPosition.TopRight))]
    [InlineData("bottom-left-2", nameof(ToolBarPosition.TopRight))]
    [InlineData("bottom-left-2", nameof(ToolBarPosition.BottomRight))]
    public void MoveTool_ShouldDeactivateMoveAndReactivateTool(string toolId, string positionName)
    {
        var position = ToolBarPosition.FromName(positionName);
        
        var tool = _createdTools.First(x => x.Id.Value == toolId);
        
        var oldPosition = tool.Position;
        
        _subject.ToggleToolActivation(tool);
        
        _subject.MoveTool(tool, position);

        _subject.ActiveTools[oldPosition]
            .Value
            .Should()
            .BeNull();
        
        _subject.ActiveTools[position]
            .Value
            .Should()
            .Be(tool);
        
        tool.Position
            .Should()
            .Be(position);
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
    public void MoveTool_ShouldDoNothing_WhenToolIsAlreadyInPosition(string toolId)
    {
        var tool = _createdTools.First(x => x.Id.Value == toolId);
        
        var position = tool.Position;
        
        _subject.MoveTool(tool, position);

        _subject.ActiveTools[position]
            .Value
            .Should()
            .BeNull();
        
        tool.Position
            .Should()
            .Be(position);
    }
    
    [Fact]
    public void DeactivateAllTools_ShouldDoNothing_WhenNoToolsAreActive()
    {
        _subject.DeactivateAllTools();

        _subject.ActiveTools[ToolBarPosition.TopLeft]
            .Value
            .Should()
            .BeNull();
        
        _subject.ActiveTools[ToolBarPosition.TopRight]
            .Value
            .Should()
            .BeNull();
        
        _subject.ActiveTools[ToolBarPosition.BottomLeft]
            .Value
            .Should()
            .BeNull();
        
        _subject.ActiveTools[ToolBarPosition.BottomRight]
            .Value
            .Should()
            .BeNull();
    }

    [Fact]
    public void DeactivateAllTools_ShouldDeactivateAllTools_WhenToolsAreActive()
    {
        var topLeft = _createdTools.First(x => x.Position == ToolBarPosition.TopLeft);
        var topRight = _createdTools.First(x => x.Position == ToolBarPosition.TopRight);
        var bottomLeft = _createdTools.First(x => x.Position == ToolBarPosition.BottomLeft);
        var bottomRight = _createdTools.First(x => x.Position == ToolBarPosition.BottomRight);
        
        _subject.ToggleToolActivation(topLeft);
        _subject.ToggleToolActivation(topRight);
        _subject.ToggleToolActivation(bottomLeft);
        _subject.ToggleToolActivation(bottomRight);
        
        _subject.DeactivateAllTools();
        
        _subject.ActiveTools[ToolBarPosition.TopLeft]
            .Value
            .Should()
            .BeNull();
        
        _subject.ActiveTools[ToolBarPosition.TopRight]
            .Value
            .Should()
            .BeNull();
        
        _subject.ActiveTools[ToolBarPosition.BottomLeft]
            .Value
            .Should()
            .BeNull();
        
        _subject.ActiveTools[ToolBarPosition.BottomRight]
            .Value
            .Should()
            .BeNull();
    }
    
    [Fact]
    public void SplatResolve_ShouldResolveToSingleInstance()
    {
        var subject1 = Locator.Current.GetService<IToolService>()!;
        var subject2 = Locator.Current.GetService<IToolService>()!;

        subject1.Should().BeOfType<ToolService>();
        subject2.Should().BeOfType<ToolService>();
        
        subject1.Should().BeSameAs(subject2);
    }
    
    [Fact]
    public void SplatResolve_ShouldResolveToLazySingleInstance()
    {
        var lazy = Locator.Current.GetService<Lazy<IToolService>>()!;
        var subject = Locator.Current.GetService<IToolService>()!;
        
        lazy.Value
            .Should()
            .BeOfType<ToolService>();
        
        subject.Should().BeOfType<ToolService>();

        lazy.Value
            .Should()
            .BeSameAs(subject);
    }
}