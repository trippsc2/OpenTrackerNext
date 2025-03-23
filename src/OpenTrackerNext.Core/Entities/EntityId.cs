using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using OpenTrackerNext.Core.Ids;
using OpenTrackerNext.Core.Json;
using StronglyTypedIds;

namespace OpenTrackerNext.Core.Entities;

/// <summary>
/// Represents a GUID identifying an entity.
/// </summary>
[ExcludeFromCodeCoverage]
[JsonConverter(typeof(GuidIdConverter<EntityId>))]
[StronglyTypedId(StronglyTypedIdBackingType.Guid, StronglyTypedIdConverter.None)]
public partial struct EntityId : IGuidId<EntityId>
{
    /// <summary>
    /// A <see cref="string"/> representing the entities folder name.
    /// </summary>
    public const string EntitiesFolderName = "Entities";

    /// <inheritdoc />
    public static string FolderName => EntitiesFolderName;

    /// <inheritdoc />
    public static EntityId Parse(string value)
    {
        return new EntityId(Guid.Parse(value));
    }
}