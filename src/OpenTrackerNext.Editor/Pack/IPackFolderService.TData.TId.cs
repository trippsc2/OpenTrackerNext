using System;
using System.Threading.Tasks;
using DynamicData;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Ids;

namespace OpenTrackerNext.Editor.Pack;

/// <summary>
/// Represents a service that manages a folder of pack files.
/// </summary>
/// <typeparam name="TData">
/// The type of the pack files.
/// </typeparam>
/// <typeparam name="TId">
/// The type of the pack file IDs.
/// </typeparam>
public interface IPackFolderService<TData, TId> : IPackFolderService
    where TData : class, ITitledDocumentData<TData>, new()
    where TId : struct, IGuidId<TId>
{
    /// <summary>
    /// Connects to the folder and returns an observable change set of the files.
    /// </summary>
    /// <returns>
    /// An <see cref="IObservable{T}"/> of <see cref="IChangeSet{TObject,TKey}"/> of
    /// <see cref="IDocumentFile"/> of <see cref="TData"/> and <see cref="TId"/> indexed by <see cref="TId"/>
    /// representing change to the files in the folder.
    /// </returns>
    IObservable<IChangeSet<IDocumentFile<TData, TId>, TId>> Connect();
    
    /// <summary>
    /// Adds a new file to the folder with the specified friendly name asynchronously.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> of <see cref="IDocumentFile{TData,TId}"/> of <see cref="TData"/> and
    /// <see cref="TId"/> representing the new file asynchronously.
    /// </returns>
    Task<IDocumentFile<TData, TId>?> AddFileAsync();

    /// <summary>
    /// Renames the specified file to the specified friendly name asynchronously.
    /// </summary>
    /// <param name="file">
    /// A <see cref="IDocumentFile{TData,TId}"/> of <see cref="TData"/> and <see cref="TId"/> representing the file to
    /// be renamed.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    Task RenameFileAsync(IDocumentFile<TData, TId> file);

    /// <summary>
    /// Deletes the specified file from the folder asynchronously.
    /// </summary>
    /// <param name="file">
    /// A <see cref="IDocumentFile{TData,TId}"/> of <see cref="TData"/> and <see cref="TId"/> representing the file to be
    /// deleted.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    Task DeleteFileAsync(IDocumentFile<TData, TId> file);
    
    /// <summary>
    /// Gets the file with the specified ID.
    /// </summary>
    /// <param name="id">
    /// A(n) <see cref="TId"/> representing the file ID.
    /// </param>
    /// <returns>
    /// A <see cref="IDocumentFile{TData,TId}"/> of <see cref="TData"/> and <see cref="TId"/> representing the file.
    /// </returns>
    IDocumentFile<TData, TId> GetFile(TId id);
}