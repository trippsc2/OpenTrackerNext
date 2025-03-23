using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace OpenTrackerNext.Core.Documents;

/// <summary>
/// Represents a document data file.
/// </summary>
public interface IDocumentFile : IDisposable, INotifyPropertyChanged
{
    /// <summary>
    /// Gets a <see cref="string"/> representing the title prefix of the document.
    /// </summary>
    string TitlePrefix { get; }
    
    /// <summary>
    /// Gets a <see cref="string"/> representing the friendly ID of the document file.
    /// </summary>
    string FriendlyId { get; }
    
    /// <summary>
    /// Gets or sets an <see cref="int"/> representing the number of times the document has been opened.
    /// </summary>
    int OpenedInDocuments { get; set; }

    /// <summary>
    /// Loads the data from the file asynchronously.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    Task LoadDataFromFileAsync();

    /// <summary>
    /// Reverts the document data to the last saved state.
    /// </summary>
    void Revert();

    /// <summary>
    /// Saves the working data to the file asynchronously.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    Task SaveAsync();

    /// <summary>
    /// Deletes the document file.
    /// </summary>
    void Delete();
}