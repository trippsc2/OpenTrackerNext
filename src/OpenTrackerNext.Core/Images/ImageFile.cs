using OpenTrackerNext.Core.Storage;
using OpenTrackerNext.SplatRegistration;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.Images;

/// <inheritdoc cref="IImageFile" />
[Splat(RegisterAsType = typeof(IImageFile.Factory))]
public sealed class ImageFile : ReadOnlyImageFile, IImageFile
{
    private readonly IStorageFile _file;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ImageFile"/> class.
    /// </summary>
    /// <param name="file">
    /// A <see cref="IStorageFile"/> representing the file.
    /// </param>
    public ImageFile(IStorageFile file)
        : base(file)
    {
        _file = file;
    }

    /// <inheritdoc/>
    [Reactive]
    public string FriendlyId { get; set; } = string.Empty;
    
    /// <inheritdoc/>
    public void Delete()
    {
        _file.Delete();
    }
}