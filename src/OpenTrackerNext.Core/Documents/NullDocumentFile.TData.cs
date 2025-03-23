using System;
using OpenTrackerNext.Core.Utils;

namespace OpenTrackerNext.Core.Documents;

/// <summary>
/// Represents a null JSON file of <see cref="TData"/>.
/// </summary>
/// <typeparam name="TData">
/// The type of the data.
/// </typeparam>
public class NullDocumentFile<TData>
    : NullDocumentFile, IDocumentFile<TData>, INullObject<NullDocumentFile<TData>>
    where TData : class, ITitledDocumentData<TData>, new()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NullDocumentFile{TData}"/> class.
    /// </summary>
    protected NullDocumentFile()
    {
    }

    /// <summary>
    /// Gets a singleton <see cref="NullDocumentFile{TData}"/> of <see cref="TData"/> instance.
    /// </summary>
    public static NullDocumentFile<TData> Instance { get; } = new();

    /// <inheritdoc />
    public TData SavedData => throw new NotSupportedException("Null file does not support this property.");
    
    /// <inheritdoc />
    public TData WorkingData => throw new NotSupportedException("Null file does not support this property.");
}