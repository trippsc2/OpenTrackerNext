using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using FluentAssertions;
using NSubstitute;
using OpenTrackerNext.Core.Icons;
using OpenTrackerNext.Core.Layouts.IconGrid.Dynamic;
using OpenTrackerNext.Core.Requirements;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Layouts.IconGrid.Dynamic;

[ExcludeFromCodeCoverage]
public sealed class DynamicIconLayoutTests : IDisposable
{
    private readonly CompositeDisposable _disposables = new();
    
    private readonly IRequirement _requirement = Substitute.For<IRequirement>();
    private readonly IIcon _metIcon = Substitute.For<IIcon>();
    private readonly IIcon _unmetIcon = Substitute.For<IIcon>();

    private readonly DynamicIconLayout _subject;

    public DynamicIconLayoutTests()
    {
        _subject = new DynamicIconLayout(_requirement, _metIcon, _unmetIcon);
            
        _subject.DisposeWith(_disposables);
    }
    
    public void Dispose()
    {
        _disposables.Dispose();
    }
    
    [Theory]
    [InlineData(16)]
    [InlineData(32)]
    public void Height_ShouldReturnExpected(int expected)
    {
        _subject.Height = expected;
            
        _subject.Height
            .Should()
            .Be(expected);
    }

    [Fact]
    public void Height_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();

        _subject.Height = 32;
        
        monitor.Should().RaisePropertyChangeFor(x => x.Height);
    }
    
    [Theory]
    [InlineData(16)]
    [InlineData(32)]
    public void Width_ShouldReturnExpected(int expected)
    {
        _subject.Width = expected;
            
        _subject.Width
            .Should()
            .Be(expected);
    }
    
    [Fact]
    public void Width_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();

        _subject.Width = 32;
        
        monitor.Should().RaisePropertyChangeFor(x => x.Width);
    }
    
    [Fact]
    public void CurrentIcon_ShouldReturnExpected_WhenRequirementIsMet()
    {
        _requirement.IsMet.Returns(true);

        _requirement.PropertyChanged += Raise
            .Event<PropertyChangedEventHandler>(
                _requirement,
                new PropertyChangedEventArgs(nameof(IRequirement.IsMet)));
        
        _subject.CurrentIcon
            .Should()
            .Be(_metIcon);
    }
    
    [Fact]
    public void CurrentIcon_ShouldReturnExpected_WhenRequirementIsNotMet()
    {
        _requirement.IsMet.Returns(false);

        _requirement.PropertyChanged += Raise
            .Event<PropertyChangedEventHandler>(
                _requirement,
                new PropertyChangedEventArgs(nameof(IRequirement.IsMet)));
        
        _subject.CurrentIcon
            .Should()
            .Be(_unmetIcon);
    }
}