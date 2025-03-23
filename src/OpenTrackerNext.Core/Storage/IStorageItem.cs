namespace OpenTrackerNext.Core.Storage;

/// <summary>
/// Represents a storage file or folder.
/// </summary>
public interface IStorageItem : IReadOnlyStorageItem
{
    /// <summary>
    /// Deletes the storage item.
    /// </summary>
    void Delete();
}