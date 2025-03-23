using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using FluentAssertions;
using NSubstitute;
using OpenTrackerNext.Core.Icons;
using OpenTrackerNext.Core.Layouts.IconGrid.Static;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Layouts.IconGrid.Static;

[ExcludeFromCodeCoverage]
public sealed class StaticIconLayoutTests : IDisposable
{
    private readonly CompositeDisposable _disposables = new();
    
    public void Dispose()
    {
        _disposables.Dispose();
    }
    
    [Theory]
    [InlineData(16)]
    [InlineData(32)]
    public void Height_ShouldReturnExpected(int expected)
    {
        var subject = new StaticIconLayout
        {
            Height = expected,
            CurrentIcon = Substitute.For<IIcon>()
        };
        
        subject.DisposeWith(_disposables);

        subject.Height
            .Should()
            .Be(expected);
    }
    
    [Fact]
    public void Height_ShouldRaisePropertyChanged()
    {
        var subject = new StaticIconLayout { CurrentIcon = Substitute.For<IIcon>() };
        
        subject.DisposeWith(_disposables);

        using var monitor = subject.Monitor();

        subject.Height = 32;
        
        monitor.Should().RaisePropertyChangeFor(x => x.Height);
    }
    
    [Theory]
    [InlineData(16)]
    [InlineData(32)]
    public void Width_ShouldReturnExpected(int expected)
    {
        var subject = new StaticIconLayout
        {
            Width = expected,
            CurrentIcon = Substitute.For<IIcon>()
        };
        
        subject.DisposeWith(_disposables);

        subject.Width
            .Should()
            .Be(expected);
    }
    
    [Fact]
    public void Width_ShouldRaisePropertyChanged()
    {
        var subject = new StaticIconLayout { CurrentIcon = Substitute.For<IIcon>() };
        
        subject.DisposeWith(_disposables);

        using var monitor = subject.Monitor();

        subject.Width = 32;
        
        monitor.Should().RaisePropertyChangeFor(x => x.Width);
    }
    
    [Fact]
    public void CurrentIcon_ShouldBeInitializedFromConstructor()
    {
        var icon = Substitute.For<IIcon>();
        
        var subject = new StaticIconLayout { CurrentIcon = icon };
        
        subject.DisposeWith(_disposables);

        subject.CurrentIcon
            .Should()
            .Be(icon);
    }
}