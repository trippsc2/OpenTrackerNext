using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using OpenTrackerNext.Core.Entities;
using OpenTrackerNext.Core.Icons.States;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.Icons.DoubleEntity;

/// <summary>
/// Represents an icon that represents and manipulates the state of two entities.
/// </summary>
public sealed class DoubleEntityIcon : ReactiveObject, IIcon
{
    private readonly IEntity _entity1;
    private readonly IEntity _entity2;
    private readonly Dictionary<(int, int), IIconState> _iconStates;
    
    private readonly CompositeDisposable _disposables = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="DoubleEntityIcon"/> class.
    /// </summary>
    /// <param name="entity1">
    /// An <see cref="IEntity"/> representing the first entity.
    /// </param>
    /// <param name="entity2">
    /// An <see cref="IEntity"/> representing the second entity.
    /// </param>
    /// <param name="iconStates">
    /// A <see cref="Dictionary{TKey,TValue}"/> of <see cref="IIconState"/> indexed by a
    /// <see cref="ValueTuple"/> of two <see cref="int"/> values representing the icon states.
    /// </param>
    public DoubleEntityIcon(
        IEntity entity1,
        IEntity entity2,
        Dictionary<(int Entity1State, int Entity2State), IIconState> iconStates)
    {
        _entity1 = entity1;
        _entity2 = entity2;
        _iconStates = iconStates;
        
        this.WhenAnyValue(
                x => x._entity1.Current,
                x => x._entity2.Current,
                GetCurrentState)
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
        IncreaseOrCycleEntityCount(_entity1);
    }

    /// <inheritdoc />
    public void OnRightClick()
    {
        IncreaseOrCycleEntityCount(_entity2);
    }

    private static void IncreaseOrCycleEntityCount(IEntity entity)
    {
        if (entity.Current == entity.Maximum)
        {
            entity.Current = entity.Minimum;
            return;
        }
        
        entity.Current++;
    }

    private IIconState GetCurrentState(int entity1Value, int entity2Value)
    {
        return _iconStates.TryGetValue((entity1Value, entity2Value), out var state)
            ? state
            : NullIconState.Instance;
    }
}