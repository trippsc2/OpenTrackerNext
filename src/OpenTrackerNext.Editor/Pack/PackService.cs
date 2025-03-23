using System;
using System.Threading.Tasks;
using OpenTrackerNext.Core.Metadata;
using OpenTrackerNext.Core.Storage;
using OpenTrackerNext.Core.Utils;
using OpenTrackerNext.Editor.Documents;
using OpenTrackerNext.Editor.Images;
using OpenTrackerNext.Editor.Tools;
using OpenTrackerNext.SplatRegistration;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Editor.Pack;

/// <inheritdoc cref="IPackService"/>
[Splat(RegisterAsType = typeof(IPackService))]
[SplatSingleInstance]
public sealed class PackService : ReactiveObject, IPackService
{
    private readonly Lazy<IDocumentService> _documentService;
    private readonly Lazy<IToolService> _toolService;
    private readonly Lazy<IPackFileService<PackMetadata>> _metadataService;
    private readonly Lazy<IImageService> _imageService;
    private readonly Lazy<IServiceCollection<IPackFolderService>> _folderServices;

    /// <summary>
    /// Initializes a new instance of the <see cref="PackService"/> class.
    /// </summary>
    /// <param name="documentService">
    /// A <see cref="Lazy{T}"/> of <see cref="IDocumentService"/> representing the document service.
    /// </param>
    /// <param name="toolService">
    /// A <see cref="Lazy{T}"/> of <see cref="IToolService"/> representing the tool service.
    /// </param>
    /// <param name="metadataService">
    /// A <see cref="Lazy{T}"/> of <see cref="IPackFileService{TData}"/> representing the metadata service.
    /// </param>
    /// <param name="imageService">
    /// A <see cref="Lazy{T}"/> of <see cref="IImageService"/> representing the image service.
    /// </param>
    /// <param name="folderServices">
    /// A <see cref="Lazy{T}"/> of <see cref="IServiceCollection{T}"/> of <see cref="IPackFolderService"/>
    /// representing the pack folder services.
    /// </param>
    public PackService(
        Lazy<IDocumentService> documentService,
        Lazy<IToolService> toolService,
        Lazy<IPackFileService<PackMetadata>> metadataService,
        Lazy<IImageService> imageService,
        Lazy<IServiceCollection<IPackFolderService>> folderServices)
    {
        _documentService = documentService;
        _toolService = toolService;
        _metadataService = metadataService;
        _imageService = imageService;
        _folderServices = folderServices;
    }

    /// <inheritdoc/>
    [Reactive]
    public IStorageFolder? LoadedPackFolder { get; private set; }
    
    /// <inheritdoc/>
    public async Task NewPackAsync(IStorageFolder folder)
    {
        LoadedPackFolder = folder;

        await _metadataService.Value
            .NewPackAsync(LoadedPackFolder)
            .ConfigureAwait(false);
        await _imageService.Value
            .NewPackAsync(LoadedPackFolder)
            .ConfigureAwait(false);

        foreach (var folderService in _folderServices.Value.All)
        {
            folderService.NewPack(LoadedPackFolder);
        }
    }

    /// <inheritdoc/>
    public async Task OpenPackAsync(IStorageFolder folder)
    {
        LoadedPackFolder = folder;

        await _metadataService.Value
            .OpenPackAsync(LoadedPackFolder)
            .ConfigureAwait(false);
        await _imageService.Value
            .OpenPackAsync(LoadedPackFolder)
            .ConfigureAwait(false);

        foreach (var folderService in _folderServices.Value.All)
        {
            await folderService
                .OpenPackAsync(LoadedPackFolder)
                .ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public void ClosePack()
    {
        LoadedPackFolder = null;
        _documentService.Value.CloseAll();
        _toolService.Value.DeactivateAllTools();

        foreach (var folderService in _folderServices.Value.All)
        {
            folderService.ClosePack();
        }
        
        _imageService.Value.ClosePack();
        _metadataService.Value.ClosePack();
    }
}