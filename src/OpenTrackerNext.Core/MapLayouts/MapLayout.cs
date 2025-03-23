using System.Collections.Generic;
using OpenTrackerNext.Core.Maps;
using ReactiveUI;

namespace OpenTrackerNext.Core.MapLayouts;

/// <inheritdoc cref="IMapLayout" />
public sealed class MapLayout : ReactiveObject, IMapLayout
{
    /// <inheritdoc/>
    public required string Name { get; init; }
    
    /// <inheritdoc/>
    public required IEnumerable<IMap> Maps { get; init; }
}