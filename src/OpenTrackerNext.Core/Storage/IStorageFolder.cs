using System.Collections.Generic;

namespace OpenTrackerNext.Core.Storage;

/// <summary>
/// Represents a storage folder.
/// </summary>
public interface IStorageFolder : IReadOnlyStorageFolder, IStorageItem
{
    /// <summary>
    /// A factory method for creating new <see cref="IStorageFolder"/> objects.
    /// </summary>
    /// <param name="path">
    /// A <see cref="string"/> representing the folder path.
    /// </param>
    /// <returns>
    /// A new <see cref="IStorageFolder"/> object.
    /// </returns>
    public delegate IStorageFolder Factory(string path);

    /// <summary>
    /// Creates the folder, if it does not exist.
    /// </summary>
    void Create();

    /// <summary>
    /// Creates a new subfolder.
    /// </summary>
    /// <param name="name">
    /// A <see cref="string"/> representing the folder name.
    /// </param>
    /// <returns>
    /// A <see cref="IStorageFolder"/> representing the new subfolder.
    /// </returns>
    IStorageFolder CreateFolder(string name);

    /// <summary>
    /// Creates a new file.
    /// </summary>
    /// <param name="name">
    /// A <see cref="string"/> representing the file name.
    /// </param>
    /// <returns>
    /// A <see cref="IStorageFile"/> representing the new file.
    /// </returns>
    IStorageFile CreateFile(string name);

    /// <summary>
    /// Returns an existing subfolder.  Returns null if the subfolder does not exist.
    /// </summary>
    /// <param name="name">
    /// A <see cref="string"/> representing the subfolder name.
    /// </param>
    /// <returns>
    /// A <see cref="IStorageFolder"/> representing the existing subfolder.
    /// </returns>
    new IStorageFolder? GetFolder(string name);

    /// <summary>
    /// Returns an existing file.  Returns null if the file does not exist.
    /// </summary>
    /// <param name="name">
    /// A <see cref="string"/> representing the file name.
    /// </param>
    /// <returns>
    /// A <see cref="IStorageFile"/> representing the existing file.
    /// </returns>
    new IStorageFile? GetFile(string name);

    /// <summary>
    /// Returns a list of all storage items in the folder.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of <see cref="IStorageItem"/> representing the storage items.
    /// </returns>
    new IEnumerable<IStorageItem> GetItems();

    /// <summary>
    /// Returns a list of all subfolders in the folder.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of <see cref="IStorageFolder"/> representing the subfolders.
    /// </returns>
    new IEnumerable<IStorageFolder> GetFolders();

    /// <summary>
    /// Returns a list of all files in the folder.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of <see cref="IStorageFile"/> representing the files.
    /// </returns>
    new IEnumerable<IStorageFile> GetFiles();
}