using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using OpenTrackerNext.Core.Ids;
using OpenTrackerNext.Core.Json;
using StronglyTypedIds;

namespace OpenTrackerNext.Core.Maps;

/// <summary>
/// Represents a map ID.
/// </summary>
[ExcludeFromCodeCoverage]
[JsonConverter(typeof(GuidIdConverter<MapId>))]
[StronglyTypedId(StronglyTypedIdBackingType.Guid, StronglyTypedIdConverter.None)]
public partial struct MapId : IGuidId<MapId>
{
    /// <summary>
    /// A <see cref="string"/> representing the folder name related to this ID.
    /// </summary>
    public const string MapsFolderName = "Maps";

    /// <inheritdoc cref="IGuidId{TSelf}.FolderName" />
    public static string FolderName => MapsFolderName;
    
    /// <inheritdoc cref="IGuidId{TSelf}.Parse" />
    public static MapId Parse(string value)
    {
        return new MapId(Guid.Parse(value));
    }
}