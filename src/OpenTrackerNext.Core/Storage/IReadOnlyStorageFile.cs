using System.IO;

namespace OpenTrackerNext.Core.Storage;

/// <summary>
/// Represents a storage file with read only access.
/// </summary>
public interface IReadOnlyStorageFile : IReadOnlyStorageItem
{
    /// <summary>
    /// Opens the file in read only mode.
    /// </summary>
    /// <returns>
    /// A <see cref="Stream"/> representing the file contents.
    /// </returns>
    Stream OpenRead();
}