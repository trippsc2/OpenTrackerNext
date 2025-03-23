using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using OpenTrackerNext.Core.Ids;
using OpenTrackerNext.Core.Json;
using StronglyTypedIds;

namespace OpenTrackerNext.Core.Images;

/// <summary>
/// Represents an image ID. 
/// </summary>
[ExcludeFromCodeCoverage]
[JsonConverter(typeof(GuidIdConverter<ImageId>))]
[StronglyTypedId(StronglyTypedIdBackingType.Guid, StronglyTypedIdConverter.None)]
public partial struct ImageId : IGuidId<ImageId>
{
    /// <summary>
    /// A <see cref="string"/> representing the folder name related to this ID.
    /// </summary>
    public const string ImagesFolderName = "Images";
    
    /// <inheritdoc cref="IGuidId{TSelf}.FolderName" />
    public static string FolderName => ImagesFolderName;

    /// <inheritdoc cref="IGuidId{TSelf}.Parse" />
    public static ImageId Parse(string value)
    {
        return new ImageId(Guid.Parse(value));
    }
}