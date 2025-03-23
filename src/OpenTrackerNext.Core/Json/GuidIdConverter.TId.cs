using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using OpenTrackerNext.Core.Ids;

namespace OpenTrackerNext.Core.Json;

/// <summary>
/// JSON converter for <see cref="IGuidId{TSelf}"/>.
/// </summary>
/// <typeparam name="TId">
/// The type of the ID.
/// </typeparam>
public sealed class GuidIdConverter<TId> : JsonConverter<TId>
    where TId : IGuidId<TId>, new()
{
    /// <inheritdoc/>
    public override TId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return TId.Parse(reader.GetString()!);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, TId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}