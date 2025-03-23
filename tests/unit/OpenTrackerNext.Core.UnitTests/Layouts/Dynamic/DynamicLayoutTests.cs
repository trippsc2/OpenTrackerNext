using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using Avalonia;
using FluentAssertions;
using NSubstitute;
using OpenTrackerNext.Core.Layouts;
using OpenTrackerNext.Core.Layouts.Dynamic;
using OpenTrackerNext.Core.Requirements;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Layouts.Dynamic;

[ExcludeFromCodeCoverage]
public sealed class DynamicLayoutTests : IDisposable
{
    private readonly CompositeDisposable _disposables = new();
    private readonly IRequirement _requirement = Substitute.For<IRequirement>();
    private readonly ILayout _metLayout = Substitute.For<ILayout>();
    private readonly ILayout _unmetLayout = Substitute.For<ILayout>();

    private readonly DynamicLayout _subject;

    public DynamicLayoutTests()
    {
        _subject = new DynamicLayout(_requirement, _metLayout, _unmetLayout);
            
        _subject.DisposeWith(_disposables);
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }

    [Fact]
    public void Margin_ShouldReturnExpected()
    {
        _subject.Margin
            .Should()
            .Be(default(Thickness));
    }
    
    [Fact]
    public void CurrentLayout_ShouldReturnExpected_WhenRequirementIsMet()
    {
        _requirement.IsMet.Returns(true);

        _requirement.PropertyChanged += Raise
            .Event<PropertyChangedEventHandler>(
                _requirement,
                new PropertyChangedEventArgs(nameof(IRequirement.IsMet)));
        
        _subject.CurrentLayout
            .Should()
            .Be(_metLayout);
    }
    
    [Fact]
    public void CurrentLayout_ShouldReturnExpected_WhenRequirementIsNotMet()
    {
        _requirement.IsMet.Returns(false);

        _requirement.PropertyChanged += Raise
            .Event<PropertyChangedEventHandler>(
                _requirement,
                new PropertyChangedEventArgs(nameof(IRequirement.IsMet)));
        
        _subject.CurrentLayout
            .Should()
            .Be(_unmetLayout);
    }
}