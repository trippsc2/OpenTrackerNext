using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using OpenTrackerNext.Core.Entities;
using OpenTrackerNext.Core.Icons.States;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.Icons.SingleEntity;

/// <summary>
/// Represents an icon that represents and manipulates the state of a single entity with no label.
/// </summary>
public sealed class SingleEntityIcon : ReactiveObject, IIcon
{
    private readonly IEntity _entity;
    private readonly Dictionary<int, IIconState> _iconStates;
    
    private readonly CompositeDisposable _disposables = new();
    
    private readonly Action _leftClickAction;
    private readonly Action _rightClickAction;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="SingleEntityIcon"/> class.
    /// </summary>
    /// <param name="entity">
    /// An <see cref="IEntity"/> representing the entity.
    /// </param>
    /// <param name="iconStates">
    /// A <see cref="Dictionary{TKey,TValue}"/> of <see cref="IIconState"/> indexed by <see cref="int"/>
    /// representing the icon states.
    /// </param>
    /// <param name="swapClickActions">
    /// A <see cref="bool"/> representing whether to swap the left and right click actions.
    /// </param>
    /// <param name="cycleCounts">
    /// A <see cref="bool"/> representing whether to cycle the counts when at minimum/maximum.
    /// </param>
    public SingleEntityIcon(
        IEntity entity,
        Dictionary<int, IIconState> iconStates,
        bool swapClickActions,
        bool cycleCounts)
    {
        _entity = entity;
        _iconStates = iconStates;
        
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
    
    private IIconState GetCurrentState(int entityValue)
    {
        return _iconStates.TryGetValue(entityValue, out var state)
            ? state
            : NullIconState.Instance;
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
}