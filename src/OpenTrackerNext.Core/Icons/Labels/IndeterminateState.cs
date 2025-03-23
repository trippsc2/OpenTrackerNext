using System.Text.Json.Serialization;
using Ardalis.SmartEnum;
using Ardalis.SmartEnum.SystemTextJson;
using OpenTrackerNext.Core.Icons.Counted;
using OpenTrackerNext.Core.Utils;

namespace OpenTrackerNext.Core.Icons.Labels;

/// <summary>
/// Represents the indeterminate state of a <see cref="CountedIcon"/>.
/// </summary>
[JsonConverter(typeof(SmartEnumNameConverter<IndeterminateState, int>))]
public sealed class IndeterminateState : SmartEnum<IndeterminateState>, INamedSmartEnum
{
    /// <summary>
    /// An <see cref="IndeterminateState"/> representing that the entity does not have an indeterminate state.
    /// </summary>
    public static readonly IndeterminateState None;

    /// <summary>
    /// An <see cref="IndeterminateState"/> representing that the entity has an indeterminate state at its minimum
    /// value.
    /// </summary>
    public static readonly IndeterminateState Minimum;

    /// <summary>
    /// An <see cref="IndeterminateState"/> representing that the entity has an indeterminate state at its maximum
    /// value.
    /// </summary>
    public static readonly IndeterminateState Maximum;

    static IndeterminateState()
    {
        None = new IndeterminateState(nameof(None), 0);
        Minimum = new IndeterminateState(nameof(Minimum), 1);
        Maximum = new IndeterminateState(nameof(Maximum), 2);
    }
    
    private IndeterminateState(string name, int value)
        : base(name, value)
    {
        DisplayName = name;
    }

    /// <inheritdoc/>
    public string DisplayName { get; }
}