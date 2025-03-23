using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using OpenTrackerNext.Core.Ids;
using OpenTrackerNext.Core.Json;
using StronglyTypedIds;

namespace OpenTrackerNext.Core.Layouts;

/// <summary>
/// Represents a layout ID.
/// </summary>
[ExcludeFromCodeCoverage]
[JsonConverter(typeof(GuidIdConverter<LayoutId>))]
[StronglyTypedId(StronglyTypedIdBackingType.Guid, StronglyTypedIdConverter.None)]
public partial struct LayoutId : IGuidId<LayoutId>
{
    /// <summary>
    /// A <see cref="string"/> representing the layouts folder name.
    /// </summary>
    public const string LayoutsFolderName = "Layouts";

    /// <inheritdoc cref="IGuidId{TSelf}.FolderName" />
    public static string FolderName => LayoutsFolderName;

    /// <inheritdoc cref="IGuidId{TSelf}.Parse" />
    public static LayoutId Parse(string value)
    {
        return new LayoutId(Guid.Parse(value));
    }
}