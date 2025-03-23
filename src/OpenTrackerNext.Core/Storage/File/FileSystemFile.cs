using System.IO;
using OpenTrackerNext.SplatRegistration;

namespace OpenTrackerNext.Core.Storage.File;

/// <summary>
/// Represents a file system file.
/// </summary>
[Splat(RegisterAsType = typeof(IStorageFile.Factory))]
public sealed class FileSystemFile : IStorageFile
{
    private readonly FileInfo _file;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileSystemFile"/> class.
    /// </summary>
    /// <param name="path">
    /// A <see cref="string"/> representing the file path.
    /// </param>
    [SplatConstructor]
    public FileSystemFile(string path)
        : this(new FileInfo(path))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileSystemFile"/> class.
    /// </summary>
    /// <param name="file">
    /// A <see cref="FileInfo"/> representing the file.
    /// </param>
    public FileSystemFile(FileInfo file)
    {
        _file = file;
        FullPath = file.FullName;
        Name = file.Name;
    }

    /// <inheritdoc/>
    public string FullPath { get; }

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public bool Exists()
    {
        return _file.Exists;
    }

    /// <inheritdoc/>
    public void Delete()
    {
        _file.Delete();
    }

    /// <inheritdoc/>
    public Stream OpenRead()
    {
        return _file.OpenRead();
    }

    /// <inheritdoc/>
    public Stream OpenModify()
    {
        return _file.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
    }

    /// <inheritdoc/>
    public Stream OpenWrite()
    {
        return _file.Open(FileMode.Create, FileAccess.Write, FileShare.None);
    }
}