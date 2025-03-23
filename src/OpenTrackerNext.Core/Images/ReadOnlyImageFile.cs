using Avalonia.Media.Imaging;
using OpenTrackerNext.Core.Storage;
using OpenTrackerNext.SplatRegistration;
using ReactiveUI;

namespace OpenTrackerNext.Core.Images;

/// <inheritdoc cref="IReadOnlyImageFile" />
[Splat(RegisterAsType = typeof(IReadOnlyImageFile.Factory))]
public class ReadOnlyImageFile : ReactiveObject, IReadOnlyImageFile
{
    private readonly IReadOnlyStorageFile _readOnlyFile;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyImageFile"/> class.
    /// </summary>
    /// <param name="file">
    /// A <see cref="IReadOnlyImageFile"/> representing the file.
    /// </param>
    // ReSharper disable once MemberCanBeProtected.Global
    public ReadOnlyImageFile(IReadOnlyStorageFile file)
    {
        _readOnlyFile = file;
        Id = ImageId.Parse(file.Name);
    }

    /// <inheritdoc/>
    public ImageId Id { get; }
    
    /// <inheritdoc/>
    public Bitmap GetBitmap()
    {
        var stream = _readOnlyFile.OpenRead();
        var bitmap = new Bitmap(stream);

        return bitmap;
    }
}