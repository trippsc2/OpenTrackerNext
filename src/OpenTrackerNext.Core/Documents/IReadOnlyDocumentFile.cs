using System.Threading.Tasks;

namespace OpenTrackerNext.Core.Documents;

/// <summary>
/// Represents a read-only document file.
/// </summary>
public interface IReadOnlyDocumentFile
{
    /// <summary>
    /// Loads the data from the file asynchronously.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    Task LoadDataFromFileAsync();
}