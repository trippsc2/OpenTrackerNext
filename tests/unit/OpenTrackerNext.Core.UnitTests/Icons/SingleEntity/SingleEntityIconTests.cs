using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using FluentAssertions;
using NSubstitute;
using OpenTrackerNext.Core.Entities;
using OpenTrackerNext.Core.Icons.SingleEntity;
using OpenTrackerNext.Core.Icons.States;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Icons.SingleEntity;

[ExcludeFromCodeCoverage]
public sealed class SingleEntityIconTests : IDisposable
{
    private readonly CompositeDisposable _disposables = new();

    private readonly Entity _entity = new(0, 0, 2);

    private readonly Dictionary<int, IIconState> _iconStates = new()
    {
        { 0, Substitute.For<IIconState>() },
        { 1, Substitute.For<IIconState>() },
        { 2, Substitute.For<IIconState>() }
    };

    public void Dispose()
    {
        _disposables.Dispose();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void CurrentState_ShouldReturnExpected_WhenEntityValueChanges(int entityValue)
    {
        _entity.Current = entityValue;

        var subject = new SingleEntityIcon(_entity, _iconStates, false, false);
            
        subject.DisposeWith(_disposables);
        
        subject.CurrentState
            .Should()
            .Be(_iconStates[entityValue]);
    }
    
    [Fact]
    public void CurrentState_ShouldRaisePropertyChanged_WhenEntityValueChanges()
    {
        var subject = new SingleEntityIcon(_entity, _iconStates, false, false);
        
        subject.DisposeWith(_disposables);
        
        using var monitor = subject.Monitor();
        
        _entity.Current = 1;
        
        monitor.Should().RaisePropertyChangeFor(x => x.CurrentState);
    }
    
    [Fact]
    public void OnLeftClick_ShouldIncreaseEntityValue_WhenEntityValueIsNotMaximumAndActionsAreNotSwapped()
    {
        _entity.Current = 0;
        
        var subject = new SingleEntityIcon(_entity, _iconStates, false, false);
            
        subject.DisposeWith(_disposables);
        
        subject.OnLeftClick();
        
        _entity.Current
            .Should()
            .Be(1);
    }
    
    [Fact]
    public void OnLeftClick_ShouldDecreaseEntityValue_WhenEntityValueIsNotMinimumAndActionsAreSwapped()
    {
        _entity.Current = 1;
        
        var subject = new SingleEntityIcon(_entity, _iconStates, true, false);
            
        subject.DisposeWith(_disposables);
        
        subject.OnLeftClick();
        
        _entity.Current
            .Should()
            .Be(0);
    }
    
    [Fact]
    public void OnLeftClick_ShouldDoNothing_WhenEntityValueIsMaximumAndActionsAreNotSwappedAndDoNotCycleCounts()
    {
        _entity.Current = _entity.Maximum;
        
        var subject = new SingleEntityIcon(_entity, _iconStates, false, false);
            
        subject.DisposeWith(_disposables);
        
        subject.OnLeftClick();
        
        _entity.Current
            .Should()
            .Be(_entity.Maximum);
    }
    
    [Fact]
    public void OnLeftClick_ShouldSetEntityValueToMinimum_WhenEntityValueIsMaximumAndActionsAreNotSwappedAndCycleCounts()
    {
        _entity.Current = _entity.Maximum;
        
        var subject = new SingleEntityIcon(_entity, _iconStates, false, true);
            
        subject.DisposeWith(_disposables);
        
        subject.OnLeftClick();
        
        _entity.Current
            .Should()
            .Be(_entity.Minimum);
    }
    
    [Fact]
    public void OnLeftClick_ShouldDoNothing_WhenEntityValueIsMinimumAndActionsAreSwappedAndDoNotCycleCounts()
    {
        _entity.Current = _entity.Minimum;
        
        var subject = new SingleEntityIcon(_entity, _iconStates, true, false);
            
        subject.DisposeWith(_disposables);
        
        subject.OnLeftClick();
        
        _entity.Current
            .Should()
            .Be(_entity.Minimum);
    }
    
    [Fact]
    public void OnLeftClick_ShouldSetEntityValueToMaximum_WhenEntityValueIsMinimumAndActionsAreSwappedAndCycleCounts()
    {
        _entity.Current = _entity.Minimum;
        
        var subject = new SingleEntityIcon(_entity, _iconStates, true, true);
            
        subject.DisposeWith(_disposables);
        
        subject.OnLeftClick();
        
        _entity.Current
            .Should()
            .Be(_entity.Maximum);
    }
    
    [Fact]
    public void OnRightClick_ShouldDecreaseEntityValue_WhenEntityValueIsNotMinimumAndActionsAreNotSwapped()
    {
        _entity.Current = 1;
        
        var subject = new SingleEntityIcon(_entity, _iconStates, false, false);
            
        subject.DisposeWith(_disposables);
        
        subject.OnRightClick();
        
        _entity.Current
            .Should()
            .Be(0);
    }
    
    [Fact]
    public void OnRightClick_ShouldIncreaseEntityValue_WhenEntityValueIsNotMaximumAndActionsAreSwapped()
    {
        _entity.Current = 1;
        
        var subject = new SingleEntityIcon(_entity, _iconStates, true, false);
            
        subject.DisposeWith(_disposables);
        
        subject.OnRightClick();
        
        _entity.Current
            .Should()
            .Be(2);
    }
    
    [Fact]
    public void OnRightClick_ShouldDoNothing_WhenEntityValueIsMinimumAndActionsAreNotSwappedAndDoNotCycleCounts()
    {
        _entity.Current = _entity.Minimum;
        
        var subject = new SingleEntityIcon(_entity, _iconStates, false, false);
            
        subject.DisposeWith(_disposables);
        
        subject.OnRightClick();
        
        _entity.Current
            .Should()
            .Be(_entity.Minimum);
    }
    
    [Fact]
    public void OnRightClick_ShouldSetEntityValueToMaximum_WhenEntityValueIsMinimumAndActionsAreNotSwappedAndCycleCounts()
    {
        _entity.Current = _entity.Minimum;
        
        var subject = new SingleEntityIcon(_entity, _iconStates, false, true);
            
        subject.DisposeWith(_disposables);
        
        subject.OnRightClick();
        
        _entity.Current
            .Should()
            .Be(_entity.Maximum);
    }
    
    [Fact]
    public void OnRightClick_ShouldDoNothing_WhenEntityValueIsMaximumAndActionsAreSwappedAndDoNotCycleCounts()
    {
        _entity.Current = _entity.Maximum;
        
        var subject = new SingleEntityIcon(_entity, _iconStates, true, false);
            
        subject.DisposeWith(_disposables);
        
        subject.OnRightClick();
        
        _entity.Current
            .Should()
            .Be(_entity.Maximum);
    }
    
    [Fact]
    public void OnRightClick_ShouldSetEntityValueToMinimum_WhenEntityValueIsMaximumAndActionsAreSwappedAndCycleCounts()
    {
        _entity.Current = _entity.Maximum;
        
        var subject = new SingleEntityIcon(_entity, _iconStates, true, true);
            
        subject.DisposeWith(_disposables);
        
        subject.OnRightClick();
        
        _entity.Current
            .Should()
            .Be(_entity.Minimum);
    }
}