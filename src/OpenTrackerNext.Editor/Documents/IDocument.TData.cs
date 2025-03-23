using OpenTrackerNext.Core.Documents;

namespace OpenTrackerNext.Editor.Documents;

/// <summary>
/// Represents a document.
/// </summary>
/// <typeparam name="TData">
/// The type of the data contained within the document.
/// </typeparam>
// ReSharper disable once TypeParameterCanBeVariant
public interface IDocument<TData> : IDocument
    where TData : class, ITitledDocumentData<TData>, new()
{
    /// <summary>
    /// A factory for creating new <see cref="IDocument{TData}"/> objects.
    /// </summary>
    /// <param name="file">
    /// A <see cref="IDocumentFile{TData}"/> representing the file associated with the document.
    /// </param>
    /// <returns>
    /// A new <see cref="IDocument{TData}"/> object.
    /// </returns>
    public delegate IDocument<TData> Factory(IDocumentFile<TData> file);
    
    /// <summary>
    /// Gets a <see cref="IDocumentFile{TData}"/> representing the file associated with the document.
    /// </summary>
    IDocumentFile<TData> File { get; }
}