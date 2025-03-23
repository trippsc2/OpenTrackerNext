using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using OpenTrackerNext.Core.ViewModels;
using OpenTrackerNext.Editor.Tools;
using Xunit;

namespace OpenTrackerNext.Editor.UnitTests.Tools;

[ExcludeFromCodeCoverage]
public sealed class ToolTests
{
    private sealed class MockViewModel : ViewModel;
    
    private readonly MockViewModel _content = new();
    
    private readonly Tool _subject;
    
    public ToolTests()
    {
        _subject = new Tool
        {
            Id = new ToolId("test"),
            Title = "Test",
            Position = ToolBarPosition.TopLeft,
            Content = _content,
            ToolTipHeader = "Header",
            ToolTipContent = "Content"
        };
    }
    
    [Fact]
    public void Id_ShouldReturnExpected()
    {
        _subject.Id
            .Should()
            .Be(new ToolId("test"));
    }
    
    [Fact]
    public void Title_ShouldReturnExpected()
    {
        _subject.Title
            .Should()
            .Be("Test");
    }
    
    [Fact]
    public void Content_ShouldReturnExpected()
    {
        _subject.Content
            .Should()
            .Be(_content);
    }
    
    [Fact]
    public void ToolTipHeader_ShouldReturnExpected()
    {
        _subject.ToolTipHeader
            .Should()
            .Be("Header");
    }
    
    [Fact]
    public void ToolTipContent_ShouldReturnExpected()
    {
        _subject.ToolTipContent
            .Should()
            .Be("Content");
    }
    
    [Fact]
    public void IsActive_ShouldDefaultToFalse()
    {
        _subject.IsActive
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void IsActive_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.IsActive = true;

        monitor.Should().RaisePropertyChangeFor(x => x.IsActive);
    }
    
    [Fact]
    public void Position_ShouldReturnExpected()
    {
        _subject.Position
            .Should()
            .Be(ToolBarPosition.TopLeft);
    }
    
    [Fact]
    public void Position_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.Position = ToolBarPosition.TopRight;

        monitor.Should().RaisePropertyChangeFor(x => x.Position);
    }
}