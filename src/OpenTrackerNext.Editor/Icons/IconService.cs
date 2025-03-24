using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Icons;
using OpenTrackerNext.Editor.Documents;
using OpenTrackerNext.Editor.Pack;
using OpenTrackerNext.ServiceCollection;
using OpenTrackerNext.SplatRegistration;

namespace OpenTrackerNext.Editor.Icons;

/// <summary>
/// Represents the logic for managing icon data.
/// </summary>
[ServiceCollectionMember<PackFolderServices>]
[Splat(RegisterAsType = typeof(IPackFolderService<IconPrototype, IconId>))]
[SplatSingleInstance]
public sealed class IconService : PackFolderService<IconPrototype, IconId>
{
    /// <summary>
    /// A <see cref="string"/> representing the icons folder name.
    /// </summary>
    public const string IconsFolderName = "Icons";
    
    /// <summary>
    /// A <see cref="string"/> representing the icon item name.
    /// </summary>
    public const string IconItemName = "Icon";

    /// <summary>
    /// Initializes a new instance of the <see cref="IconService"/> class.
    /// </summary>
    /// <param name="documentService">
    /// The <see cref="IDocumentService"/>.
    /// </param>
    /// <param name="fileFactory">
    /// An Autofac factory for creating new <see cref="IDocumentFile"/> of <see cref="IconPrototype"/> and
    /// <see cref="IconId"/> objects.
    /// </param>
    public IconService(IDocumentService documentService, IDocumentFile<IconPrototype, IconId>.Factory fileFactory)
        : base(documentService, fileFactory, IconsFolderName, IconItemName)
    {
    }
}