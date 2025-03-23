using OpenTrackerNext.Core.Images;
using OpenTrackerNext.Document;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.Maps.Static;

/// <summary>
/// Represents a <see cref="StaticMap"/> within the editor.
/// </summary>
[Document]
public sealed partial class StaticMapPrototype : ReactiveObject, IMapSubtypePrototype
{
    /// <summary>
    /// Gets or sets an <see cref="ImageId"/> representing the map image.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public ImageId Image { get; set; } = ImageId.Empty;
}