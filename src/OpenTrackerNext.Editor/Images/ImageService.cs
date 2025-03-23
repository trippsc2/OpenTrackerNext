using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Kernel;
using OpenTrackerNext.Core.Ids;
using OpenTrackerNext.Core.Images;
using OpenTrackerNext.Core.Json;
using OpenTrackerNext.Core.Results;
using OpenTrackerNext.Core.Storage;
using OpenTrackerNext.Core.Validation;
using OpenTrackerNext.Core.ViewModels.Dialogs;
using OpenTrackerNext.SplatRegistration;
using ReactiveUI;
using IAvaloniaStorageFile = Avalonia.Platform.Storage.IStorageFile;

namespace OpenTrackerNext.Editor.Images;

/// <inheritdoc />
[Splat(RegisterAsType = typeof(IImageService))]
[SplatSingleInstance]
public sealed class ImageService : IImageService
{
    /// <summary>
    /// A <see cref="string"/> representing the images folder name.
    /// </summary>
    public const string ImageFolderName = "Images";
    
    /// <summary>
    /// A <see cref="string"/> representing the image manifest file name.
    /// </summary>
    public const string ManifestFileName = "images.json";

    private readonly IImageFile.Factory _imageFileFactory;

    private readonly SourceCache<IImageFile, ImageId> _images = new(image => image.Id);

    private IStorageFolder? _folder;
    private IStorageFile? _manifestFile;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ImageService"/> class.
    /// </summary>
    /// <param name="imageFileFactory">
    /// An Autofac factory for creating new <see cref="IImageFile"/> objects.
    /// </param>
    public ImageService(IImageFile.Factory imageFileFactory)
    {
        _imageFileFactory = imageFileFactory;

        NameValidationRules =
        [
            new ValidationRule<string?>
            {
                Rule = name => !string.IsNullOrWhiteSpace(name),
                FailureMessage = "Name cannot be empty."
            },
            new ValidationRule<string?>
            {
                Rule = name => _images.Items.All(x => x.FriendlyId != name),
                FailureMessage = "Name must be unique."
            }
        ];
    }

    /// <inheritdoc/>
    public List<ValidationRule<string?>> NameValidationRules { get; }

    /// <inheritdoc/>
    public Interaction<Unit, IReadOnlyList<IAvaloniaStorageFile>?> AddImagesDialog { get; } = new();
    
    /// <inheritdoc/>
    public Interaction<TextBoxDialogViewModel, OperationResult> RenameImageDialog { get; } = new();
    
    /// <inheritdoc/>
    public Interaction<YesNoDialogViewModel, YesNoDialogResult> DeleteImageDialog { get; } = new();
    
    /// <inheritdoc/>
    public Interaction<ExceptionDialogViewModel, Unit> ExceptionDialog { get; } = new();
    
    /// <inheritdoc/>
    public IObservable<IChangeSet<IImageFile, ImageId>> Connect()
    {
        return _images.Connect();
    }

    /// <inheritdoc/>
    public async Task NewPackAsync(IStorageFolder packFolder)
    {
        _manifestFile = packFolder.CreateFile(ManifestFileName);
        _folder = packFolder.CreateFolder(ImageFolderName);
        _images.AddOrUpdate(NullImageFile.Instance);

        await WriteManifestToFileAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task OpenPackAsync(IStorageFolder packFolder)
    {
        var manifestData = await GetInitialManifestDataAsync(packFolder).ConfigureAwait(false);

        _folder = packFolder.GetFolder(ImageFolderName);

        if (_folder is null)
        {
            await NewPackAsync(packFolder).ConfigureAwait(false);
            return;
        }
        
        _images.AddOrUpdate(NullImageFile.Instance);

        var untitledCount = 0;

        foreach (var file in _folder.GetFiles())
        {
            var imageFile = _imageFileFactory(file);

            imageFile.FriendlyId =
                manifestData.TryGetValue(imageFile.Id.ToLowercaseString(), out var friendlyName)
                    ? friendlyName
                    : $"Untitled {++untitledCount}";

            _images.AddOrUpdate(imageFile);
        }

        await WriteManifestToFileAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public void ClosePack()
    {
        _images.Clear();
        _folder = null;
        _manifestFile = null;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<IImageFile>> AddFilesAsync()
    {
        if (_folder is null)
        {
            return new List<IImageFile>();
        }

        var sourceFiles = await AddImagesDialog.Handle(Unit.Default);

        if (sourceFiles is null)
        {
            return new List<IImageFile>();
        }

        var newImageFiles = new List<IImageFile>();

        foreach (var sourceFile in sourceFiles)
        {
            var friendlyName = Path.GetFileNameWithoutExtension(sourceFile.Name);

            try
            {
                await using var sourceStream = await sourceFile.OpenReadAsync().ConfigureAwait(false);

                var destinationFile = _folder!.CreateFile(Guid.NewGuid().ToLowercaseString());
                await using var destinationStream = destinationFile.OpenWrite();

                await sourceStream.CopyToAsync(destinationStream).ConfigureAwait(false);

                var imageFile = _imageFileFactory(destinationFile);
                imageFile.FriendlyId = friendlyName;

                _images.AddOrUpdate(imageFile);

                await WriteManifestToFileAsync().ConfigureAwait(false);

                newImageFiles.Add(imageFile);
            }
            catch (Exception exception)
            {
                await ExceptionDialog.Handle(new ExceptionDialogViewModel(exception));
            }
        }

        return newImageFiles;
    }

    /// <inheritdoc/>
    public async Task RenameFileAsync(IImageFile imageFile)
    {
        if (!_images.Items.Contains(imageFile))
        {
            return;
        }

        var textBoxDialog = new TextBoxDialogViewModel(DialogIcon.Question, NameValidationRules)
        {
            Title = $"Rename Image - {imageFile.FriendlyId}",
            Message = "Enter Name"
        };
        var result = await RenameImageDialog.Handle(textBoxDialog);

        if (result.IsCancel)
        {
            return;
        }

        try
        {
            imageFile.FriendlyId = textBoxDialog.InputText;

            await WriteManifestToFileAsync().ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            await ExceptionDialog.Handle(new ExceptionDialogViewModel(exception));
        }
    }

    /// <inheritdoc/>
    public async Task DeleteFileAsync(IImageFile imageFile)
    {
        if (!_images.Items.Contains(imageFile))
        {
            return;
        }

        var dialog = new YesNoDialogViewModel(DialogIcon.Question)
        {
            Title = $"Delete Image - {imageFile.FriendlyId}",
            Message = $"Are you sure you want to delete 'Image - {imageFile.FriendlyId}'?"
        };
        var result = await DeleteImageDialog.Handle(dialog);

        if (result.IsNo)
        {
            return;
        }

        try
        {
            _images.Remove(imageFile);
            imageFile.Delete();

            await WriteManifestToFileAsync().ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            await ExceptionDialog.Handle(new ExceptionDialogViewModel(exception));
        }
    }

    /// <inheritdoc/>
    public IImageFile GetImageFile(ImageId id)
    {
        return _images
            .Lookup(id)
            .ValueOr(() => NullImageFile.Instance);
    }

    private async Task<Dictionary<string, string>> GetInitialManifestDataAsync(IStorageFolder packFolder)
    {
        _manifestFile = packFolder.GetFile(ManifestFileName);

        if (_manifestFile is null)
        {
            _manifestFile = packFolder.CreateFile(ManifestFileName);

            return new Dictionary<string, string>();
        }

        try
        {
            return await ReadManifestFromFileAsync().ConfigureAwait(false);
        }
        catch
        {
            return new Dictionary<string, string>();
        }
    }

    private async Task<Dictionary<string, string>> ReadManifestFromFileAsync()
    {
        var stream = _manifestFile!.OpenRead();
        await using (stream.ConfigureAwait(false))
        {
            return await JsonContext
                .DeserializeAsync<Dictionary<string, string>>(stream)
                .ConfigureAwait(false);
        }
    }

    private async Task WriteManifestToFileAsync()
    {
        var dictionary = _images.Items
            .Where(image => image != NullImageFile.Instance)
            .ToDictionary(image => image.Id.Value, image => image.FriendlyId);
        
        var stream = _manifestFile!.OpenWrite();
        await using (stream.ConfigureAwait(false))
        {
            await JsonContext
                .SerializeAsync(stream, dictionary)
                .ConfigureAwait(false);
        }
    }
}