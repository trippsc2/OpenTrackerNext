using System.Collections.Generic;
using OpenTrackerNext.Core.Maps;

namespace OpenTrackerNext.Core.MapLayouts;

/// <summary>
/// Represents a map layout.
/// </summary>
public interface IMapLayout
{
    /// <summary>
    /// Gets a <see cref="string"/> representing the name of the map layout.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets an <see cref="IEnumerable{T}"/> of <see cref="IMap"/> representing the maps of the map layout.
    /// </summary>
    IEnumerable<IMap> Maps { get; }
}