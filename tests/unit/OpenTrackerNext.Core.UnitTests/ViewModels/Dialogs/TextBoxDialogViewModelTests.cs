using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Disposables;
using FluentAssertions;
using OpenTrackerNext.Core.Validation;
using OpenTrackerNext.Core.ViewModels.Dialogs;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.ViewModels.Dialogs;

[ExcludeFromCodeCoverage]
public sealed class TextBoxDialogViewModelTests : IDisposable
{
    private readonly CompositeDisposable _disposables = new();
    
    private const string Title = "Test";
    private const string Message = "Test message";
    
    private readonly TextBoxDialogViewModel _subject;

    public TextBoxDialogViewModelTests()
    {
        var validationRules = new List<ValidationRule<string?>>
        {
            new()
            {
                Rule = x => x != "Cheese",
                FailureMessage = "No cheese!"
            }
        };
        
        _subject = new TextBoxDialogViewModel(null, validationRules)
        {
            Title = Title,
            Message = Message
        };
        
        _subject.Activator
            .Activate()
            .DisposeWith(_disposables);
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }
    
    [Fact]
    public void MaxWidth_ShouldDefaultTo480()
    {
        _subject.MaxWidth
            .Should()
            .Be(480.0);
    }

    [Theory]
    [InlineData(450.0)]
    [InlineData(500.0)]
    public void MaxWidth_ShouldReturnExpected_WhenInitialized(double maxWidth)
    {
        var subject = new TextBoxDialogViewModel
        {
            MaxWidth = maxWidth,
            Title = Title,
            Message = Message
        };
        
        subject.MaxWidth
            .Should()
            .Be(maxWidth);
    }

    [Fact]
    public void MinWidth_ShouldDefaultTo410()
    {
        _subject.MinWidth.Should().Be(410.0);
    }

    [Theory]
    [InlineData(400.0)]
    [InlineData(350.0)]
    public void MinWidth_ShouldReturnExpected_WhenInitialized(double minWidth)
    {
        var subject = new TextBoxDialogViewModel
        {
            MinWidth = minWidth,
            Title = Title,
            Message = Message
        };
        
        subject.MinWidth
            .Should()
            .Be(minWidth);
    }
    
    [Fact]
    public void Title_ShouldReturnExpected()
    {
        _subject.Title
            .Should()
            .Be(Title);
    }

    [Fact]
    public void Message_ShouldReturnExpected()
    {
        _subject.Message
            .Should()
            .Be(Message);
    }

    [Theory]
    [InlineData(false, null)]
    [InlineData(true, nameof(DialogIcon.Question))]
    [InlineData(true, nameof(DialogIcon.Error))]
    public void IconIsVisible_ShouldReturnExpected(bool expected, string? iconName)
    {
        var icon = iconName is not null
            ? DialogIcon.FromName(iconName)
            : null;
        
        var subject = new TextBoxDialogViewModel(icon)
        {
            Title = Title,
            Message = Message
        };
        
        subject.IconIsVisible
            .Should()
            .Be(expected);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData(nameof(DialogIcon.Question))]
    [InlineData(nameof(DialogIcon.Error))]
    public void IconValue_ShouldReturnExpected(string? iconName)
    {
        var icon = iconName is not null
            ? DialogIcon.FromName(iconName)
            : null;
        
        var subject = new TextBoxDialogViewModel(icon)
        {
            Title = Title,
            Message = Message
        };
        
        subject.IconValue
            .Should()
            .Be(icon?.IconValue);
    }
    
    [Fact]
    public void InputText_ShouldValidateAgainstRules()
    {
        _subject.ValidationContext
            .IsValid
            .Should()
            .BeTrue();
        
        _subject.InputText = "Cheese";
        
        _subject.ValidationContext
            .IsValid
            .Should()
            .BeFalse();
    }

    [Fact]
    public void OkCommand_ShouldRequestClose()
    {
        var closeRequested = false;
        
        _subject.CloseInteraction
            .RegisterHandler(context =>
            {
                closeRequested = true;
                context.SetOutput(Unit.Default);
            })
            .DisposeWith(_disposables);
        
        _subject.OkCommand!
            .Execute()
            .Subscribe()
            .DisposeWith(_disposables);

        closeRequested.Should().BeTrue();
    }
    
    [Fact]
    public void OkCommand_ShouldBeDisabled_WhenInputTextIsInvalid()
    {
        var canExecute = true;

        _subject.OkCommand!
            .CanExecute
            .Subscribe(x => canExecute = x)
            .DisposeWith(_disposables);
        
        _subject.InputText = "Cheese";
        
        canExecute.Should().BeFalse();
    }
    
    [Fact]
    public void OkCommand_ShouldBeEnabled_WhenInputTextIsValid()
    {
        var canExecute = false;

        _subject.OkCommand!
            .CanExecute
            .Subscribe(x => canExecute = x)
            .DisposeWith(_disposables);
        
        _subject.InputText = "Cheese";
        _subject.InputText = "Not cheese";
        
        canExecute.Should().BeTrue();
    }
    
    [Fact]
    public void CancelCommand_ShouldRequestClose()
    {
        var closeRequested = false;
        
        _subject.CloseInteraction
            .RegisterHandler(context =>
            {
                closeRequested = true;
                context.SetOutput(Unit.Default);
            })
            .DisposeWith(_disposables);
        
        _subject.CancelCommand!
            .Execute()
            .Subscribe()
            .DisposeWith(_disposables);

        closeRequested.Should().BeTrue();
    }
}