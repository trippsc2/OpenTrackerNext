using System.IO;
using System.IO.Compression;

namespace OpenTrackerNext.Core.Storage.Zip;

/// <summary>
/// Represents a zip entry file.
/// </summary>
public sealed class ZipEntryFile : IReadOnlyStorageFile
{
    private readonly IStorageFile _zipStorageFile;

    /// <summary>
    /// Initializes a new instance of the <see cref="ZipEntryFile"/> class.
    /// </summary>
    /// <param name="zipStorageFile">
    /// A <see cref="IStorageFile"/> representing the zip file.
    /// </param>
    /// <param name="entryPath">
    /// A <see cref="string"/> representing the entry path.
    /// </param>
    public ZipEntryFile(IStorageFile zipStorageFile, string entryPath)
    {
        _zipStorageFile = zipStorageFile;
        FullPath = entryPath;
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

        return archive.GetEntry(FullPath) is not null;
    }

    /// <inheritdoc/>
    public Stream OpenRead()
    {
        if (!_zipStorageFile.Exists())
        {
            throw new FileNotFoundException();
        }
        
        var archive = GetArchive();
        var entry = archive.GetEntry(FullPath);

        return entry?.Open() ?? throw new FileNotFoundException();
    }

    private ZipArchive GetArchive()
    {
        var stream = CopyReadOnlyStreamToMemoryStream();

        return new ZipArchive(stream, ZipArchiveMode.Read);
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