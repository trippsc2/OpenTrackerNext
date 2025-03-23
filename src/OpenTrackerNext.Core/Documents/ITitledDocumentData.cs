namespace OpenTrackerNext.Core.Documents;

/// <summary>
/// Represents the content data of a titled document.
/// </summary>
public interface ITitledDocumentData : IDocumentData
{
    /// <summary>
    /// Gets a <see cref="string"/> representing the title prefix of the document type.
    /// </summary>
    static abstract string TitlePrefix { get; }
}