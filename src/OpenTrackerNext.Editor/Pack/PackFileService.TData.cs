using System.Threading.Tasks;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Storage;
using OpenTrackerNext.Editor.Documents;

namespace OpenTrackerNext.Editor.Pack;

/// <inheritdoc />
public abstract class PackFileService<TData> : IPackFileService<TData>
    where TData : class, ITitledDocumentData<TData>, new()
{
    private readonly IDocumentService _documentService;
    private readonly IDocumentFile<TData>.Factory _fileFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="PackFileService{TData}"/> class.
    /// </summary>
    /// <param name="documentService">
    /// The <see cref="IDocumentService"/>.
    /// </param>
    /// <param name="fileFactory">
    /// An Autofac factory for creating new <see cref="IDocumentFile{TData}"/> of <see cref="TData"/> objects.
    /// </param>
    /// <param name="fileName">
    /// A <see cref="string"/> representing the file name.
    /// </param>
    protected PackFileService(IDocumentService documentService, IDocumentFile<TData>.Factory fileFactory, string fileName)
    {
        _documentService = documentService;
        _fileFactory = fileFactory;
        FileName = fileName;
    }

    /// <summary>
    /// Gets a <see cref="string"/> representing the file name.
    /// </summary>
    public string FileName { get; }

    /// <inheritdoc/>
    public IDocumentFile<TData>? File { get; private set; }
    
    /// <inheritdoc/>
    public async Task NewPackAsync(IStorageFolder folder)
    {
        File = _fileFactory(folder.CreateFile(FileName));

        await File.SaveAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task OpenPackAsync(IStorageFolder folder)
    {
        var file = folder.GetFile(FileName);

        if (file is null)
        {
            await NewPackAsync(folder).ConfigureAwait(false);
            return;
        }

        var jsonFile = _fileFactory(file);
        await jsonFile.LoadDataFromFileAsync();

        File = jsonFile;
    }

    /// <inheritdoc/>
    public void ClosePack()
    {
        if (File is null)
        {
            return;
        }

        _documentService.Close(File);
        File.Dispose();
        File = null;
    }
}