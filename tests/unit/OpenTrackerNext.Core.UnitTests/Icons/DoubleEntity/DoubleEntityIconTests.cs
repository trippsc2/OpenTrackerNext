using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using FluentAssertions;
using NSubstitute;
using OpenTrackerNext.Core.Entities;
using OpenTrackerNext.Core.Icons.DoubleEntity;
using OpenTrackerNext.Core.Icons.States;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Icons.DoubleEntity;

[ExcludeFromCodeCoverage]
public sealed class DoubleEntityIconTests : IDisposable
{
    private readonly CompositeDisposable _disposables = new();

    private readonly Entity _entity1 = new(0, 0, 1);
    
    private readonly Entity _entity2 = new(0, 0, 2);

    private readonly Dictionary<(int, int), IIconState> _iconStates = new()
    {
        { (0, 0), Substitute.For<IIconState>() },
        { (0, 1), Substitute.For<IIconState>() },
        { (0, 2), Substitute.For<IIconState>() },
        { (1, 0), Substitute.For<IIconState>() },
        { (1, 1), Substitute.For<IIconState>() },
        { (1, 2), Substitute.For<IIconState>() }
    };
    
    public void Dispose()
    {
        _disposables.Dispose();
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0, 1)]
    [InlineData(0, 2)]
    [InlineData(1, 0)]
    [InlineData(1, 1)]
    [InlineData(1, 2)]
    public void CurrentState_ShouldReturnExpected_WhenEntityValuesChange(int entity1Value, int entity2Value)
    {
        _entity1.Current = entity1Value;
        _entity2.Current = entity2Value;
        
        var subject = new DoubleEntityIcon(_entity1, _entity2, _iconStates);
        
        subject.DisposeWith(_disposables);
        
        subject.CurrentState
            .Should()
            .Be(_iconStates[(entity1Value, entity2Value)]);
    }

    [Fact]
    public void CurrentState_ShouldRaisePropertyChanged_WhenChanged()
    {
        var subject = new DoubleEntityIcon(_entity1, _entity2, _iconStates);
        
        subject.DisposeWith(_disposables);
        
        using var monitor = subject.Monitor();
        
        _entity1.Current = 1;
        
        monitor.Should().RaisePropertyChangeFor(x => x.CurrentState);
    }
    
    [Fact]
    public void OnLeftClick_ShouldIncreaseEntity1Value_WhenNotAtMaximum()
    {
        var subject = new DoubleEntityIcon(_entity1, _entity2, _iconStates);
        
        subject.DisposeWith(_disposables);
        
        subject.OnLeftClick();
        
        _entity1.Current
            .Should()
            .Be(1);
    }
    
    [Fact]
    public void OnLeftClick_ShouldSetEntity1ValueToMinimum_WhenAtMaximum()
    {
        _entity1.Current = _entity1.Maximum;
        
        var subject = new DoubleEntityIcon(_entity1, _entity2, _iconStates);
        
        subject.DisposeWith(_disposables);
        
        subject.OnLeftClick();
        
        _entity1.Current
            .Should()
            .Be(_entity1.Minimum);
    }
    
    [Fact]
    public void OnRightClick_ShouldIncreaseEntity2Value_WhenNotAtMaximum()
    {
        var subject = new DoubleEntityIcon(_entity1, _entity2, _iconStates);
        
        subject.DisposeWith(_disposables);
        
        subject.OnRightClick();
        
        _entity2.Current
            .Should()
            .Be(1);
    }
    
    [Fact]
    public void OnRightClick_ShouldSetEntity2ValueToMinimum_WhenAtMaximum()
    {
        _entity2.Current = _entity2.Maximum;
        
        var subject = new DoubleEntityIcon(_entity1, _entity2, _iconStates);
        
        subject.DisposeWith(_disposables);
        
        subject.OnRightClick();
        
        _entity2.Current
            .Should()
            .Be(_entity2.Minimum);
    }
}