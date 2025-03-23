using OpenTrackerNext.Core.Ids;
using OpenTrackerNext.Core.Storage;

namespace OpenTrackerNext.Core.Documents;

/// <summary>
/// Represents a read-only JSON file.
/// </summary>
/// <typeparam name="TData">
/// The type of JSON data contained in the file.
/// </typeparam>
/// <typeparam name="TId">
/// The type of the ID.
/// </typeparam>
public interface IReadOnlyDocumentFile<out TData, out TId> : IReadOnlyDocumentFile<TData>
    where TData : class, ITitledDocumentData<TData>, new()
    where TId : IGuidId<TId>, new()
{
    /// <summary>
    /// A factory method that returns new <see cref="IReadOnlyDocumentFile{TData,TId}"/> objects.
    /// </summary>
    /// <param name="file">
    /// A <see cref="IReadOnlyStorageFile"/> representing the file.
    /// </param>
    /// <returns>
    /// A new <see cref="IReadOnlyDocumentFile{TData,TId}"/> object.
    /// </returns>
    public new delegate IReadOnlyDocumentFile<TData, TId> Factory(IReadOnlyStorageFile file);
    
    /// <summary>
    /// Gets a <see cref="TId"/> representing the ID of the document.
    /// </summary>
    TId Id { get; }
}