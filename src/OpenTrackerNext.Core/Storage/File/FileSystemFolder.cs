using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using OpenTrackerNext.SplatRegistration;

namespace OpenTrackerNext.Core.Storage.File;

/// <summary>
/// Represents a file system folder.
/// </summary>
[Splat(RegisterAsType = typeof(IStorageFolder.Factory))]
public sealed class FileSystemFolder : IStorageFolder
{
    private readonly DirectoryInfo _directory;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileSystemFolder"/> class.
    /// </summary>
    /// <param name="path">
    /// A <see cref="string"/> representing the directory path.
    /// </param>
    [SplatConstructor]
    public FileSystemFolder(string path)
        : this(new DirectoryInfo(path))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileSystemFolder"/> class.
    /// </summary>
    /// <param name="directory">
    /// A <see cref="DirectoryInfo"/> representing the directory.
    /// </param>
    private FileSystemFolder(DirectoryInfo directory)
    {
        _directory = directory;
        FullPath = _directory.FullName;
        Name = _directory.Name;
    }

    /// <inheritdoc/>
    public string FullPath { get; }

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public bool Exists()
    {
        return _directory.Exists;
    }

    /// <inheritdoc/>
    public void Delete()
    {
        if (Exists())
        {
            _directory.Delete(true);
        }
    }

    /// <inheritdoc/>
    public void Create()
    {
        _directory.Create();
    }

    /// <inheritdoc/>
    public IStorageFolder CreateFolder(string name)
    {
        var newFolder = _directory.CreateSubdirectory(name);

        return new FileSystemFolder(newFolder);
    }

    /// <inheritdoc/>
    public IStorageFile CreateFile(string name)
    {
        Create();

        var filePath = Path.Combine(FullPath, name);
        var newFile = new FileInfo(filePath);

        using var stream = newFile.Create();

        return new FileSystemFile(newFile);
    }

    /// <inheritdoc/>
    public IStorageFolder? GetFolder(string name)
    {
        var folderPath = Path.Combine(FullPath, name);
        var folder = new DirectoryInfo(folderPath);

        return folder.Exists ? new FileSystemFolder(folder) : null;
    }

    /// <inheritdoc/>
    public IStorageFile? GetFile(string name)
    {
        var filePath = Path.Combine(FullPath, name);
        var file = new FileInfo(filePath);

        return file.Exists ? new FileSystemFile(file) : null;
    }

    /// <inheritdoc/>
    public IEnumerable<IStorageItem> GetItems()
    {
        if (!Exists())
        {
            return [];
        }

        var items = _directory.GetFileSystemInfos();

        return items
            .Select<FileSystemInfo, IStorageItem>(
                x => x switch
                {
                    DirectoryInfo directory => new FileSystemFolder(directory),
                    FileInfo file => new FileSystemFile(file),
                    _ => throw new UnreachableException()
                });
    }

    /// <inheritdoc/>
    public IEnumerable<IStorageFolder> GetFolders()
    {
        if (!Exists())
        {
            return [];
        }

        var folders = _directory.GetDirectories();

        return folders.Select(x => new FileSystemFolder(x));
    }

    /// <inheritdoc/>
    public IEnumerable<IStorageFile> GetFiles()
    {
        if (!Exists())
        {
            return [];
        }

        var files = _directory.GetFiles();

        return files.Select(x => new FileSystemFile(x));
    }

    /// <inheritdoc/>
    IReadOnlyStorageFile? IReadOnlyStorageFolder.GetFile(string name)
    {
        return GetFile(name);
    }

    /// <inheritdoc/>
    IEnumerable<IReadOnlyStorageItem> IReadOnlyStorageFolder.GetItems()
    {
        return GetItems();
    }

    /// <inheritdoc/>
    IEnumerable<IReadOnlyStorageFolder> IReadOnlyStorageFolder.GetFolders()
    {
        return GetFolders();
    }

    /// <inheritdoc/>
    IEnumerable<IReadOnlyStorageFile> IReadOnlyStorageFolder.GetFiles()
    {
        return GetFiles();
    }

    /// <inheritdoc/>
    IReadOnlyStorageFolder? IReadOnlyStorageFolder.GetFolder(string name)
    {
        return GetFolder(name);
    }
}