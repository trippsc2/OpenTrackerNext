using OpenTrackerNext.Core.Images;
using OpenTrackerNext.Document;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.Icons.Static;

/// <summary>
/// Represents a <see cref="StaticIcon"/> within the editor.
/// </summary>
[Document]
public sealed partial class StaticIconPrototype : ReactiveObject, IIconSubtypePrototype
{
    /// <summary>
    /// Gets or sets the <see cref="ImageId"/> of the icon.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public ImageId Image { get; set; } = ImageId.Empty;
}