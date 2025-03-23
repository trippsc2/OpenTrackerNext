using OpenTrackerNext.Core.Utils;
using ReactiveUI;

namespace OpenTrackerNext.Core.Icons.Labels;

/// <summary>
/// Returns a null <see cref="IIconLabel"/> object.
/// </summary>
public sealed class NullIconLabel : ReactiveObject, IIconLabel, INullObject<NullIconLabel>
{
    private NullIconLabel()
    {
    }
    
    /// <summary>
    /// Gets a singleton instance of the <see cref="NullIconLabel"/> class.
    /// </summary>
    public static NullIconLabel Instance { get; } = new();

    /// <inheritdoc/>
    public string? Text => null;
    
    /// <inheritdoc/>
    public IconLabelPosition Position => IconLabelPosition.BottomRight;

    /// <inheritdoc/>
    public void Dispose()
    {
    }
}