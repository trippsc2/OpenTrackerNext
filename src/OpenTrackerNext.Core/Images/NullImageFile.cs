using System;
using System.Reflection;
using Avalonia.Media.Imaging;
using ReactiveUI;

namespace OpenTrackerNext.Core.Images;

/// <summary>
/// Represents a non-existent image file.
/// </summary>
public sealed class NullImageFile : ReactiveObject, IImageFile
{
    /// <summary>
    /// A <see cref="string"/> representing the friendly name of null files.
    /// </summary>
    public const string FriendlyIdConstant = "<none>";

    /// <summary>
    /// A <see cref="NullImageFile"/> representing a null image file instance.
    /// </summary>
    public static readonly NullImageFile Instance = new();

    private NullImageFile()
    {
    }

    /// <inheritdoc/>
    public ImageId Id => ImageId.Empty;

    /// <inheritdoc/>
    public string FriendlyId
    {
        get => FriendlyIdConstant;
        set => throw new NotSupportedException("This is a null image file.");
    }

    /// <inheritdoc/>
    public void Delete()
    {
        throw new NotSupportedException("This is a null image file.");
    }

    /// <inheritdoc/>
    public Bitmap GetBitmap()
    {
        const string resourceName = "OpenTrackerNext.Core.Assets.blank.png";
        
        var assembly = Assembly.GetExecutingAssembly();
        var stream = assembly.GetManifestResourceStream(resourceName);
        
        return new Bitmap(stream!);
    }
}