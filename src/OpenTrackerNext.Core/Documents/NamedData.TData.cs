namespace OpenTrackerNext.Core.Documents;

/// <summary>
/// Represents named data.
/// </summary>
/// <typeparam name="TData">
/// The type of the data.
/// </typeparam>
public sealed class NamedData<TData>
    where TData : ITitledDocumentData<TData>, new()
{
    /// <summary>
    /// Gets a <see cref="string"/> representing the friendly name of the data.
    /// </summary>
    public required string Name { get; init; }
    
    /// <summary>
    /// Gets a <see cref="TData"/> representing the data.
    /// </summary>
    public required TData Data { get; init; }
}