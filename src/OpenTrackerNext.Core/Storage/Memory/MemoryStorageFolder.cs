using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OpenTrackerNext.Core.Storage.Memory;

/// <summary>
/// Represents a folder stored in system RAM.
/// </summary>
/// <param name="fullPath">
/// A <see cref="string"/> representing the full path of the folder.
/// </param>
public sealed class MemoryStorageFolder(string fullPath) : IStorageFolder
{
    private readonly List<MemoryStorageFolder> _folders = [];
    private readonly List<MemoryStorageFile> _files = [];

    private bool _shouldExist;

    /// <inheritdoc/>
    public string FullPath { get; } = fullPath;

    /// <inheritdoc/>
    public string Name { get; } = Path.GetFileName(fullPath);

    /// <inheritdoc/>
    public bool Exists()
    {
        return _shouldExist;
    }

    /// <inheritdoc/>
    public void Delete()
    {
        _shouldExist = false;
        
        _folders.Clear();
        _files.Clear();
    }

    /// <inheritdoc/>
    public void Create()
    {
        _shouldExist = true;
    }

    /// <inheritdoc/>
    public IStorageFolder CreateFolder(string name)
    {
        Create();
        
        var existingFolder = GetFolder(name);
        
        if (existingFolder is not null)
        {
            return existingFolder;
        }
        
        var existingFile = GetFile(name);

        if (existingFile is not null)
        {
            throw new IOException("Simulated file already exists.");
        }
        
        var newFolder = new MemoryStorageFolder(Path.Join(FullPath, name));
        newFolder.Create();
        _folders.Add(newFolder);
        
        return newFolder;
    }

    /// <inheritdoc/>
    public IStorageFile CreateFile(string name)
    {
        Create();

        var existingFile = GetFile(name);
        
        if (existingFile is not null)
        {
            var existingStream = existingFile.OpenWrite();
            existingStream.Dispose();
            
            return existingFile;
        }
        
        var existingFolder = GetFolder(name);
        
        if (existingFolder is not null)
        {
            throw new UnauthorizedAccessException("Simulated folder already exists.");
        }
        
        var newFile = new MemoryStorageFile(Path.Join(FullPath, name));

        var newStream = newFile.OpenWrite();
        newStream.Dispose();
        
        _files.Add(newFile);
        
        return newFile;
    }

    /// <inheritdoc/>
    public IStorageFolder? GetFolder(string name)
    {
        if (!_shouldExist)
        {
            return null;
        }
        
        var existingFolder = _folders.FirstOrDefault(x => x.Name == name);

        if (existingFolder is null || existingFolder.Exists())
        {
            return existingFolder;
        }
        
        _folders.Remove(existingFolder);
        return null;
    }

    /// <inheritdoc/>
    public IStorageFile? GetFile(string name)
    {
        if (!_shouldExist)
        {
            return null;
        }
        
        var existingFile = _files.FirstOrDefault(x => x.Name == name);
        
        if (existingFile is null || existingFile.Exists())
        {
            return existingFile;
        }
        
        _files.Remove(existingFile);
        return null;
    }

    /// <inheritdoc/>
    public IEnumerable<IStorageItem> GetItems()
    {
        return _shouldExist
            ? _folders.Cast<IStorageItem>().Concat(_files)
            : Array.Empty<IStorageItem>();
    }

    /// <inheritdoc/>
    public IEnumerable<IStorageFolder> GetFolders()
    {
        return _shouldExist
            ? _folders
            : Array.Empty<IStorageFolder>();
    }

    /// <inheritdoc/>
    public IEnumerable<IStorageFile> GetFiles()
    {
        return _shouldExist
            ? _files
            : Array.Empty<IStorageFile>();
    }

    /// <inheritdoc/>
    IReadOnlyStorageFolder? IReadOnlyStorageFolder.GetFolder(string name)
    {
        return GetFolder(name);
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
}