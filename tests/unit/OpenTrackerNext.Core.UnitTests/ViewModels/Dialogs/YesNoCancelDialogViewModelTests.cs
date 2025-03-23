using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Disposables;
using FluentAssertions;
using OpenTrackerNext.Core.Results;
using OpenTrackerNext.Core.ViewModels.Dialogs;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.ViewModels.Dialogs;

[ExcludeFromCodeCoverage]
public sealed class YesNoCancelDialogViewModelTests : IDisposable
{
    private const string Title = "Test";
    private const string Message = "Test message";
    
    private readonly CompositeDisposable _disposables = new();
    
    private readonly YesNoCancelDialogViewModel _subject = new(null)
    {
        Title = Title,
        Message = Message
    };

    public YesNoCancelDialogViewModelTests()
    {
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
        var subject = new YesNoCancelDialogViewModel(null)
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
        
        var subject = new YesNoCancelDialogViewModel(icon)
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
        
        var subject = new YesNoCancelDialogViewModel(icon)
        {
            Title = Title,
            Message = Message
        };
        
        subject.IconValue
            .Should()
            .Be(icon?.IconValue);
    }

    [Fact]
    public void YesCommand_ShouldReturnYes()
    {
        YesNoCancelDialogResult? result = null;

        _subject.CloseInteraction.RegisterHandler(context =>
        {
            context.SetOutput(Unit.Default);
            result = context.Input;
        });

        _subject.YesCommand!
            .Execute()
            .Subscribe()
            .DisposeWith(_disposables);

        result.Should().NotBeNull();
        
        result!.Value
            .Should()
            .BeOfType<Yes>();
    }
    
    [Fact]
    public void NoCommand_ShouldReturnNo()
    {
        YesNoCancelDialogResult? result = null;

        _subject.CloseInteraction.RegisterHandler(context =>
        {
            context.SetOutput(Unit.Default);
            result = context.Input;
        });

        _subject.NoCommand!
            .Execute()
            .Subscribe()
            .DisposeWith(_disposables);

        result.Should().NotBeNull();
        
        result!.Value
            .Should()
            .BeOfType<No>();
    }
    
    [Fact]
    public void CancelCommand_ShouldReturnCancel()
    {
        YesNoCancelDialogResult? result = null;

        _subject.CloseInteraction.RegisterHandler(context =>
        {
            context.SetOutput(Unit.Default);
            result = context.Input;
        });

        _subject.CancelCommand!
            .Execute()
            .Subscribe()
            .DisposeWith(_disposables);

        result.Should().NotBeNull();
        
        result!.Value
            .Should()
            .BeOfType<Cancel>();
    }
}