using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Maps;
using OpenTrackerNext.Editor.Documents;
using OpenTrackerNext.Editor.Pack;
using OpenTrackerNext.ServiceCollection;
using OpenTrackerNext.SplatRegistration;

namespace OpenTrackerNext.Editor.Maps;

/// <summary>
/// Represents the logic for managing the map data.
/// </summary>
[ServiceCollectionMember<PackFolderServices>]
[Splat(RegisterAsType = typeof(IPackFolderService<MapPrototype, MapId>))]
[SplatSingleInstance]
public sealed class MapService : PackFolderService<MapPrototype, MapId>
{
    /// <summary>
    /// A <see cref="string"/> representing the maps folder name.
    /// </summary>
    public const string MapsFolderName = "Maps";
    
    /// <summary>
    /// A <see cref="string"/> representing the map item name.
    /// </summary>
    public const string MapItemName = "Map";

    /// <summary>
    /// Initializes a new instance of the <see cref="MapService"/> class.
    /// </summary>
    /// <param name="documentService">
    /// The <see cref="IDocumentService"/>.
    /// </param>
    /// <param name="fileFactory">
    /// An Autofac factory for creating new <see cref="IDocumentFile"/> of <see cref="MapPrototype"/> and
    /// <see cref="MapId"/> objects.
    /// </param>
    public MapService(IDocumentService documentService, IDocumentFile<MapPrototype, MapId>.Factory fileFactory)
        : base(documentService, fileFactory, MapsFolderName, MapItemName)
    {
    }
}