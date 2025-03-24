using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Entities;
using OpenTrackerNext.Editor.Documents;
using OpenTrackerNext.Editor.Pack;
using OpenTrackerNext.ServiceCollection;
using OpenTrackerNext.SplatRegistration;

namespace OpenTrackerNext.Editor.Entities;

/// <summary>
/// Represents the logic for managing entity data.
/// </summary>
[ServiceCollectionMember<PackFolderServices>]
[Splat(RegisterAsType = typeof(IPackFolderService<EntityPrototype, EntityId>))]
[SplatSingleInstance]
public sealed class EntityService : PackFolderService<EntityPrototype, EntityId>
{
    /// <summary>
    /// A <see cref="string"/> representing the entities folder name.
    /// </summary>
    public const string EntitiesFolderName = "Entities";
    
    /// <summary>
    /// A <see cref="string"/> representing the entity item name.
    /// </summary>
    public const string EntityItemName = "Entity";

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityService"/> class.
    /// </summary>
    /// <param name="documentService">
    /// The <see cref="IDocumentService"/>.
    /// </param>
    /// <param name="fileFactory">
    /// An Autofac factory for creating new <see cref="IDocumentFile"/> of <see cref="EntityPrototype"/> and
    /// <see cref="EntityId"/> objects.
    /// </param>
    public EntityService(IDocumentService documentService, IDocumentFile<EntityPrototype, EntityId>.Factory fileFactory)
        : base(documentService, fileFactory, EntitiesFolderName, EntityItemName)
    {
    }
}