using System;
using System.Threading.Tasks;
using ReactiveUI;

namespace OpenTrackerNext.Core.Documents;

/// <summary>
/// Represents a null JSON file.
/// </summary>
public abstract class NullDocumentFile : ReactiveObject, IDocumentFile
{
    /// <summary>
    /// A <see cref="string"/> representing the friendly name of null files.
    /// </summary>
    public const string FriendlyIdConstant = "<none>";
    
    /// <inheritdoc />
    public string TitlePrefix => throw new NotSupportedException("Null file does not support this property.");
    
    /// <inheritdoc />
    public string FriendlyId => FriendlyIdConstant;
    
    /// <inheritdoc />
    public int OpenedInDocuments
    {
        get => throw new NotSupportedException("Null file does not support this property.");
        set => throw new NotSupportedException("Null file does not support this property.");
    }
    
    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
    
    /// <inheritdoc />
    public Task LoadDataFromFileAsync()
    {
        throw new NotSupportedException("Null file does not support this method.");
    }

    /// <inheritdoc />
    public void Revert()
    {
        throw new NotSupportedException("Null file does not support this method.");
    }

    /// <inheritdoc />
    public Task SaveAsync()
    {
        throw new NotSupportedException("Null file does not support this method.");
    }
    
    /// <inheritdoc />
    public void Delete()
    {
        throw new NotSupportedException("Null file does not support this method.");
    }
}