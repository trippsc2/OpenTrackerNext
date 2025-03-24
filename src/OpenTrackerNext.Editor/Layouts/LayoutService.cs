using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Layouts;
using OpenTrackerNext.Editor.Documents;
using OpenTrackerNext.Editor.Pack;
using OpenTrackerNext.ServiceCollection;
using OpenTrackerNext.SplatRegistration;

namespace OpenTrackerNext.Editor.Layouts;

/// <summary>
/// Represents the logic for managing <see cref="LayoutPrototype"/> objects.
/// </summary>
[ServiceCollectionMember<PackFolderServices>]
[Splat(RegisterAsType = typeof(IPackFolderService<LayoutPrototype, LayoutId>))]
[SplatSingleInstance]
public sealed class LayoutService : PackFolderService<LayoutPrototype, LayoutId>
{
    /// <summary>
    /// A <see cref="string"/> representing the name of the folder.
    /// </summary>
    public const string LayoutsFolderName = "Layouts";
    
    /// <summary>
    /// A <see cref="string"/> representing the name of the item.
    /// </summary>
    public const string LayoutItemName = "Layout";

    /// <summary>
    /// Initializes a new instance of the <see cref="LayoutService"/> class.
    /// </summary>
    /// <param name="documentService">
    /// The <see cref="IDocumentService"/>.
    /// </param>
    /// <param name="fileFactory">
    /// A factory method for creating new <see cref="IDocumentFile"/> objects with
    /// <see cref="LayoutPrototype"/> data and <see cref="LayoutId"/> ID.
    /// </param>
    public LayoutService(IDocumentService documentService, IDocumentFile<LayoutPrototype, LayoutId>.Factory fileFactory)
        : base(documentService, fileFactory, LayoutsFolderName, LayoutItemName)
    {
    }
}