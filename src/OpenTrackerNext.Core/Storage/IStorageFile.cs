using System.IO;

namespace OpenTrackerNext.Core.Storage;

/// <summary>
/// Represents a storage file.
/// </summary>
public interface IStorageFile : IReadOnlyStorageFile, IStorageItem
{
    /// <summary>
    /// A factory method for creating new <see cref="IStorageFile"/> objects.
    /// </summary>
    /// <param name="path">
    /// A <see cref="string"/> representing the file path.
    /// </param>
    /// <returns>
    /// A new <see cref="IStorageFile"/> object.
    /// </returns>
    public delegate IStorageFile Factory(string path);

    /// <summary>
    /// Opens the file in a writeable mode.
    /// If the file does not exist, it will be created.
    /// If the file exists, it will be overwritten.
    /// </summary>
    /// <returns>
    /// A <see cref="Stream"/> representing the file contents in writeable mode.
    /// </returns>
    Stream OpenWrite();

    /// <summary>
    /// Opens the file in a writeable mode.
    /// If the file does not exist, it will be created.
    /// If the file exists, it will be opened in a read/write context.
    /// </summary>
    /// <returns>
    /// A <see cref="Stream"/> representing the file contents in writeable mode.
    /// </returns>
    Stream OpenModify();
}