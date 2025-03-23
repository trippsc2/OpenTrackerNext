using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace OpenTrackerNext.Core.Storage.Zip;

/// <summary>
/// Represents a zip entry folder.
/// </summary>
public sealed class ZipEntryFolder : IReadOnlyStorageFolder
{
    private readonly IStorageFile _zipStorageFile;

    /// <summary>
    /// Initializes a new instance of the <see cref="ZipEntryFolder"/> class.
    /// </summary>
    /// <param name="zipStorageFile">
    /// A <see cref="FileInfo"/> representing the zip file.
    /// </param>
    /// <param name="entryPath">
    /// A <see cref="string"/> representing the entry path.
    /// </param>
    public ZipEntryFolder(IStorageFile zipStorageFile, string entryPath)
    {
        _zipStorageFile = zipStorageFile;

        if (string.IsNullOrWhiteSpace(entryPath))
        {
            FullPath = string.Empty;
            Name = string.Empty;
            return;
        }

        if (entryPath.EndsWith('/'))
        {
            FullPath = entryPath;
            entryPath = entryPath[..^1];
            Name = Path.GetFileName(entryPath);
        }

        FullPath = $"{entryPath}/";
        Name = Path.GetFileName(entryPath);
    }

    /// <inheritdoc/>
    public string FullPath { get; }
    
    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public bool Exists()
    {
        if (!_zipStorageFile.Exists())
        {
            return false;
        }

        using var archive = GetArchive();

        return FullPath == string.Empty || archive.GetEntry(FullPath) is not null;
    }

    /// <inheritdoc/>
    public IReadOnlyStorageFolder? GetFolder(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Folder name cannot be empty.", nameof(name));
        }

        if (!_zipStorageFile.Exists())
        {
            return null;
        }

        var path = $"{FullPath}{name}/";

        using var readArchive = GetArchive();

        if (!readArchive.Entries.Any(x => x.FullName.StartsWith(path)))
        {
            return null;
        }

        var folder = new ZipEntryFolder(_zipStorageFile, path);

        return folder;
    }

    /// <inheritdoc/>
    public IReadOnlyStorageFile? GetFile(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("File name cannot be empty.", nameof(name));
        }

        if (!_zipStorageFile.Exists())
        {
            return null;
        }

        var path = $"{FullPath}{name}";

        using var archive = GetArchive();

        return archive.GetEntry(path) is not null
            ? new ZipEntryFile(_zipStorageFile, path)
            : null;
    }

    /// <inheritdoc/>
    public IEnumerable<IReadOnlyStorageItem> GetItems()
    {
        if (!_zipStorageFile.Exists())
        {
            return Enumerable.Empty<IStorageItem>();
        }

        using var archive = GetArchive();

        return archive.Entries
            .Where(x => x.FullName.StartsWith(FullPath))
            .Select(x => x.FullName)
            .Where(x => !x[FullPath.Length..].TrimEnd('/').Contains('/'))
            .Select(
                x =>
                    x.EndsWith('/')
                        ? (IReadOnlyStorageItem)new ZipEntryFolder(_zipStorageFile, x)
                        : new ZipEntryFile(_zipStorageFile, x));
    }

    /// <inheritdoc/>
    public IEnumerable<IReadOnlyStorageFolder> GetFolders()
    {
        if (!_zipStorageFile.Exists())
        {
            return Enumerable.Empty<IStorageFolder>();
        }

        using var archive = GetArchive();

        return archive.Entries
            .Where(x => x.FullName.StartsWith(FullPath))
            .Select(x => x.FullName)
            .Where(x => x.EndsWith('/'))
            .Where(x => !x[FullPath.Length..^1].Contains('/'))
            .Select(x => new ZipEntryFolder(_zipStorageFile, x));
    }

    /// <inheritdoc/>
    public IEnumerable<IReadOnlyStorageFile> GetFiles()
    {
        if (!_zipStorageFile.Exists())
        {
            return Enumerable.Empty<IStorageFile>();
        }

        using var archive = GetArchive();

        return archive.Entries
            .Where(x => x.FullName.StartsWith(FullPath))
            .Select(x => x.FullName)
            .Where(x => !x[FullPath.Length..].Contains('/'))
            .Select(x => new ZipEntryFile(_zipStorageFile, x));
    }

    private ZipArchive GetArchive(ZipArchiveMode mode = ZipArchiveMode.Read)
    {
        var stream = mode switch
        {
            ZipArchiveMode.Read => CopyReadOnlyStreamToMemoryStream(),
            ZipArchiveMode.Create => _zipStorageFile.OpenModify(),
            ZipArchiveMode.Update => _zipStorageFile.OpenModify(),
            _ => throw new UnreachableException()
        };

        return new ZipArchive(stream, mode);
    }

    private MemoryStream CopyReadOnlyStreamToMemoryStream()
    {
        using var fileStream = _zipStorageFile.OpenRead();
        var memoryStream = new MemoryStream();

        fileStream.CopyTo(memoryStream);
        memoryStream.Seek(0, SeekOrigin.Begin);

        return memoryStream;
    }
}