using OpenTrackerNext.Core.Equality;

namespace OpenTrackerNext.Core.Documents;

/// <summary>
/// Represents the content data of a document.
/// </summary>
/// <typeparam name="TSelf">
/// The type of document data.
/// </typeparam>
public interface IDocumentData<TSelf> : IDocumentData, ICloneable<TSelf>, IMakeValueEqual<TSelf>, IValueEquatable<TSelf>
    where TSelf : IDocumentData<TSelf>, new();