using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using OpenTrackerNext.Core.Metadata;
using OpenTrackerNext.Core.Storage.Memory;
using OpenTrackerNext.Core.Utils;
using OpenTrackerNext.Editor.Documents;
using OpenTrackerNext.Editor.Images;
using OpenTrackerNext.Editor.Pack;
using OpenTrackerNext.Editor.Tools;
using Splat;
using Xunit;

namespace OpenTrackerNext.Editor.UnitTests.Pack;

[ExcludeFromCodeCoverage]
public sealed class PackServiceTests
{
    private readonly IDocumentService _documentService = Substitute.For<IDocumentService>();
    private readonly IToolService _toolService = Substitute.For<IToolService>();
    private readonly IPackFileService<PackMetadata> _metadataService = Substitute.For<IPackFileService<PackMetadata>>();
    private readonly IImageService _imageService = Substitute.For<IImageService>();
    private readonly IServiceCollection<IPackFolderService> _folderServices = 
        Substitute.For<IServiceCollection<IPackFolderService>>();

    private readonly List<IPackFolderService> _allFolderServices =
    [
        Substitute.For<IPackFolderService>(),
        Substitute.For<IPackFolderService>(),
        Substitute.For<IPackFolderService>()
    ];
    
    private readonly PackService _subject;

    public PackServiceTests()
    {
        _folderServices.All.Returns(_allFolderServices);
        
        var lazyDocumentService = new Lazy<IDocumentService>(() => _documentService);
        var lazyToolService = new Lazy<IToolService>(() => _toolService);
        var lazyMetadataService = new Lazy<IPackFileService<PackMetadata>>(() => _metadataService);
        var lazyImageService = new Lazy<IImageService>(() => _imageService);
        var lazyFolderServices = new Lazy<IServiceCollection<IPackFolderService>>(() => _folderServices);
        
        _subject = new PackService(
            lazyDocumentService,
            lazyToolService,
            lazyMetadataService,
            lazyImageService,
            lazyFolderServices);
    }

    [Fact]
    public void LoadedPackFolder_ShouldReturnExpected_WhenNoPackIsLoaded()
    {
        _subject.LoadedPackFolder
            .Should()
            .BeNull();
    }

    [Fact]
    public async Task NewPackAsync_ShouldSetLoadedPackFolder()
    {
        var packFolder = new MemoryStorageFolder(Path.GetTempPath());
        
        await _subject.NewPackAsync(packFolder);
        
        _subject.LoadedPackFolder
            .Should()
            .Be(packFolder);
    }

    [Fact]
    public async Task NewPackAsync_ShouldCallNewPackOnAllServices()
    {
        var packFolder = new MemoryStorageFolder(Path.GetTempPath());

        await _subject.NewPackAsync(packFolder);

        await _metadataService
            .Received(1)
            .NewPackAsync(Arg.Is(packFolder));
        
        await _imageService
            .Received(1)
            .NewPackAsync(Arg.Is(packFolder));

        foreach (var folderService in _allFolderServices)
        {
            folderService
                .Received(1)
                .NewPack(Arg.Is(packFolder));
        }
    }

    [Fact]
    public async Task OpenPackAsync_ShouldSetLoadedPackFolder()
    {
        var packFolder = new MemoryStorageFolder(Path.GetTempPath());
        
        await _subject.OpenPackAsync(packFolder);
        
        _subject.LoadedPackFolder
            .Should()
            .Be(packFolder);
    }

    [Fact]
    public async Task OpenPackAsync_ShouldCallNewPackOnAllServices()
    {
        var packFolder = new MemoryStorageFolder(Path.GetTempPath());

        await _subject.OpenPackAsync(packFolder);

        await _metadataService
            .Received(1)
            .OpenPackAsync(Arg.Is(packFolder));
        
        await _imageService
            .Received(1)
            .OpenPackAsync(Arg.Is(packFolder));

        foreach (var folderService in _allFolderServices)
        {
            await folderService
                .Received(1)
                .OpenPackAsync(Arg.Is(packFolder));
        }
    }

    [Fact]
    public async Task ClosePack_ShouldSetLoadedPackFolderToNull()
    {
        var packFolder = new MemoryStorageFolder(Path.GetTempPath());
        
        await _subject.NewPackAsync(packFolder);
        
        _subject.LoadedPackFolder
            .Should()
            .Be(packFolder);
        
        _subject.ClosePack();
        
        _subject.LoadedPackFolder
            .Should()
            .BeNull();
    }

    [Fact]
    public void ClosePack_ShouldCloseAllServices()
    {
        _subject.ClosePack();
        
        _documentService
            .Received()
            .CloseAll();
        
        _toolService
            .Received()
            .DeactivateAllTools();

        foreach (var folderService in _allFolderServices)
        {
            folderService
                .Received()
                .ClosePack();
        }
        
        _imageService
            .Received()
            .ClosePack();
        
        _metadataService
            .Received()
            .ClosePack();
    }
    
    [Fact]
    public void SplatResolve_ShouldResolveToSingleInstance()
    {
        var subject1 = Locator.Current.GetService<IPackService>()!;
        var subject2 = Locator.Current.GetService<IPackService>()!;

        subject1.Should().BeOfType<PackService>();
        subject2.Should().BeOfType<PackService>();
        
        subject1.Should().BeSameAs(subject2);
    }
    
    [Fact]
    public void SplatResolve_ShouldResolveToLazySingleInstance()
    {
        var lazy = Locator.Current.GetService<Lazy<IPackService>>()!;
        var subject = Locator.Current.GetService<IPackService>()!;
        
        lazy.Value
            .Should()
            .BeOfType<PackService>();
        
        subject.Should().BeOfType<PackService>();

        lazy.Value
            .Should()
            .BeSameAs(subject);
    }
}