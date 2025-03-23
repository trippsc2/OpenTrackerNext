using OpenTrackerNext.Core.Images;
using OpenTrackerNext.Core.Utils;
using ReactiveUI;

namespace OpenTrackerNext.Core.Maps;

/// <summary>
/// Represents a null <see cref="IMap"/> object.
/// </summary>
public sealed class NullMap : ReactiveObject, IMap, INullObject<NullMap>
{
    private NullMap()
    {
    }
    
    /// <summary>
    /// Gets a singleton instance of the <see cref="NullMap"/> class.
    /// </summary>
    public static NullMap Instance { get; } = new();
    
    /// <inheritdoc/>
    public IReadOnlyImageFile Image => NullImageFile.Instance;

    /// <inheritdoc/>
    public void Dispose()
    {
    }
}