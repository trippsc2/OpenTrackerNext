using System.IO;

namespace OpenTrackerNext.Core.Storage.Memory;

/// <summary>
/// Represents a file stored in system RAM.
/// </summary>
/// <param name="fullPath">
/// A <see cref="string"/> representing the full path of the file.
/// </param>
public sealed class MemoryStorageFile(string fullPath) : IStorageFile
{
    private byte[]? _data;
    private MemoryStream? _previousWriteStream;

    /// <inheritdoc/>
    public string FullPath { get; } = fullPath;
    
    /// <inheritdoc/>
    public string Name { get; } = Path.GetFileName(fullPath);

    /// <inheritdoc/>
    public bool Exists()
    {
        return _data is not null;
    }

    /// <inheritdoc/>
    public void Delete()
    {
        _data = null;
        _previousWriteStream = null;
    }

    /// <inheritdoc/>
    public Stream OpenWrite()
    {
        _data = [];
        _previousWriteStream = new MemoryStream();
        
        return _previousWriteStream;
    }

    /// <inheritdoc/>
    public Stream OpenModify()
    {
        var stream = new MemoryStream();

        if (_previousWriteStream is not null)
        {
            stream.Write(_previousWriteStream.ToArray());
            stream.Seek(0, SeekOrigin.Begin);
        }

        _data = [];
        _previousWriteStream = stream;

        return stream;
    }

    /// <inheritdoc/>
    public Stream OpenRead()
    {
        if (_previousWriteStream is null)
        {
            return _data is null
                ? new MemoryStream()
                : new MemoryStream(_data);
        }
        
        _data = _previousWriteStream.ToArray();
        _previousWriteStream = null;

        return new MemoryStream(_data);
    }
}