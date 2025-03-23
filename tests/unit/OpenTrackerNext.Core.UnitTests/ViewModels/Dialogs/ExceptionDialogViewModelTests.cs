using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Disposables;
using FluentAssertions;
using OpenTrackerNext.Core.ViewModels.Dialogs;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.ViewModels.Dialogs;

[ExcludeFromCodeCoverage]
public sealed class ExceptionDialogViewModelTests : IDisposable
{
    private readonly CompositeDisposable _disposables = new();

    public void Dispose()
    {
        _disposables.Dispose();
    }
    
    [Fact]
    public void Title_ShouldReturnExpected_WhenTitleIsNotProvidedInConstructor()
    {
        var subject = new ExceptionDialogViewModel(new Exception("Test"));

        subject.Title
            .Should()
            .Be("Exception");
    }
    
    [Fact]
    public void Title_ShouldReturnExpected_WhenTitleIsProvidedInConstructor()
    {
        const string title = "Test Title";
        
        var subject = new ExceptionDialogViewModel(new Exception("Test"), title);

        subject.Title
            .Should()
            .Be(title);
    }
    
    [Fact]
    public void Message_ShouldReturnExpected()
    {
        const string message = "Test Message";
        
        var subject = new ExceptionDialogViewModel(new Exception(message));

        subject.Message
            .Should()
            .Be(message);
    }

    [Fact]
    public void StackTrace_ShouldReturnExpected_WhenStackTraceIsEmpty()
    {
        var subject = new ExceptionDialogViewModel(new Exception("Test"));
        
        subject.StackTrace
            .Should()
            .BeNull();
    }
    
    [Fact]
    public void StackTrace_ShouldReturnExpected_WhenStackTraceIsNotEmpty()
    {
        try
        {
            throw new Exception();
        }
        catch (Exception exception)
        {
            var subject = new ExceptionDialogViewModel(exception);
        
            subject.StackTrace
                .Should()
                .Be(exception.StackTrace);
        }
    }
    
    [Fact]
    public void StackTraceIsVisible_ShouldReturnExpected_WhenStackTraceIsEmpty()
    {
        var subject = new ExceptionDialogViewModel(new Exception("Test"));
        
        subject.StackTraceIsVisible
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void StackTraceIsVisible_ShouldReturnExpected_WhenStackTraceIsNotEmpty()
    {
        try
        {
            throw new Exception();
        }
        catch (Exception exception)
        {
            var subject = new ExceptionDialogViewModel(exception);
        
            subject.StackTraceIsVisible
                .Should()
                .BeTrue();
        }
    }
    
    [Fact]
    public void CloseCommand_ShouldInvokeCloseInteraction()
    {
        var subject = new ExceptionDialogViewModel(new Exception("Test"));

        subject.Activator
            .Activate()
            .DisposeWith(_disposables);
        
        var closeInteraction = false;
        
        subject.CloseInteraction.RegisterHandler(
            interaction =>
            {
                closeInteraction = true;
                interaction.SetOutput(Unit.Default);
            });
        
        subject.CloseCommand!
            .Execute()
            .Subscribe()
            .DisposeWith(_disposables);
        
        closeInteraction.Should().BeTrue();
    }
}