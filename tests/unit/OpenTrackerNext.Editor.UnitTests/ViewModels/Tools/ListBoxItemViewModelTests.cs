using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using OpenTrackerNext.Core.ViewModels.Menus;
using OpenTrackerNext.Editor.ViewModels.Tools;
using ReactiveUI;
using Xunit;

namespace OpenTrackerNext.Editor.UnitTests.ViewModels.Tools;

[ExcludeFromCodeCoverage]
public sealed class ListBoxItemViewModelTests
{
    [Theory]
    [InlineData("Test")]
    [InlineData("Test2")]
    public void Text_ShouldReturnExpected(string expected)
    {
        var subject = new ListBoxItemViewModel
        {
            Text = expected,
            DoubleTapCommand = ReactiveCommand.Create(() => { })
        };
        
        subject.Text
            .Should()
            .Be(expected);
    }

    [Fact]
    public void ContextMenuItems_ShouldBeEmpty_WhenNotInitialized()
    {
        var subject = new ListBoxItemViewModel
        {
            Text = "Test",
            DoubleTapCommand = ReactiveCommand.Create(() => { })
        };
        
        subject.ContextMenuItems
            .Should()
            .BeEmpty();
    }
    
    [Fact]
    public void ContextMenuItems_ShouldReturnExpected_WhenInitialized()
    {
        var expected = new ObservableCollection<MenuItemViewModel>();
        
        var subject = new ListBoxItemViewModel
        {
            Text = "Test",
            ContextMenuItems = expected,
            DoubleTapCommand = ReactiveCommand.Create(() => { })
        };
        
        subject.ContextMenuItems
            .Should()
            .BeSameAs(expected);
    }
    
    [Fact]
    public void DoubleTapCommand_ShouldReturnExpected()
    {
        var expected = ReactiveCommand.Create(() => { });
        
        var subject = new ListBoxItemViewModel
        {
            Text = "Test",
            DoubleTapCommand = expected
        };
        
        subject.DoubleTapCommand
            .Should()
            .BeSameAs(expected);
    }
    
    [Fact]
    public void GetViewType_ShouldReturnExpected()
    {
        var subject = new ListBoxItemViewModel
        {
            Text = "Test",
            DoubleTapCommand = ReactiveCommand.Create(() => { })
        };
        
        subject.GetViewType()
            .Should()
            .Be(typeof(IViewFor<IListBoxItemViewModel>));
    }
}