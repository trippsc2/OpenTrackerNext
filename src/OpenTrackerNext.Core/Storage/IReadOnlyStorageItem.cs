namespace OpenTrackerNext.Core.Storage;

/// <summary>
/// Represents a storage file or folder with read only access.
/// </summary>
public interface IReadOnlyStorageItem
{
    /// <summary>
    /// Gets a <see cref="string"/> representing the storage item full path.
    /// </summary>
    string FullPath { get; }

    /// <summary>
    /// Gets a <see cref="string"/> representing the storage item name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Returns whether the storage item exists.
    /// </summary>
    /// <returns>
    /// A <see cref="bool"/> representing whether the storage item exists.
    /// </returns>
    bool Exists();
}