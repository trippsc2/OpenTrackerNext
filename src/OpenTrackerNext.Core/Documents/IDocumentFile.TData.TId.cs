using System.Threading.Tasks;
using OpenTrackerNext.Core.Ids;
using OpenTrackerNext.Core.Storage;

namespace OpenTrackerNext.Core.Documents;

/// <summary>
/// Represents a document data file with GUID-based ID.
/// </summary>
/// <typeparam name="TData">
/// The type of the document data.
/// </typeparam>
/// <typeparam name="TId">
/// The type of the document ID.
/// </typeparam>
public interface IDocumentFile<out TData, out TId> : IDocumentFile<TData>
    where TData : ITitledDocumentData<TData>, new()
    where TId : IGuidId<TId>, new()
{
    /// <summary>
    /// A factory for creating new <see cref="IDocumentFile{TData,TId}"/> objects.
    /// </summary>
    /// <param name="file">
    /// A <see cref="IStorageFile"/> representing the document file.
    /// </param>
    /// <returns>
    /// A new <see cref="IDocumentFile{TData,TId}"/> object.
    /// </returns>
    public new delegate IDocumentFile<TData, TId> Factory(IStorageFile file);
    
    /// <summary>
    /// Gets a(n) <see cref="TId"/> representing the document ID.
    /// </summary>
    TId Id { get; }

    /// <summary>
    /// Renames the document and saves the new name asynchronously.
    /// </summary>
    /// <param name="newName">
    /// A <see cref="string"/> representing the new name of the document.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    Task RenameAsync(string newName);
}