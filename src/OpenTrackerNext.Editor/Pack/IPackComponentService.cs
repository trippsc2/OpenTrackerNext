using System.Threading.Tasks;
using OpenTrackerNext.Core.Storage;

namespace OpenTrackerNext.Editor.Pack;

/// <summary>
/// Represents the service for pack component data.
/// </summary>
public interface IPackComponentService
{
    /// <summary>
    /// Loads the folder from the specified pack folder asynchronously.
    /// </summary>
    /// <param name="packFolder">
    /// A <see cref="IStorageFolder"/> representing the pack folder.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    Task OpenPackAsync(IStorageFolder packFolder);
    
    /// <summary>
    /// Unloads the existing pack folder.
    /// </summary>
    void ClosePack();
}