using System.Collections.Generic;

namespace OpenTrackerNext.Core.Storage;

/// <summary>
/// Represents a storage folder with read only access.
/// </summary>
public interface IReadOnlyStorageFolder : IReadOnlyStorageItem
{
    /// <summary>
    /// Returns an existing subfolder.  Returns null if the subfolder does not exist.
    /// </summary>
    /// <param name="name">
    /// A <see cref="string"/> representing the subfolder name.
    /// </param>
    /// <returns>
    /// A <see cref="IStorageFolder"/> representing the existing subfolder.
    /// </returns>
    IReadOnlyStorageFolder? GetFolder(string name);

    /// <summary>
    /// Returns an existing file.  Returns null if the file does not exist.
    /// </summary>
    /// <param name="name">
    /// A <see cref="string"/> representing the file name.
    /// </param>
    /// <returns>
    /// A <see cref="IStorageFile"/> representing the existing file.
    /// </returns>
    IReadOnlyStorageFile? GetFile(string name);

    /// <summary>
    /// Returns a list of all storage items in the folder.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of <see cref="IStorageItem"/> representing the storage items.
    /// </returns>
    IEnumerable<IReadOnlyStorageItem> GetItems();

    /// <summary>
    /// Returns a list of all subfolders in the folder.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of <see cref="IStorageFolder"/> representing the subfolders.
    /// </returns>
    IEnumerable<IReadOnlyStorageFolder> GetFolders();

    /// <summary>
    /// Returns a list of all files in the folder.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of <see cref="IStorageFile"/> representing the files.
    /// </returns>
    IEnumerable<IReadOnlyStorageFile> GetFiles();
}