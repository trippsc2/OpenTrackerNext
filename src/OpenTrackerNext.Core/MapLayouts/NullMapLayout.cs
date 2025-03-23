using System.Collections.Generic;
using OpenTrackerNext.Core.Maps;
using OpenTrackerNext.Core.Utils;

namespace OpenTrackerNext.Core.MapLayouts;

/// <summary>
/// Represents a null <see cref="IMapLayout"/> object.
/// </summary>
public sealed class NullMapLayout : IMapLayout, INullObject<NullMapLayout>
{
    private NullMapLayout()
    {
    }
    
    /// <summary>
    /// Gets a singleton instance of the <see cref="NullMapLayout"/> class.
    /// </summary>
    public static NullMapLayout Instance { get; } = new();

    /// <inheritdoc />
    public string Name => string.Empty;
    
    /// <inheritdoc />
    public IEnumerable<IMap> Maps { get; } = [];
}