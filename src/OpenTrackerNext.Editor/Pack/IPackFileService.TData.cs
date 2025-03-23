using System.Threading.Tasks;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Storage;

namespace OpenTrackerNext.Editor.Pack;

/// <summary>
/// Represents the service for presenting a single pack file.
/// </summary>
/// <typeparam name="TData">
/// The type of the pack file.
/// </typeparam>
public interface IPackFileService<out TData> : IPackComponentService
    where TData : class, ITitledDocumentData<TData>, new()
{
    /// <summary>
    /// Gets a <see cref="IDocumentFile{TData}"/> of <see cref="TData"/> representing the pack file.
    /// </summary>
    IDocumentFile<TData>? File { get; }

    /// <summary>
    /// Initializes the folder within the specified pack folder asynchronously.
    /// </summary>
    /// <param name="folder">
    /// A <see cref="IStorageFolder"/> representing the pack folder.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    Task NewPackAsync(IStorageFolder folder);
}