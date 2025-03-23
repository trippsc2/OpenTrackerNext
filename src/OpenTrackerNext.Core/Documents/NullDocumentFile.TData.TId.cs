using System;
using System.Threading.Tasks;
using OpenTrackerNext.Core.Ids;
using OpenTrackerNext.Core.Utils;

namespace OpenTrackerNext.Core.Documents;

/// <summary>
/// Represents a null JSON file of <see cref="TData"/> with a <see cref="TId"/> ID.
/// </summary>
/// <typeparam name="TData">
/// The type of the data.
/// </typeparam>
/// <typeparam name="TId">
/// The type of the ID.
/// </typeparam>
public sealed class NullDocumentFile<TData, TId>
    : NullDocumentFile<TData>, IDocumentFile<TData, TId>, INullObject<NullDocumentFile<TData, TId>>
    where TData : class, ITitledDocumentData<TData>, new()
    where TId : struct, IGuidId<TId>
{
    private NullDocumentFile()
    {
    }

    /// <summary>
    /// Gets a <see cref="NullDocumentFile{TData,TId}"/> of <see cref="TData"/> with a <see cref="TId"/> ID
    /// representing the null file instance.
    /// </summary>
    public new static NullDocumentFile<TData, TId> Instance { get; } = new();
    
    /// <inheritdoc/>
    public TId Id => default;
    
    /// <inheritdoc />
    public Task RenameAsync(string newName)
    {
        throw new NotSupportedException("Null file does not support this method.");
    }
}