using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Disposables;
using FluentAssertions;
using NSubstitute;
using OpenTrackerNext.Core.Factories;
using OpenTrackerNext.Core.ViewModels;
using OpenTrackerNext.Editor.Documents;
using OpenTrackerNext.Editor.Tools;
using OpenTrackerNext.Editor.ViewModels.Documents;
using OpenTrackerNext.Editor.ViewModels.Tools;
using Splat;
using Xunit;

namespace OpenTrackerNext.Editor.UnitTests.ViewModels.Tools;

[ExcludeFromCodeCoverage]
public sealed class ToolLayoutViewModelTests
{
    private sealed class MockViewModel : ViewModel;
    
    private readonly CompositeDisposable _disposables = new();
    private readonly IDocumentService _documentService = Substitute.For<IDocumentService>();
    private readonly IFactory<IDocument, ViewModel> _documentViewModelFactory =
        Substitute.For<IFactory<IDocument, ViewModel>>();
    private readonly IToolFactory _toolFactory = Substitute.For<IToolFactory>();
    private readonly ToolService _toolService;
    private readonly DocumentLayoutViewModel _centerContent;
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

    private readonly ToolLayoutViewModel _subject;

    public ToolLayoutViewModelTests()
    {
        _centerContent = new DocumentLayoutViewModel(_documentService, DocumentDockFactory);
        _toolFactory.CreateTools().Returns(_createdTools);
        _toolService = new ToolService(_toolFactory);
        
        _subject = new ToolLayoutViewModel(
            _toolService,
            _centerContent,
            ToolBarFactory,
            ToolPaneFactory);

        _subject.Activator
            .Activate()
            .DisposeWith(_disposables);

        _subject.TopLeftToolBar
            .Activator
            .Activate()
            .DisposeWith(_disposables);

        _subject.TopRightToolBar
            .Activator
            .Activate()
            .DisposeWith(_disposables);

        _subject.BottomLeftToolBar
            .Activator
            .Activate()
            .DisposeWith(_disposables);

        _subject.BottomRightToolBar
            .Activator
            .Activate()
            .DisposeWith(_disposables);

        _subject.LeftToolPane
            .Activator
            .Activate()
            .DisposeWith(_disposables);

        _subject.RightToolPane
            .Activator
            .Activate()
            .DisposeWith(_disposables);

        _subject.CenterContent
            .Activator
            .Activate()
            .DisposeWith(_disposables);
    }

    private DocumentBodyViewModel DocumentBodyFactory(IDocument document)
    {
        return new DocumentBodyViewModel(_documentViewModelFactory, document);
    }
    
    private DocumentViewModel DocumentFactory(IDocument document)
    {
        return new DocumentViewModel(_documentService, DocumentBodyFactory, document);
    }
    
    private DocumentDockViewModel DocumentDockFactory(DocumentSide side)
    {
        return new DocumentDockViewModel(_documentService, DocumentFactory, side);
    }

    private ToolButtonViewModel ToolButtonFactory(Tool tool)
    {
        return new ToolButtonViewModel(_toolService, tool);
    }

    private ToolBarViewModel ToolBarFactory(ToolBarPosition position)
    {
        return new ToolBarViewModel(_toolService, ToolButtonFactory, position);
    }

    private ToolViewModel ToolFactory(Tool tool)
    {
        return new ToolViewModel(_toolService, tool);
    }

    private ToolPaneViewModel ToolPaneFactory(ToolPaneSide side)
    {
        return new ToolPaneViewModel(_toolService, ToolFactory, side);
    }
    
    [Fact]
    public void TopLeftToolBar_ShouldEqualExpected()
    {
        _subject.TopLeftToolBar
            .Should()
            .BeOfType<ToolBarViewModel>()
            .Subject
            .Buttons
            .Should()
            .HaveCount(2);
    }
    
    [Fact]
    public void TopRightToolBar_ShouldEqualExpected()
    {
        _subject.TopRightToolBar
            .Should()
            .BeOfType<ToolBarViewModel>()
            .Subject
            .Buttons
            .Should()
            .HaveCount(2);
    }
    
    [Fact]
    public void BottomLeftToolBar_ShouldEqualExpected()
    {
        _subject.BottomLeftToolBar
            .Should()
            .BeOfType<ToolBarViewModel>()
            .Subject
            .Buttons
            .Should()
            .HaveCount(2);
    }
    
    [Fact]
    public void BottomRightToolBar_ShouldEqualExpected()
    {
        _subject.BottomRightToolBar
            .Should()
            .BeOfType<ToolBarViewModel>()
            .Subject
            .Buttons
            .Should()
            .HaveCount(2);
    }
    
    [Fact]
    public void LeftToolPane_ShouldEqualExpected()
    {
        _subject.LeftToolPane
            .Should()
            .BeOfType<ToolPaneViewModel>();

        _subject.LeftToolPane
            .Top
            .Should()
            .BeNull();
        
        _subject.LeftToolPane
            .Bottom
            .Should()
            .BeNull();
        
        var topLeftTool = _createdTools
            .FirstOrDefault(x => x.Position == ToolBarPosition.TopLeft)
            .Should()
            .NotBeNull()
            .And.BeOfType<Tool>()
            .Subject;

        var bottomLeftTool = _createdTools
            .FirstOrDefault(x => x.Position == ToolBarPosition.BottomLeft)
            .Should()
            .NotBeNull()
            .And.BeOfType<Tool>()
            .Subject;

        _toolService.ActiveTools[ToolBarPosition.TopLeft].Value = topLeftTool;
        
        _subject.LeftToolPane
            .Top
            .Should()
            .BeOfType<ToolViewModel>()
            .Subject
            .Tool
            .Should()
            .Be(topLeftTool);
        
        _toolService.ActiveTools[ToolBarPosition.BottomLeft].Value = bottomLeftTool;
        
        _subject.LeftToolPane
            .Bottom
            .Should()
            .BeOfType<ToolViewModel>()
            .Subject
            .Tool
            .Should()
            .Be(bottomLeftTool);
    }
    
    [Fact]
    public void RightToolPane_ShouldEqualExpected()
    {
        _subject.RightToolPane
            .Should()
            .BeOfType<ToolPaneViewModel>();

        _subject.RightToolPane
            .Top
            .Should()
            .BeNull();
        
        _subject.RightToolPane
            .Bottom
            .Should()
            .BeNull();
        
        var topRightTool = _createdTools
            .FirstOrDefault(x => x.Position == ToolBarPosition.TopRight)
            .Should()
            .NotBeNull()
            .And.BeOfType<Tool>()
            .Subject;

        var bottomRightTool = _createdTools
            .FirstOrDefault(x => x.Position == ToolBarPosition.BottomRight)
            .Should()
            .NotBeNull()
            .And.BeOfType<Tool>()
            .Subject;

        _toolService.ActiveTools[ToolBarPosition.TopRight].Value = topRightTool;
        
        _subject.RightToolPane
            .Top
            .Should()
            .BeOfType<ToolViewModel>()
            .Subject
            .Tool
            .Should()
            .Be(topRightTool);
        
        _toolService.ActiveTools[ToolBarPosition.BottomRight].Value = bottomRightTool;
        
        _subject.RightToolPane
            .Bottom
            .Should()
            .BeOfType<ToolViewModel>()
            .Subject
            .Tool
            .Should()
            .Be(bottomRightTool);
    }
    
    [Fact]
    public void CenterContent_ShouldEqualExpected()
    {
        _subject.CenterContent
            .Should()
            .Be(_centerContent);
    }

    [Fact]
    public void LeftToolPaneIsVisible_ShouldReturnExpected()
    {
        _subject.LeftToolPaneIsVisible
            .Should()
            .BeFalse();
        
        var topLeftTool = _createdTools
            .FirstOrDefault(x => x.Position == ToolBarPosition.TopLeft)
            .Should()
            .NotBeNull()
            .And.BeOfType<Tool>()
            .Subject;
        
        var bottomLeftTool = _createdTools
            .FirstOrDefault(x => x.Position == ToolBarPosition.BottomLeft)
            .Should()
            .NotBeNull()
            .And.BeOfType<Tool>()
            .Subject;
        
        _toolService.ActiveTools[ToolBarPosition.TopLeft].Value = topLeftTool;
        
        _subject.LeftToolPaneIsVisible
            .Should()
            .BeTrue();
        
        _toolService.ActiveTools[ToolBarPosition.BottomLeft].Value = bottomLeftTool;
        
        _subject.LeftToolPaneIsVisible
            .Should()
            .BeTrue();
        
        _toolService.ActiveTools[ToolBarPosition.TopLeft].Value = null;
        
        _subject.LeftToolPaneIsVisible
            .Should()
            .BeTrue();
        
        _toolService.ActiveTools[ToolBarPosition.BottomLeft].Value = null;
        
        _subject.LeftToolPaneIsVisible
            .Should()
            .BeFalse();
    }

    [Fact]
    public void RightToolPaneIsVisible_ShouldReturnExpected()
    {
        _subject.RightToolPaneIsVisible
            .Should()
            .BeFalse();
        
        var topRightTool = _createdTools
            .FirstOrDefault(x => x.Position == ToolBarPosition.TopRight)
            .Should()
            .NotBeNull()
            .And.BeOfType<Tool>()
            .Subject;
        
        var bottomRightTool = _createdTools
            .FirstOrDefault(x => x.Position == ToolBarPosition.BottomRight)
            .Should()
            .NotBeNull()
            .And.BeOfType<Tool>()
            .Subject;
        
        _toolService.ActiveTools[ToolBarPosition.TopRight].Value = topRightTool;
        
        _subject.RightToolPaneIsVisible
            .Should()
            .BeTrue();
        
        _toolService.ActiveTools[ToolBarPosition.BottomRight].Value = bottomRightTool;
        
        _subject.RightToolPaneIsVisible
            .Should()
            .BeTrue();
        
        _toolService.ActiveTools[ToolBarPosition.TopRight].Value = null;
        
        _subject.RightToolPaneIsVisible
            .Should()
            .BeTrue();
        
        _toolService.ActiveTools[ToolBarPosition.BottomRight].Value = null;
        
        _subject.RightToolPaneIsVisible
            .Should()
            .BeFalse();
    }

    [Fact]
    public void CenterColumn_ShouldReturnExpected()
    {
        _subject.CenterColumn
            .Should()
            .Be(1);
        
        var topLeftTool = _createdTools
            .FirstOrDefault(x => x.Position == ToolBarPosition.TopLeft)
            .Should()
            .NotBeNull()
            .And.BeOfType<Tool>()
            .Subject;
        
        var bottomLeftTool = _createdTools
            .FirstOrDefault(x => x.Position == ToolBarPosition.BottomLeft)
            .Should()
            .NotBeNull()
            .And.BeOfType<Tool>()
            .Subject;
        
        _toolService.ActiveTools[ToolBarPosition.TopLeft].Value = topLeftTool;
        
        _subject.CenterColumn
            .Should()
            .Be(3);
        
        _toolService.ActiveTools[ToolBarPosition.BottomLeft].Value = bottomLeftTool;
        
        _subject.CenterColumn
            .Should()
            .Be(3);
        
        _toolService.ActiveTools[ToolBarPosition.TopLeft].Value = null;
        
        _subject.CenterColumn
            .Should()
            .Be(3);
        
        _toolService.ActiveTools[ToolBarPosition.BottomLeft].Value = null;
        
        _subject.CenterColumn
            .Should()
            .Be(1);
    }

    [Fact]
    public void CenterColumnSpan_ShouldReturnExpected()
    {
        _subject.CenterColumnSpan
            .Should()
            .Be(5);
        
        var topLeftTool = _createdTools
            .FirstOrDefault(x => x.Position == ToolBarPosition.TopLeft)
            .Should()
            .NotBeNull()
            .And.BeOfType<Tool>()
            .Subject;
        
        var bottomLeftTool = _createdTools
            .FirstOrDefault(x => x.Position == ToolBarPosition.BottomLeft)
            .Should()
            .NotBeNull()
            .And.BeOfType<Tool>()
            .Subject;
        
        var topRightTool = _createdTools
            .FirstOrDefault(x => x.Position == ToolBarPosition.TopRight)
            .Should()
            .NotBeNull()
            .And.BeOfType<Tool>()
            .Subject;
        
        var bottomRightTool = _createdTools
            .FirstOrDefault(x => x.Position == ToolBarPosition.BottomRight)
            .Should()
            .NotBeNull()
            .And.BeOfType<Tool>()
            .Subject;
        
        _toolService.ActiveTools[ToolBarPosition.TopLeft].Value = topLeftTool;
        
        _subject.CenterColumnSpan
            .Should()
            .Be(3);
        
        _toolService.ActiveTools[ToolBarPosition.BottomLeft].Value = bottomLeftTool;
        
        _subject.CenterColumnSpan
            .Should()
            .Be(3);
        
        _toolService.ActiveTools[ToolBarPosition.TopLeft].Value = null;
        
        _subject.CenterColumnSpan
            .Should()
            .Be(3);
        
        _toolService.ActiveTools[ToolBarPosition.BottomLeft].Value = null;
        
        _subject.CenterColumnSpan
            .Should()
            .Be(5);
        
        _toolService.ActiveTools[ToolBarPosition.TopRight].Value = topRightTool;
        
        _subject.CenterColumnSpan
            .Should()
            .Be(3);
        
        _toolService.ActiveTools[ToolBarPosition.BottomRight].Value = bottomRightTool;
        
        _subject.CenterColumnSpan
            .Should()
            .Be(3);
        
        _toolService.ActiveTools[ToolBarPosition.TopRight].Value = null;
        
        _subject.CenterColumnSpan
            .Should()
            .Be(3);
        
        _toolService.ActiveTools[ToolBarPosition.BottomRight].Value = null;
        
        _subject.CenterColumnSpan
            .Should()
            .Be(5);
        
        _toolService.ActiveTools[ToolBarPosition.TopLeft].Value = topLeftTool;
        _toolService.ActiveTools[ToolBarPosition.BottomLeft].Value = bottomLeftTool;
        _toolService.ActiveTools[ToolBarPosition.TopRight].Value = topRightTool;
        _toolService.ActiveTools[ToolBarPosition.BottomRight].Value = bottomRightTool;
        
        _subject.CenterColumnSpan
            .Should()
            .Be(1);
    }

    [Fact]
    public void DragHitBoxesAreActive_ShouldReturnExpected()
    {
        _subject.DragHitBoxesAreActive
            .Should()
            .BeFalse();
        
        _toolService.DragHitBoxesAreActive = true;
        
        _subject.DragHitBoxesAreActive
            .Should()
            .BeTrue();
        
        _toolService.DragHitBoxesAreActive = false;
        
        _subject.DragHitBoxesAreActive
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void SplatResolve_ShouldResolveToTransientInstance()
    {
        var subject1 = Locator.Current.GetService<ToolLayoutViewModel>()!;
        var subject2 = Locator.Current.GetService<ToolLayoutViewModel>()!;
        
        subject1.Should().NotBeSameAs(subject2);
    }
}