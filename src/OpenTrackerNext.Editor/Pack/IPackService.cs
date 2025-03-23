using System.ComponentModel;
using System.Threading.Tasks;
using OpenTrackerNext.Core.Storage;

namespace OpenTrackerNext.Editor.Pack;

/// <summary>
/// Represents the logic for managing pack components.
/// </summary>
public interface IPackService : INotifyPropertyChanged
{
    /// <summary>
    /// Gets a <see cref="IStorageFolder"/> representing the loaded pack folder.
    /// </summary>
    IStorageFolder? LoadedPackFolder { get; }

    /// <summary>
    /// Creates a new pack at the specified folder.
    /// </summary>
    /// <param name="folder">
    /// A <see cref="IStorageFolder"/> represents the pack folder.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    Task NewPackAsync(IStorageFolder folder);

    /// <summary>
    /// Opens an existing pack at the specified folder.
    /// </summary>
    /// <param name="folder">
    /// A <see cref="IStorageFolder"/> represents the pack folder.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    Task OpenPackAsync(IStorageFolder folder);
    
    /// <summary>
    /// Closes the currently loaded pack.
    /// </summary>
    void ClosePack();
}