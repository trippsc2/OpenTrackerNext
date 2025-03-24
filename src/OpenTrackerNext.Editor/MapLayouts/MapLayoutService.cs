using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.MapLayouts;
using OpenTrackerNext.Editor.Documents;
using OpenTrackerNext.Editor.Pack;
using OpenTrackerNext.ServiceCollection;
using OpenTrackerNext.SplatRegistration;

namespace OpenTrackerNext.Editor.MapLayouts;

/// <summary>
/// Represents the logic for managing <see cref="MapLayoutPrototype"/> objects.
/// </summary>
[ServiceCollectionMember<PackFolderServices>]
[Splat(RegisterAsType = typeof(IPackFolderService<MapLayoutPrototype, MapLayoutId>))]
[SplatSingleInstance]
public sealed class MapLayoutService : PackFolderService<MapLayoutPrototype, MapLayoutId>
{
    /// <summary>
    /// A <see cref="string"/> representing the name of the folder.
    /// </summary>
    public const string MapLayoutsFolderName = "MapLayouts";
    
    /// <summary>
    /// A <see cref="string"/> representing the name of the item.
    /// </summary>
    public const string MapLayoutItemName = "Map Layout";

    /// <summary>
    /// Initializes a new instance of the <see cref="MapLayoutService"/> class.
    /// </summary>
    /// <param name="documentService">
    /// The <see cref="IDocumentService"/>.
    /// </param>
    /// <param name="fileFactory">
    /// A factory method for creating new <see cref="IDocumentFile"/> objects with
    /// <see cref="MapLayoutPrototype"/> data and <see cref="MapLayoutId"/> ID.
    /// </param>
    public MapLayoutService(
        IDocumentService documentService,
        IDocumentFile<MapLayoutPrototype, MapLayoutId>.Factory fileFactory)
        : base(documentService, fileFactory, MapLayoutsFolderName, MapLayoutItemName)
    {
    }
}