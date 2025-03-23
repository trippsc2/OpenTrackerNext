using OpenTrackerNext.Core.Storage;

namespace OpenTrackerNext.Core.Documents;

/// <summary>
/// Represents a document data file.
/// </summary>
/// <typeparam name="TData">
/// The type of the document data.
/// </typeparam>
public interface IDocumentFile<out TData> : IDocumentFile
    where TData : ITitledDocumentData<TData>, new()
{
    /// <summary>
    /// A factory for creating new <see cref="IDocumentFile{TData}"/> objects.
    /// </summary>
    /// <param name="file">
    /// The <see cref="IStorageFile"/> representing the document file.
    /// </param>
    /// <returns>
    /// A new <see cref="IDocumentFile{TData}"/> instance.
    /// </returns>
    public delegate IDocumentFile<TData> Factory(IStorageFile file);
    
    /// <summary>
    /// Gets a(n) <typeparamref name="TData"/> representing the saved file data.
    /// </summary>
    TData SavedData { get; }
    
    /// <summary>
    /// Gets a(n) <typeparamref name="TData"/> representing the working file data.
    /// </summary>
    TData WorkingData { get; }
}