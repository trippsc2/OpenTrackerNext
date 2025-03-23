using System;
using System.Reactive;
using OpenTrackerNext.Core.Equality;

namespace OpenTrackerNext.Core.Documents;

/// <summary>
/// Represents the content data of a document.
/// </summary>
public interface IDocumentData : ICloneable, IMakeValueEqual, IValueEquatable
{
    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> representing changes made to the document or any of its children.
    /// </summary>
    IObservable<Unit> DocumentChanges { get; }
}