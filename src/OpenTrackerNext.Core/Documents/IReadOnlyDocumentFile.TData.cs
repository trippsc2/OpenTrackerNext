using OpenTrackerNext.Core.Storage;

namespace OpenTrackerNext.Core.Documents;

/// <summary>
/// Represents a read-only document file.
/// </summary>
/// <typeparam name="TData">
/// The type of the document data.
/// </typeparam>
public interface IReadOnlyDocumentFile<out TData> : IReadOnlyDocumentFile
    where TData : ITitledDocumentData<TData>, new()
{
    /// <summary>
    /// A factory for creating new <see cref="IReadOnlyDocumentFile{TData}"/> objects.
    /// </summary>
    /// <param name="file">
    /// A <see cref="IReadOnlyStorageFile"/> representing the document file.
    /// </param>
    /// <returns>
    /// A new <see cref="IReadOnlyDocumentFile{TData}"/> instance.
    /// </returns>
    public delegate IReadOnlyDocumentFile<TData> Factory(IReadOnlyStorageFile file);
    
    /// <summary>
    /// Gets a <typeparamref name="TData"/> representing the data.
    /// </summary>
    TData Data { get; }
}