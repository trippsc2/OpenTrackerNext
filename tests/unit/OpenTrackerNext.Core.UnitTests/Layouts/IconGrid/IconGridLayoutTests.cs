using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using Avalonia;
using FluentAssertions;
using NSubstitute;
using OpenTrackerNext.Core.Layouts.IconGrid;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Layouts.IconGrid;

[ExcludeFromCodeCoverage]
public sealed class IconGridLayoutTests : IDisposable
{
    private readonly CompositeDisposable _disposables = new();
    
    public void Dispose()
    {
        _disposables.Dispose();
    }
    
    [Theory]
    [InlineData(0, 0, 0, 0)]
    [InlineData(1, 2, 3, 4)]
    public void Margin_ShouldBeInitializedFromConstructor(int left, int top, int right, int bottom)
    {
        var subject = new IconGridLayout
        {
            Margin = new Thickness(left, top, right, bottom),
            HorizontalSpacing = 0.0,
            VerticalSpacing = 0.0,
            Icons = new List<IReadOnlyList<IIconLayout>>()
        };
            
        subject.DisposeWith(_disposables);

        subject.Margin
            .Should()
            .Be(new Thickness(left, top, right, bottom));
    }
    
    [Theory]
    [InlineData(0.0)]
    [InlineData(1.0)]
    [InlineData(2.0)]
    public void HorizontalSpacing_ShouldBeInitializedFromConstructor(double expected)
    {
        var subject = new IconGridLayout
        {
            Margin = new Thickness(),
            HorizontalSpacing = expected,
            VerticalSpacing = 0.0,
            Icons = new List<IReadOnlyList<IIconLayout>>()
        };
        
        subject.DisposeWith(_disposables);

        subject.HorizontalSpacing
            .Should()
            .Be(expected);
    }
    
    [Theory]
    [InlineData(0.0)]
    [InlineData(1.0)]
    [InlineData(2.0)]
    public void VerticalSpacing_ShouldBeInitializedFromConstructor(double expected)
    {
        var subject = new IconGridLayout
        {
            Margin = new Thickness(),
            HorizontalSpacing = 0.0,
            VerticalSpacing = expected,
            Icons = new List<IReadOnlyList<IIconLayout>>()
        };
        
        subject.DisposeWith(_disposables);

        subject.VerticalSpacing
            .Should()
            .Be(expected);
    }
    
    [Fact]
    public void Icons_ShouldBeInitializedFromConstructor()
    {
        var icons = new List<IReadOnlyList<IIconLayout>>();
        var subject = new IconGridLayout
        {
            Margin = new Thickness(),
            HorizontalSpacing = 0.0,
            VerticalSpacing = 0.0,
            Icons = icons
        };
        
        subject.DisposeWith(_disposables);
        
        subject.Icons
            .Should()
            .BeSameAs(icons);
    }
    
    [Fact]
    public void Dispose_ShouldDisposeIcons()
    {
        var icons = new List<IReadOnlyList<IIconLayout>>
        {
            new List<IIconLayout> { Substitute.For<IIconLayout>() }
        };
        
        var subject = new IconGridLayout
        {
            Margin = new Thickness(),
            HorizontalSpacing = 0.0,
            VerticalSpacing = 0.0,
            Icons = icons
        };
        
        subject.Dispose();
        
        icons[0][0].Received().Dispose();
    }
}