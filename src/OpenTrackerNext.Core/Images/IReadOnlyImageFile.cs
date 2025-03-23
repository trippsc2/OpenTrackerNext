using Avalonia.Media.Imaging;
using OpenTrackerNext.Core.Storage;

namespace OpenTrackerNext.Core.Images;

/// <summary>
/// Represents a read-only image file.
/// </summary>
public interface IReadOnlyImageFile
{
    /// <summary>
    /// A factory method for creating new <see cref="IReadOnlyImageFile"/> objects with the specified file.
    /// </summary>
    /// <param name="file">
    /// A <see cref="IReadOnlyStorageFile"/> representing the file.
    /// </param>
    /// <returns>
    /// A new <see cref="IReadOnlyImageFile"/> object.
    /// </returns>
    public delegate IReadOnlyImageFile Factory(IReadOnlyStorageFile file);

    /// <summary>
    /// Gets a <see cref="ImageId"/> representing the ID of the image file.
    /// </summary>
    ImageId Id { get; }
    
    /// <summary>
    /// Returns the bitmap content of the image file.
    /// </summary>
    /// <returns>
    /// A <see cref="Bitmap"/> representing the bitmap content of the image file.
    /// </returns>
    Bitmap GetBitmap();
}