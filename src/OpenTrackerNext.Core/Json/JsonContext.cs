using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Entities;
using OpenTrackerNext.Core.Icons;
using OpenTrackerNext.Core.Layouts;
using OpenTrackerNext.Core.MapLayouts;
using OpenTrackerNext.Core.Maps;
using OpenTrackerNext.Core.Metadata;
using OpenTrackerNext.Core.UIPanes;

namespace OpenTrackerNext.Core.Json;

/// <summary>
/// The source generated <see cref="JsonSerializerOptions"/> used for serialization and deserialization.
/// </summary>
[JsonSourceGenerationOptions(IncludeFields = true, IgnoreReadOnlyProperties = true, WriteIndented = true)]
[JsonSerializable(typeof(Dictionary<Guid, string>))]
[JsonSerializable(typeof(Entity))]
[JsonSerializable(typeof(MapLayoutId))]
[JsonSerializable(typeof(NamedData<EntityPrototype>))]
[JsonSerializable(typeof(NamedData<IconPrototype>))]
[JsonSerializable(typeof(NamedData<LayoutPrototype>))]
[JsonSerializable(typeof(NamedData<MapLayoutPrototype>))]
[JsonSerializable(typeof(NamedData<MapPrototype>))]
[JsonSerializable(typeof(NamedData<UIPanePrototype>))]
[JsonSerializable(typeof(PackMetadata))]
[JsonSerializable(typeof(UIPaneId))]
public sealed partial class JsonContext : JsonSerializerContext
{
    /// <summary>
    /// Deserializes the specified JSON string to an instance of the specified type.
    /// </summary>
    /// <param name="json">
    /// A <see cref="string"/> representing the JSON string to deserialize.
    /// </param>
    /// <typeparam name="TOutput">
    /// The type of the instance to deserialize to.
    /// </typeparam>
    /// <returns>
    /// A new <see cref="TOutput"/> instance deserialized from the specified JSON string.
    /// </returns>
    public static TOutput Deserialize<TOutput>(string json)
    {
        return (TOutput)JsonSerializer.Deserialize(json, typeof(TOutput), Default)!;
    }
    
    /// <summary>
    /// Deserializes the specified JSON stream to an instance of the specified type asynchronously.
    /// </summary>
    /// <param name="readStream">
    /// A <see cref="Stream"/> representing the JSON stream to deserialize.
    /// </param>
    /// <typeparam name="TOutput">
    /// The type of the instance to deserialize to.
    /// </typeparam>
    /// <returns>
    /// A <see cref="Task"/> returning a new <see cref="TOutput"/> instance deserialized from the specified JSON stream.
    /// </returns>
    public static async Task<TOutput> DeserializeAsync<TOutput>(Stream readStream)
    {
        var result = await JsonSerializer
            .DeserializeAsync(readStream, typeof(TOutput), Default)
            .ConfigureAwait(false);

        return (TOutput)result!;
    }
    
    /// <summary>
    /// Serializes the specified value to a JSON string.
    /// </summary>
    /// <param name="value">
    /// A <see cref="TInput"/> representing the value to serialize.
    /// </param>
    /// <typeparam name="TInput">
    /// The type of the value to serialize.
    /// </typeparam>
    /// <returns>
    /// A <see cref="string"/> representing the JSON string serialized from the specified value.
    /// </returns>
    public static string Serialize<TInput>(TInput value)
    {
        return JsonSerializer.Serialize(value, typeof(TInput), Default);
    }
    
    /// <summary>
    /// Serializes the specified value to a stream asynchronously.
    /// </summary>
    /// <param name="writeStream">
    /// A <see cref="Stream"/> representing the stream to which the serialized data will be written.
    /// </param>
    /// <param name="value">
    /// A <see cref="TInput"/> representing the value to serialize.
    /// </param>
    /// <typeparam name="TInput">
    /// The type of the value to serialize.
    /// </typeparam>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    public static Task SerializeAsync<TInput>(Stream writeStream, TInput value)
    {
        return JsonSerializer.SerializeAsync(writeStream, value, typeof(TInput), Default);
    }
}