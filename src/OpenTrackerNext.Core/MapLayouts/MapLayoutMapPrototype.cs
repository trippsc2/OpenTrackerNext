using OpenTrackerNext.Core.Maps;
using OpenTrackerNext.Document;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.MapLayouts;

/// <summary>
/// Represents a map within a <see cref="MapLayoutPrototype"/>.
/// </summary>
[Document]
public sealed partial class MapLayoutMapPrototype : ReactiveObject
{
    /// <summary>
    /// Gets or sets an <see cref="MapId"/> representing the map.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public MapId Map { get; set; } = MapId.Empty;
}