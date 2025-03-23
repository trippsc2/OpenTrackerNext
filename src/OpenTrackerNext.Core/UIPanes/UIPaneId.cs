using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using OpenTrackerNext.Core.Ids;
using OpenTrackerNext.Core.Json;
using StronglyTypedIds;

namespace OpenTrackerNext.Core.UIPanes;

/// <summary>
/// Represents a UI pane IO.
/// </summary>
[ExcludeFromCodeCoverage]
[JsonConverter(typeof(GuidIdConverter<UIPaneId>))]
[StronglyTypedId(StronglyTypedIdBackingType.Guid, StronglyTypedIdConverter.None)]
public partial struct UIPaneId : IGuidId<UIPaneId>
{
    /// <summary>
    /// A <see cref="string"/> representing the name of the folder containing the UI pane data.
    /// </summary>
    public const string UIPanesFolderName = "UIPanes";

    /// <inheritdoc cref="IGuidId{TSelf}.FolderName" />
    public static string FolderName => UIPanesFolderName;

    /// <inheritdoc cref="IGuidId{TSelf}.Parse" />
    public static UIPaneId Parse(string value)
    {
        return new UIPaneId(Guid.Parse(value));
    }
}