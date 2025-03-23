using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using FluentAssertions;
using NSubstitute;
using OpenTrackerNext.Core.Entities;
using OpenTrackerNext.Core.Icons.Counted;
using OpenTrackerNext.Core.Icons.States;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Icons.Counted;

[ExcludeFromCodeCoverage]
public sealed class CountedIconTests : IDisposable
{
    private readonly CompositeDisposable _disposables = new();

    private readonly Entity _entity = new(0, 0, 5);

    private readonly IIconState _disabledState = Substitute.For<IIconState>();
    private readonly IIconState _defaultState = Substitute.For<IIconState>();

    public void Dispose()
    {
        _disposables.Dispose();
    }

    [Fact]
    public void CurrentState_ShouldReturnDisabledState_WhenEntityValueIsMinimum()
    {
        _entity.Current = _entity.Minimum;

        var subject = new CountedIcon(
            _entity,
            _disabledState,
            _defaultState,
            false,
            false);
            
        subject.DisposeWith(_disposables);

        subject.CurrentState
            .Should()
            .Be(_disabledState);
    }
    
    [Fact]
    public void CurrentState_ShouldReturnDefaultState_WhenEntityValueIsNotMinimum()
    {
        _entity.Current = 1;

        var subject = new CountedIcon(
            _entity,
            _disabledState,
            _defaultState,
            false,
            false);
        
        subject.DisposeWith(_disposables);

        subject.CurrentState
            .Should()
            .Be(_defaultState);
    }

    [Fact]
    public void CurrentState_ShouldRaisePropertyChanged_WhenChanged()
    {
        var subject = new CountedIcon(
            _entity,
            _disabledState,
            _defaultState,
            false,
            false);
        
        subject.DisposeWith(_disposables);
        
        using var monitor = subject.Monitor();
        
        _entity.Current = _entity.Maximum;
        
        monitor.Should().RaisePropertyChangeFor(x => x.CurrentState);
    }
    
    [Fact]
    public void OnLeftClick_ShouldIncreaseEntityValue_WhenEntityValueIsNotMaximumAndActionsAreNotSwapped()
    {
        _entity.Current = 1;

        var subject = new CountedIcon(
            _entity,
            _disabledState,
            _defaultState,
            false,
            false);
        
        subject.DisposeWith(_disposables);
        
        subject.OnLeftClick();
        
        _entity.Current
            .Should()
            .Be(2);
    }
    
    [Fact]
    public void OnLeftClick_ShouldDecreaseEntityValue_WhenEntityValueIsNotMinimumAndActionsAreSwapped()
    {
        _entity.Current = 1;

        var subject = new CountedIcon(
            _entity,
            _disabledState,
            _defaultState,
            true,
            false);
        
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

        var subject = new CountedIcon(_entity, _disabledState, _defaultState, false, false);
        
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

        var subject = new CountedIcon(_entity, _disabledState, _defaultState, false, true);
        
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

        var subject = new CountedIcon(_entity, _disabledState, _defaultState, true, false);
        
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

        var subject = new CountedIcon(_entity, _disabledState, _defaultState, true, true);
        
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

        var subject = new CountedIcon(_entity, _disabledState, _defaultState, false, false);
        
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

        var subject = new CountedIcon(_entity, _disabledState, _defaultState, true, false);
        
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
        
        var subject = new CountedIcon(_entity, _disabledState, _defaultState, false, false);
            
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

        var subject = new CountedIcon(_entity, _disabledState, _defaultState, false, true);
        
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
        
        var subject = new CountedIcon(_entity, _disabledState, _defaultState, true, false);
            
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

        var subject = new CountedIcon(_entity, _disabledState, _defaultState, true, true);
        
        subject.DisposeWith(_disposables);
        
        subject.OnRightClick();
        
        _entity.Current
            .Should()
            .Be(_entity.Minimum);
    }
}