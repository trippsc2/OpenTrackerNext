using System.Reactive.Disposables;
using System.Reactive.Linq;
using OpenTrackerNext.Core.Entities;
using OpenTrackerNext.Core.Icons.Counted;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.Icons.Labels;

/// <summary>
/// Represents a <see cref="CountedIcon"/> label.
/// </summary>
public sealed class CountedIconLabel : ReactiveObject, IIconLabel
{
    private readonly CompositeDisposable _disposables = new();
    
    private readonly IEntity _entity;
    private readonly bool _hideLabelAtMinimum;
    private readonly bool _addAsteriskAtMaximum;
    private readonly IndeterminateState _indeterminateState;

    /// <summary>
    /// Initializes a new instance of the <see cref="CountedIconLabel"/> class.
    /// </summary>
    /// <param name="entity">
    /// An <see cref="IEntity"/> representing the entity from which the label is derived.
    /// </param>
    /// <param name="hideLabelAtMinimum">
    /// A <see cref="bool"/> representing whether the label is hidden at the minimum entity value.
    /// If the label has <see cref="IndeterminateState.Minimum"/>, the label is hidden for the value one after
    /// minimum.
    /// </param>
    /// <param name="addAsteriskAtMaximum">
    /// A <see cref="bool"/> representing whether the label should have an asterisk at the maximum entity value.
    /// If the label has <see cref="IndeterminateState.Maximum"/>, the asterisk is added for the value one before
    /// maximum.
    /// </param>
    /// <param name="indeterminateState">
    /// A <see cref="IndeterminateState"/> representing whether and where the entity has an indeterminate state.
    /// </param>
    public CountedIconLabel(
        IEntity entity,
        bool hideLabelAtMinimum,
        bool addAsteriskAtMaximum,
        IndeterminateState indeterminateState)
    {
        _entity = entity;
        _hideLabelAtMinimum = hideLabelAtMinimum;
        _addAsteriskAtMaximum = addAsteriskAtMaximum;
        _indeterminateState = indeterminateState;

        this.WhenAnyValue(x => x._entity.Current)
            .Select(GetText)
            .ToPropertyEx(this, x => x.Text)
            .DisposeWith(_disposables);
    }

    /// <inheritdoc/>
    public required IconLabelPosition Position { get; init; }

    /// <inheritdoc/>
    [ObservableAsProperty]
    public string? Text { get; }
    
    /// <inheritdoc/>
    public void Dispose()
    {
        _disposables.Dispose();
    }

    private string? GetText(int value)
    {
        if ((_indeterminateState == IndeterminateState.Minimum && value == _entity.Minimum) ||
            (_indeterminateState == IndeterminateState.Maximum && value == _entity.Maximum))
        {
            return "?";
        }

        if (_hideLabelAtMinimum &&
            (value == _entity.Minimum ||
             (_indeterminateState == IndeterminateState.Minimum && value == _entity.Minimum + 1)))
        {
            return null;
        }
        
        if (_addAsteriskAtMaximum &&
            (value == _entity.Maximum ||
             (_indeterminateState == IndeterminateState.Maximum && value == _entity.Maximum - 1)))
        {
            return $"{value}*";
        }
        
        return value.ToString();
    }
}