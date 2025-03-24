using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.UIPanes;
using OpenTrackerNext.Editor.Documents;
using OpenTrackerNext.Editor.Pack;
using OpenTrackerNext.ServiceCollection;
using OpenTrackerNext.SplatRegistration;

namespace OpenTrackerNext.Editor.UIPanes;

/// <summary>
/// Represents the logic for managing <see cref="UIPanePrototype"/> objects.
/// </summary>
[ServiceCollectionMember<PackFolderServices>]
[Splat(RegisterAsType = typeof(IPackFolderService<UIPanePrototype, UIPaneId>))]
[SplatSingleInstance]
public sealed class UIPaneService : PackFolderService<UIPanePrototype, UIPaneId>
{
    /// <summary>
    /// A <see cref="string"/> representing the name of the folder.
    /// </summary>
    public const string UIPanesFolderName = "UIPanes";
    
    /// <summary>
    /// A <see cref="string"/> representing the name of the item.
    /// </summary>
    public const string UIPaneItemName = "UI Pane";

    /// <summary>
    /// Initializes a new instance of the <see cref="UIPaneService"/> class.
    /// </summary>
    /// <param name="documentService">
    /// The <see cref="IDocumentService"/>.
    /// </param>
    /// <param name="fileFactory">
    /// A factory method for creating new <see cref="IDocumentFile"/> objects with
    /// <see cref="UIPanePrototype"/> data and <see cref="UIPaneId"/> ID.
    /// </param>
    public UIPaneService(IDocumentService documentService, IDocumentFile<UIPanePrototype, UIPaneId>.Factory fileFactory)
        : base(documentService, fileFactory, UIPanesFolderName, UIPaneItemName)
    {
    }
}