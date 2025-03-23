using OpenTrackerNext.Core.Icons.States;
using OpenTrackerNext.Core.Utils;
using ReactiveUI;

namespace OpenTrackerNext.Core.Icons;

/// <summary>
/// Represents a null <see cref="IIcon"/> object.
/// </summary>
public sealed class NullIcon : ReactiveObject, IIcon, INullObject<NullIcon>
{
    private NullIcon()
    {
    }

    /// <summary>
    /// Gets a singleton instance of the <see cref="NullIcon"/> class.
    /// </summary>
    public static NullIcon Instance { get; } = new();

    /// <inheritdoc/>
    public IIconState CurrentState => NullIconState.Instance;
    
    /// <inheritdoc/>
    public void Dispose()
    {
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