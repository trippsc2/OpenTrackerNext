using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using OpenTrackerNext.Core.Ids;
using OpenTrackerNext.Core.Json;
using StronglyTypedIds;

namespace OpenTrackerNext.Core.MapLayouts;

/// <summary>
/// Represents a map layout ID.
/// </summary>
[ExcludeFromCodeCoverage]
[JsonConverter(typeof(GuidIdConverter<MapLayoutId>))]
[StronglyTypedId(StronglyTypedIdBackingType.Guid, StronglyTypedIdConverter.None)]
public partial struct MapLayoutId : IGuidId<MapLayoutId>
{
    /// <summary>
    /// A <see cref="string"/> representing the folder name related to this ID.
    /// </summary>
    public const string MapLayoutsFolderName = "MapLayouts";

    /// <inheritdoc cref="IGuidId{TSelf}.FolderName" />
    public static string FolderName => MapLayoutsFolderName;
    
    /// <inheritdoc cref="IGuidId{TSelf}.Parse" />
    public static MapLayoutId Parse(string value)
    {
        return new MapLayoutId(Guid.Parse(value));
    }
}