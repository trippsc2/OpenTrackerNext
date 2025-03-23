using System.ComponentModel;
using OpenTrackerNext.Core.Storage;
#pragma warning disable CS0109 // Member does not hide an inherited member; new keyword is not required

namespace OpenTrackerNext.Core.Images;

/// <summary>
/// Represents an image file.
/// </summary>
public interface IImageFile : INotifyPropertyChanged, IReadOnlyImageFile
{
    /// <summary>
    /// A factory method that creates new <see cref="IImageFile"/> objects.
    /// </summary>
    /// <param name="file">
    /// A <see cref="IStorageFile"/> representing the file.
    /// </param>
    /// <returns>
    /// An <see cref="IImageFile"/> representing the new image file.
    /// </returns>
    public new delegate IImageFile Factory(IStorageFile file);
    
    /// <summary>
    /// Gets or sets a <see cref="string"/> representing the friendly name of the image file.
    /// </summary>
    string FriendlyId { get; set; }
    
    /// <summary>
    /// Deletes the image file.
    /// </summary>
    void Delete();
}