using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using OpenTrackerNext.Core.Ids;
using OpenTrackerNext.Core.Json;
using StronglyTypedIds;

namespace OpenTrackerNext.Core.Icons;

/// <summary>
/// Represents an icon ID.
/// </summary>
[ExcludeFromCodeCoverage]
[JsonConverter(typeof(GuidIdConverter<IconId>))]
[StronglyTypedId(StronglyTypedIdBackingType.Guid, StronglyTypedIdConverter.None)]
public partial struct IconId : IGuidId<IconId>
{
    /// <summary>
    /// A <see cref="string"/> representing the folder name storing the icon data.
    /// </summary>
    public const string IconsFolderName = "Icons";

    /// <inheritdoc cref="IGuidId{TSelf}.FolderName" />
    public static string FolderName => IconsFolderName;
    
    /// <inheritdoc cref="IGuidId{TSelf}.Parse" />
    public static IconId Parse(string value)
    {
        return new IconId(Guid.Parse(value));
    }
}