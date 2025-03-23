using OpenTrackerNext.Core.Icons.Labels;
using OpenTrackerNext.Core.Images;
using OpenTrackerNext.Core.Utils;

namespace OpenTrackerNext.Core.Icons.States;

/// <summary>
/// Represents a null <see cref="IIconState"/> object.
/// </summary>
public sealed class NullIconState : IIconState, INullObject<NullIconState>
{
    private NullIconState()
    {
    }

    /// <summary>
    /// Gets a singleton instance of the <see cref="NullIconState"/> class.
    /// </summary>
    public static NullIconState Instance { get; } = new();
    
    /// <inheritdoc/>
    public IIconLabel Label => NullIconLabel.Instance;
    
    /// <inheritdoc/>
    public IReadOnlyImageFile Image => NullImageFile.Instance;

    /// <inheritdoc/>
    public void Dispose()
    {
    }
}