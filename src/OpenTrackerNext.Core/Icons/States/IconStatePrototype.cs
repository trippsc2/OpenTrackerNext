using OpenTrackerNext.Core.Images;
using OpenTrackerNext.Document;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.Icons.States;

/// <summary>
/// Represents an icon state of an entity <see cref="IIcon"/> within the editor.
/// </summary>
[Document]
public sealed partial class IconStatePrototype : ReactiveObject
{
    /// <summary>
    /// Gets or sets an <see cref="ImageId"/> representing the image.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public ImageId Image { get; set; } = ImageId.Empty;
}