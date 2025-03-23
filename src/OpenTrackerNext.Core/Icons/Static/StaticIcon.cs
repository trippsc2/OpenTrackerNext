using OpenTrackerNext.Core.Icons.States;
using ReactiveUI;

namespace OpenTrackerNext.Core.Icons.Static;

/// <summary>
/// Represents an <see cref="IIcon"/> that does not change state and has no click functionality.
/// </summary>
public sealed class StaticIcon : ReactiveObject, IIcon
{
    /// <inheritdoc/>
    public required IIconState CurrentState { get; init; }

    /// <inheritdoc/>
    public void Dispose()
    {
        CurrentState.Dispose();
    }

    /// <inheritdoc/>
    public void OnLeftClick()
    {
    }

    /// <inheritdoc/>
    public void OnRightClick()
    {
    }
}