using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using DynamicData;
using FluentAssertions;
using Newtonsoft.Json;
using NSubstitute;
using OpenTrackerNext.Core.Ids;
using OpenTrackerNext.Core.Images;
using OpenTrackerNext.Core.Results;
using OpenTrackerNext.Core.Storage;
using OpenTrackerNext.Core.Storage.Memory;
using OpenTrackerNext.Editor.Images;
using Splat;
using Xunit;
using IAvaloniaStorageFile = Avalonia.Platform.Storage.IStorageFile;

namespace OpenTrackerNext.Editor.UnitTests.Images;

[ExcludeFromCodeCoverage]
public sealed class ImageServiceTests
{
    private readonly CompositeDisposable _disposables = new();
    private readonly MemoryStorageFolder _packFolder;
    
    private readonly ImageService _subject;

    private readonly ReadOnlyObservableCollection<IImageFile> _derivedList;
    
    private static ImageFile ImageFileFactory(IStorageFile file)
    {
        return new ImageFile(file);
    }

    public ImageServiceTests()
    {
        var packFolderName = Guid.NewGuid().ToLowercaseString();
        var packFolderPath = Path.Join(Path.GetTempPath(), packFolderName);
        
        _packFolder = new MemoryStorageFolder(packFolderPath);
        
        _subject = new ImageService(ImageFileFactory);

        _subject.Connect()
            .Bind(out _derivedList)
            .Subscribe()
            .DisposeWith(_disposables);
    }

    private static IStorageFile CreateImageFile(IStorageFolder parentFolder)
    {
        var fileName = Guid.NewGuid().ToLowercaseString();

        return parentFolder.CreateFile(fileName);
    }

    private async Task<IImageFile> AddFileAsync(string friendlyId)
    {
        var file = Substitute.For<IAvaloniaStorageFile>();
        
        file.Name.Returns(friendlyId);
        file.OpenReadAsync().Returns(new MemoryStream());

        _subject.AddImagesDialog
            .RegisterHandler(context => context.SetOutput(new List<IAvaloniaStorageFile> { file }))
            .DisposeWith(_disposables);
        
        var results = await _subject.AddFilesAsync();

        return results[0];
    }

    [Fact]
    public async Task NameValidationRules_ShouldReturnExpectedRules()
    {
        var rules = _subject.NameValidationRules;
        
        rules.Should().HaveCount(2);
        
        rules[0].FailureMessage
            .Should()
            .Be("Name cannot be empty.");
        
        rules[1].FailureMessage
            .Should()
            .Be("Name must be unique.");
        
        rules[0].Rule(null)
            .Should()
            .BeFalse();
        
        rules[0].Rule(string.Empty)
            .Should()
            .BeFalse();
        
        rules[0].Rule("  ")
            .Should()
            .BeFalse();
        
        rules[0].Rule("Test")
            .Should()
            .BeTrue();
        
        rules[1].Rule("Test")
            .Should()
            .BeTrue();
        
        rules[1].Rule("Test2")
            .Should()
            .BeTrue();

        await _subject.NewPackAsync(_packFolder);

        var stream = new MemoryStream();
        var sourceFile = Substitute.For<IAvaloniaStorageFile>();
        sourceFile.OpenReadAsync().Returns(stream);

        await AddFileAsync("Test");
        
        rules[1].Rule("Test")
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public async Task NewPackAsync_ShouldAddNullImageFile()
    {
        await _subject.NewPackAsync(_packFolder);

        _derivedList.Should()
            .HaveCount(1)
            .And.Contain(x => x is NullImageFile);
    }

    [Fact]
    public async Task NewPackAsync_ShouldSaveEmptyManifest()
    {
        await _subject.NewPackAsync(_packFolder);

        var manifestFile = _packFolder.GetFile(ImageService.ManifestFileName);

        manifestFile.Should().NotBeNull();

        await using var stream = manifestFile.OpenRead();
        using var reader = new StreamReader(stream);

        var json = await reader.ReadToEndAsync(TestContext.Current.CancellationToken);

        var manifestDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        
        manifestDictionary.Should().BeEmpty();
    }

    [Fact]
    public async Task OpenPackAsync_ShouldAddNullImageFileOnly_WhenManifestDoesNotExist_WhenFolderDoesNotExist()
    {
        await _subject.OpenPackAsync(_packFolder);

        _derivedList.Should()
            .ContainSingle()
            .And.Contain(x => x is NullImageFile);
    }

    [Fact]
    public async Task OpenPackAsync_ShouldSaveEmptyManifest_WhenManifestDoesNotExist_WhenFolderDoesNotExist()
    {
        await _subject.OpenPackAsync(_packFolder);

        var manifestFile = _packFolder.GetFile(ImageService.ManifestFileName);
        
        manifestFile.Should().NotBeNull();

        await using var stream = manifestFile.OpenRead();
        using var reader = new StreamReader(stream);
        
        var json = await reader.ReadToEndAsync(TestContext.Current.CancellationToken);
        var manifestDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        
        manifestDictionary.Should().BeEmpty();
    }

    [Fact]
    public async Task OpenPackAsync_ShouldAddUntitledImages_WhenManifestDoesNotExist_WhenFolderExists()
    {
        var imageFolder = _packFolder.CreateFolder(ImageService.ImageFolderName);

        CreateImageFile(imageFolder);
        CreateImageFile(imageFolder);
        CreateImageFile(imageFolder);
        
        await _subject.OpenPackAsync(_packFolder);
        
        _derivedList.Should()
            .HaveCount(4)
            .And.Contain(x => x is NullImageFile)
            .And.Contain(x => x.FriendlyId == "Untitled 1")
            .And.Contain(x => x.FriendlyId == "Untitled 2")
            .And.Contain(x => x.FriendlyId == "Untitled 3");
    }
    
    [Fact]
    public async Task OpenPackAsync_ShouldSaveManifestWithUntitledImages_WhenManifestDoesNotExist_WhenFolderExists()
    {
        var imageFolder = _packFolder.CreateFolder(ImageService.ImageFolderName);
        
        var imageFiles = new List<IStorageFile>
        {
            CreateImageFile(imageFolder),
            CreateImageFile(imageFolder),
            CreateImageFile(imageFolder)
        };
        
        await _subject.OpenPackAsync(_packFolder);
        
        var manifestFile = _packFolder.GetFile(ImageService.ManifestFileName);
        
        manifestFile.Should().NotBeNull();

        await using var stream = manifestFile.OpenRead();
        using var reader = new StreamReader(stream);
        
        var json = await reader.ReadToEndAsync(TestContext.Current.CancellationToken);
        var manifestDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

        manifestDictionary.Should()
            .NotBeNull()
            .And.HaveCount(3)
            .And.ContainKey(imageFiles[0].Name)
            .And.ContainKey(imageFiles[1].Name)
            .And.ContainKey(imageFiles[2].Name);
        
        manifestDictionary[imageFiles[0].Name]
            .Should()
            .Be(_derivedList[1].FriendlyId);
        
        manifestDictionary[imageFiles[1].Name]
            .Should()
            .Be(_derivedList[2].FriendlyId);
        
        manifestDictionary[imageFiles[2].Name]
            .Should()
            .Be(_derivedList[3].FriendlyId);
    }
    
    [Fact]
    public async Task OpenPackAsync_ShouldAddNullImageFileOnly_WhenManifestExists_WhenFolderDoesNotExists()
    {
        _packFolder.CreateFile(ImageService.ManifestFileName);
        
        await _subject.OpenPackAsync(_packFolder);
        
        _derivedList.Should()
            .HaveCount(1)
            .And.Contain(x => x is NullImageFile);
    }
    
    [Fact]
    public async Task OpenPackAsync_ShouldSaveEmptyManifest_WhenManifestExists_WhenFolderDoesNotExists()
    {
        var manifestFile = _packFolder.CreateFile(ImageService.ManifestFileName);
        
        await _subject.OpenPackAsync(_packFolder);

        await using var stream = manifestFile.OpenRead();
        using var reader = new StreamReader(stream);
        
        var jsonText = await reader.ReadToEndAsync(TestContext.Current.CancellationToken);
        var manifestDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonText);
        
        manifestDictionary.Should().BeEmpty();
    }
    
    [Fact]
    public async Task OpenPackAsync_ShouldAddUntitledImages_WhenManifestExists_WhenFolderExists()
    {
        _packFolder.CreateFile(ImageService.ManifestFileName);
        
        var imageFolder = _packFolder.CreateFolder(ImageService.ImageFolderName);

        CreateImageFile(imageFolder);
        CreateImageFile(imageFolder);
        CreateImageFile(imageFolder);
        
        await _subject.OpenPackAsync(_packFolder);
        
        _derivedList.Should()
            .HaveCount(4)
            .And.Contain(x => x is NullImageFile)
            .And.Contain(x => x.FriendlyId == "Untitled 1")
            .And.Contain(x => x.FriendlyId == "Untitled 2")
            .And.Contain(x => x.FriendlyId == "Untitled 3");
    }
    
    [Fact]
    public async Task OpenPackAsync_ShouldSaveManifestWithUntitledImages_WhenManifestExists_WhenFolderExists()
    {
        var manifestFile = _packFolder.CreateFile(ImageService.ManifestFileName);
        var imageFolder = _packFolder.CreateFolder(ImageService.ImageFolderName);

        var imageFiles = new List<IStorageFile>
        {
            CreateImageFile(imageFolder),
            CreateImageFile(imageFolder),
            CreateImageFile(imageFolder)
        };
        
        await _subject.OpenPackAsync(_packFolder);

        await using var readStream = manifestFile.OpenRead();
        using var reader = new StreamReader(readStream);
        
        var json = await reader.ReadToEndAsync(TestContext.Current.CancellationToken);
        var manifestDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

        manifestDictionary.Should()
            .NotBeNull()
            .And.HaveCount(3)
            .And.ContainKey(imageFiles[0].Name)
            .And.ContainKey(imageFiles[1].Name)
            .And.ContainKey(imageFiles[2].Name);
        
        manifestDictionary[imageFiles[0].Name]
            .Should()
            .Be(_derivedList[1].FriendlyId);
        
        manifestDictionary[imageFiles[1].Name]
            .Should()
            .Be(_derivedList[2].FriendlyId);
        
        manifestDictionary[imageFiles[2].Name]
            .Should()
            .Be(_derivedList[3].FriendlyId);
    }
    
    [Fact]
    public async Task ClosePack_ShouldClearImages()
    {
        await _subject.NewPackAsync(_packFolder);
        _subject.ClosePack();
        
        _derivedList.Should().BeEmpty();
    }

    [Fact]
    public async Task AddFilesAsync_ShouldAddImage_WhenFolderIsLoaded_WhenDialogIsSuccess()
    {
        const string friendlyId = "Test";
        const string fileName = $"{friendlyId}.png";
        
        await _subject.NewPackAsync(_packFolder);
        
        var sourceStream = new MemoryStream();
        var sourceFile = Substitute.For<IAvaloniaStorageFile>();
        
        sourceFile.Name.Returns(fileName);
        sourceFile.OpenReadAsync().Returns(sourceStream);

        _subject.AddImagesDialog
            .RegisterHandler(context => context.SetOutput(new List<IAvaloniaStorageFile> { sourceFile }))
            .DisposeWith(_disposables);
        
        var results = await _subject.AddFilesAsync();
        
        results.Should()
            .HaveCount(1)
            .And.Contain(x => x.FriendlyId == friendlyId);
    }

    [Fact]
    public async Task AddFilesAsync_ShouldOpenExceptionDialog_WhenFolderIsLoaded_WhenDialogIsSuccess_WhenDestinationFileThrowsException()
    {
        var packFolder = Substitute.For<IStorageFolder>();
        var imagesFolder = Substitute.For<IStorageFolder>();
        
        packFolder.CreateFolder(ImageService.ImageFolderName).Returns(imagesFolder);
        
        packFolder.CreateFile(Arg.Any<string>())
            .Returns(_ =>
            {
                var file = Substitute.For<IStorageFile>();
                file.OpenWrite().Returns(new MemoryStream());
                
                return file;
            });
        
        await _subject.NewPackAsync(packFolder);
        
        var sourceStream = new MemoryStream();
        var sourceFile = Substitute.For<IAvaloniaStorageFile>();
        
        sourceFile.Name.Returns("Test.png");
        sourceFile.OpenReadAsync().Returns(sourceStream);
        
        _subject.AddImagesDialog
            .RegisterHandler(context => context.SetOutput(new List<IAvaloniaStorageFile> { sourceFile }))
            .DisposeWith(_disposables);

        imagesFolder
            .When(x => x.CreateFile(Arg.Any<string>()))
            .Throw<Exception>();

        var exceptionDialogShown = false;
        
        _subject.ExceptionDialog
            .RegisterHandler(context =>
            {
                exceptionDialogShown = true;
                context.SetOutput(Unit.Default);
            })
            .DisposeWith(_disposables);
        
        var results = await _subject.AddFilesAsync();
        
        results.Should().BeEmpty();
        
        exceptionDialogShown.Should().BeTrue();
    }

    [Fact]
    public async Task AddFilesAsync_ShouldDoNothing_WhenFolderIsLoaded_WhenDialogIsCancel()
    {
        await _subject.NewPackAsync(_packFolder);
        
        _subject.AddImagesDialog
            .RegisterHandler(context => context.SetOutput(null))
            .DisposeWith(_disposables);
        
        var results = await _subject.AddFilesAsync();

        results.Should().BeEmpty();
    }
    
    [Fact]
    public async Task AddFilesAsync_ShouldDoNothing_WhenFolderIsNotLoaded()
    {
        var results = await _subject.AddFilesAsync();

        results.Should().BeEmpty();
    }

    [Fact]
    public async Task RenameFileAsync_ShouldRenameFile_WhenFileIsLoaded_WhenDialogIsSuccess()
    {
        const string newFriendlyId = "Test2";
        
        await _subject.NewPackAsync(_packFolder);

        var imageFile = await AddFileAsync("Test");

        _subject.RenameImageDialog
            .RegisterHandler(context =>
            {
                context.Input.InputText = newFriendlyId;
                context.SetOutput(OperationResult.Success);
            })
            .DisposeWith(_disposables);
        
        await _subject.RenameFileAsync(imageFile);
        
        imageFile.FriendlyId
            .Should()
            .Be(newFriendlyId);
        
        await using var stream = _packFolder
            .GetFile(ImageService.ManifestFileName)!
            .OpenRead();
        
        using var reader = new StreamReader(stream);
        
        var jsonText = await reader.ReadToEndAsync(TestContext.Current.CancellationToken);
        var manifestDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonText);
        
        manifestDictionary.Should().NotBeNull();
        
        manifestDictionary[imageFile.Id.ToLowercaseString()]
            .Should()
            .Be(newFriendlyId);
    }

    [Fact]
    public async Task RenameFileAsync_ShouldDoNothing_WhenFileIsLoaded_WhenDialogIsCancel()
    {
        const string friendlyId = "Test";
        
        await _subject.NewPackAsync(_packFolder);

        var imageFile = await AddFileAsync(friendlyId);
        
        _subject.RenameImageDialog
            .RegisterHandler(context => context.SetOutput(OperationResult.Cancel))
            .DisposeWith(_disposables);
        
        await _subject.RenameFileAsync(imageFile);
        
        imageFile.FriendlyId
            .Should()
            .Be(friendlyId);
    }

    [Fact]
    public async Task RenameFileAsync_ShouldOpenExceptionDialog_WhenFileIsLoaded_WhenDialogIsYes_WhenRenameThrows()
    {
        var packFolder = Substitute.For<IStorageFolder>();
        
        var manifestFile = Substitute.For<IStorageFile>();
        var imagesFolder = Substitute.For<IStorageFolder>();

        packFolder.CreateFolder(Arg.Is(ImageService.ImageFolderName)).Returns(imagesFolder);
        
        packFolder
            .CreateFile(Arg.Is(ImageService.ManifestFileName))
            .Returns(manifestFile);

        manifestFile.OpenWrite().Returns(_ => new MemoryStream());

        imagesFolder
            .CreateFile(Arg.Any<string>())
            .Returns(info =>
            {
                var id = (string)info[0];
                var file = Substitute.For<IStorageFile>();
                file.OpenWrite().Returns(_ => new MemoryStream());
                file.Name.Returns(id);
                
                return file;
            });
        
        await _subject.NewPackAsync(packFolder);

        var imageFile = await AddFileAsync("Test");
        
        _subject.RenameImageDialog
            .RegisterHandler(context => context.SetOutput(OperationResult.Success))
            .DisposeWith(_disposables);

        manifestFile
            .When(x => x.OpenWrite())
            .Throw<Exception>();
        
        var exceptionDialogShown = false;
        
        _subject.ExceptionDialog
            .RegisterHandler(context =>
            {
                exceptionDialogShown = true;
                context.SetOutput(Unit.Default);
            })
            .DisposeWith(_disposables);
        
        await _subject.RenameFileAsync(imageFile);

        exceptionDialogShown.Should().BeTrue();
    }
    
    [Fact]
    public async Task RenameFileAsync_ShouldDoNothing_WhenFileIsNotLoaded()
    {
        await _subject.NewPackAsync(_packFolder);
        
        var imageFile = Substitute.For<IImageFile>();

        await _subject.RenameFileAsync(imageFile);
    }

    [Fact]
    public async Task DeleteFileAsync_ShouldDeleteFile_ShouldRemoveFileFromCollection_WhenFileIsLoaded_WhenDialogIsYes()
    {
        await _subject.NewPackAsync(_packFolder);

        var imageFile = await AddFileAsync("Test");
        var imageId = imageFile.Id.Value;
        
        _subject.DeleteImageDialog
            .RegisterHandler(context => context.SetOutput(YesNoDialogResult.Yes))
            .DisposeWith(_disposables);
        
        await _subject.DeleteFileAsync(imageFile);

        _derivedList.Should().ContainSingle();
        
        var imageFolder = _packFolder.GetFolder(ImageService.ImageFolderName);
        
        imageFolder.Should().NotBeNull();
        
        imageFolder.GetFile(imageId.ToLowercaseString())
            .Should()
            .BeNull();
    }

    [Fact]
    public async Task DeleteFileAsync_ShouldDoNothing_WhenFileIsLoaded_WhenDialogIsNo()
    {
        await _subject.NewPackAsync(_packFolder);

        var imageFile = await AddFileAsync("Test");
        var imageId = imageFile.Id.Value;
        
        _subject.DeleteImageDialog
            .RegisterHandler(context => context.SetOutput(YesNoDialogResult.No))
            .DisposeWith(_disposables);
        
        await _subject.DeleteFileAsync(imageFile);

        _derivedList.Should()
            .HaveCount(2)
            .And.Contain(x => x is NullImageFile)
            .And.Contain(x => x == imageFile);
        
        var imageFolder = _packFolder.GetFolder(ImageService.ImageFolderName);
        
        imageFolder.Should().NotBeNull();
        
        imageFolder.GetFile(imageId.ToLowercaseString())
            .Should()
            .NotBeSameAs(NullImageFile.Instance);
    }

    [Fact]
    public async Task DeleteFileAsync_ShouldOpenExceptionDialog_WhenFileIsLoaded_WhenDialogIsYes_WhenDeleteThrows()
    {
        var packFolder = Substitute.For<IStorageFolder>();
        var imagesFolder = Substitute.For<IStorageFolder>();

        packFolder.CreateFolder(Arg.Is(ImageService.ImageFolderName)).Returns(imagesFolder);
        
        packFolder
            .CreateFile(Arg.Any<string>())
            .Returns(info =>
            {
                var id = (string)info[0];
                var file = Substitute.For<IStorageFile>();
                file.OpenWrite().Returns(_ => new MemoryStream());
                file.Name.Returns(id);

                return file;
            });

        imagesFolder
            .CreateFile(Arg.Any<string>())
            .Returns(info =>
            {
                var id = (string)info[0];
                var file = Substitute.For<IStorageFile>();
                file.OpenWrite().Returns(_ => new MemoryStream());
                file.Name.Returns(id);
                file.When(x => x.Delete())
                    .Throw<Exception>();
                
                return file;
            });
        
        await _subject.NewPackAsync(packFolder);

        var imageFile = await AddFileAsync("Test");
        
        _subject.DeleteImageDialog
            .RegisterHandler(context => context.SetOutput(YesNoDialogResult.Yes))
            .DisposeWith(_disposables);

        var exceptionDialogShown = false;
        
        _subject.ExceptionDialog
            .RegisterHandler(context =>
            {
                exceptionDialogShown = true;
                context.SetOutput(Unit.Default);
            })
            .DisposeWith(_disposables);
        
        await _subject.DeleteFileAsync(imageFile);

        exceptionDialogShown.Should().BeTrue();
    }
    
    [Fact]
    public async Task DeleteFileAsync_ShouldDoNothing_WhenFileIsNotLoaded()
    {
        await _subject.NewPackAsync(_packFolder);
        
        var imageFile = Substitute.For<IImageFile>();

        await _subject.DeleteFileAsync(imageFile);
        
        _derivedList.Should()
            .ContainSingle()
            .And.Contain(x => x is NullImageFile);
    }

    [Fact]
    public async Task GetImageFile_ShouldReturnExpected_WhenFileIsLoaded()
    {
        await _subject.NewPackAsync(_packFolder);

        var sourceStream = new MemoryStream();
        var sourceFile = Substitute.For<IAvaloniaStorageFile>();
        
        sourceFile.OpenReadAsync().Returns(sourceStream);
        
        var imageFile = await AddFileAsync("Test");
        var imageId = imageFile.Id;
        
        var result = _subject.GetImageFile(imageId);
        
        result.Should().BeSameAs(imageFile);
    }

    [Fact]
    public void GetImageFile_ShouldReturnNullImageFile_WhenFileIsNotLoaded()
    {
        var result = _subject.GetImageFile(ImageId.New());

        result.Should().BeSameAs(NullImageFile.Instance);
    }

    [Fact]
    public void SplatResolve_ShouldResolveToSingleInstance()
    {
        var subject1 = Locator.Current.GetService<IImageService>()!;
        var subject2 = Locator.Current.GetService<IImageService>()!;

        subject1.Should().BeOfType<ImageService>();
        subject2.Should().BeOfType<ImageService>();
        
        subject1.Should().BeSameAs(subject2);
    }
    
    [Fact]
    public void SplatResolve_ShouldResolveToLazySingleInstance()
    {
        var lazy = Locator.Current.GetService<Lazy<IImageService>>()!;
        var subject = Locator.Current.GetService<IImageService>()!;

        lazy.Value
            .Should()
            .BeOfType<ImageService>();
        
        subject.Should().BeOfType<ImageService>();
        
        lazy.Value
            .Should()
            .BeSameAs(subject);
    }
}