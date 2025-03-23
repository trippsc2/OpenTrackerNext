namespace OpenTrackerNext.Core.Documents;

/// <summary>
/// Represents the content data of a titled document.
/// </summary>
/// <typeparam name="TSelf">
/// The type of document data.
/// </typeparam>
public interface ITitledDocumentData<TSelf> : IDocumentData<TSelf>, ITitledDocumentData
    where TSelf : ITitledDocumentData<TSelf>, new();