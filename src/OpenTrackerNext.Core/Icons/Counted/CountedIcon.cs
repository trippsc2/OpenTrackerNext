using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using OpenTrackerNext.Core.Entities;
using OpenTrackerNext.Core.Icons.States;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.Icons.Counted;

/// <summary>
/// Represents an icon with a count label.
/// </summary>
public sealed class CountedIcon : ReactiveObject, IIcon
{
    private readonly IEntity _entity;
    private readonly IIconState _disabledState;
    private readonly IIconState _defaultState;
    
    private readonly CompositeDisposable _disposables = new();
    
    private readonly Action _leftClickAction;
    private readonly Action _rightClickAction;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="CountedIcon"/> class.
    /// </summary>
    /// <param name="entity">
    /// An <see cref="IEntity"/> representing the entity.
    /// </param>
    /// <param name="disabledState">
    /// An <see cref="IIconState"/> representing the disabled state.
    /// </param>
    /// <param name="defaultState">
    /// An <see cref="IIconState"/> representing the default state.
    /// </param>
    /// <param name="swapClickActions">
    /// A <see cref="bool"/> representing whether to swap the left and right click actions.
    /// </param>
    /// <param name="cycleCounts">
    /// A <see cref="bool"/> representing whether to cycle the counts when at minimum/maximum.
    /// </param>
    public CountedIcon(
        IEntity entity,
        IIconState disabledState,
        IIconState defaultState,
        bool swapClickActions,
        bool cycleCounts)
    {
        _entity = entity;
        _disabledState = disabledState;
        _defaultState = defaultState;
        
        _leftClickAction = swapClickActions
            ? () => DecreaseCount(cycleCounts)
            : () => IncreaseCount(cycleCounts);
        _rightClickAction = swapClickActions
            ? () => IncreaseCount(cycleCounts)
            : () => DecreaseCount(cycleCounts);

        this.WhenAnyValue(x => x._entity.Current)
            .Select(GetCurrentState)
            .ToPropertyEx(this, x => x.CurrentState)
            .DisposeWith(_disposables);
    }
    
    /// <inheritdoc />
    [ObservableAsProperty]
    public IIconState CurrentState { get; } = null!;
    
    /// <inheritdoc />
    public void Dispose()
    {
        _disposables.Dispose();
    }

    /// <inheritdoc />
    public void OnLeftClick()
    {
        _leftClickAction();
    }

    /// <inheritdoc />
    public void OnRightClick()
    {
        _rightClickAction();
    }
    
    private void IncreaseCount(bool cycleCounts)
    {
        if (_entity.Current == _entity.Maximum)
        {
            if (!cycleCounts)
            {
                return;
            }
            
            _entity.Current = _entity.Minimum;
            return;
        }

        _entity.Current++;
    }

    private void DecreaseCount(bool cycleCounts)
    {
        if (_entity.Current == _entity.Minimum)
        {
            if (!cycleCounts)
            {
                return;
            }
            
            _entity.Current = _entity.Maximum;
            return;
        }
        
        _entity.Current--;
    }

    private IIconState GetCurrentState(int entityValue)
    {
        return _entity.Minimum == entityValue
            ? _disabledState
            : _defaultState;
    }
}