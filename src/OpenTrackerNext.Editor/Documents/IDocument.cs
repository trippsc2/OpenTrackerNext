using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace OpenTrackerNext.Editor.Documents;

/// <summary>
/// Represents a document.
/// </summary>
public interface IDocument : IDisposable, INotifyPropertyChanged
{
    /// <summary>
    /// Gets a value indicating whether the document has unsaved changes.
    /// </summary>
    bool IsUnsaved { get; }
    
    /// <summary>
    /// Gets or sets a <see cref="DocumentSide"/> representing the side of the document.
    /// </summary>
    DocumentSide Side { get; set; }
    
    /// <summary>
    /// Gets a <see cref="string"/> representing the base title of the document.
    /// </summary>
    string BaseTitle { get; }
    
    /// <summary>
    /// Gets a <see cref="string"/> representing the title of the document.
    /// </summary>
    string Title { get; }
    
    /// <summary>
    /// Reverts the document to its last saved state.
    /// </summary>
    void Revert();
    
    /// <summary>
    /// Saves the document asynchronously.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    Task SaveAsync();
}