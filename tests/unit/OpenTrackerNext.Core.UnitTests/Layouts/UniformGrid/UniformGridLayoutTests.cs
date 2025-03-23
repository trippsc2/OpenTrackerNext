using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using Avalonia;
using FluentAssertions;
using OpenTrackerNext.Core.Layouts;
using OpenTrackerNext.Core.Layouts.UniformGrid;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Layouts.UniformGrid;

[ExcludeFromCodeCoverage]
public sealed class UniformGridLayoutTests : IDisposable
{
    private readonly CompositeDisposable _disposables = new();

    public void Dispose()
    {
        _disposables.Dispose();
    }
    
    [Theory]
    [InlineData(0, 0, 0, 0)]
    [InlineData(1, 2, 3, 4)]
    public void Margin_ShouldBeInitializedFromConstructor(double left, double top, double right, double bottom)
    {
        var margin = new Thickness(left, top, right, bottom);

        var subject = new UniformGridLayout
        {
            Margin = margin,
            Columns = 1,
            Rows = 1,
            Items = new List<ILayout>()
        };
        
        subject.DisposeWith(_disposables);

        subject.Margin
            .Should()
            .Be(margin);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Columns_ShouldBeInitializedFromConstructor(int expected)
    {
        var subject = new UniformGridLayout
        {
            Margin = new Thickness(),
            Columns = expected,
            Rows = 1,
            Items = new List<ILayout>()
        };
        
        subject.DisposeWith(_disposables);

        subject.Columns
            .Should()
            .Be(expected);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Rows_ShouldBeInitializedFromConstructor(int expected)
    {
        var subject = new UniformGridLayout
        {
            Margin = new Thickness(),
            Columns = 1,
            Rows = expected,
            Items = new List<ILayout>()
        };
        
        subject.DisposeWith(_disposables);

        subject.Rows
            .Should()
            .Be(expected);
    }
    
    [Fact]
    public void Items_ShouldBeInitializedFromConstructor()
    {
        var items = new List<ILayout>();

        var subject = new UniformGridLayout
        {
            Margin = new Thickness(),
            Columns = 1,
            Rows = 1,
            Items = items
        };
        
        subject.DisposeWith(_disposables);

        subject.Items
            .Should()
            .BeSameAs(items);
    }
}