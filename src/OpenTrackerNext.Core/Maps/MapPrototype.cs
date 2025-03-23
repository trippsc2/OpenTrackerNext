using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Images;
using OpenTrackerNext.Core.Maps.Static;
using OpenTrackerNext.Document;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.Maps;

/// <summary>
/// Represents a <see cref="IMap"/> within the editor.
/// </summary>
[Document]
public sealed partial class MapPrototype : ReactiveObject, ITitledDocumentData<MapPrototype>
{
    /// <summary>
    /// A <see cref="string"/> representing the title prefix of a map.
    /// </summary>
    public const string MapTitlePrefix = "Map - ";
    
    /// <inheritdoc cref="ITitledDocumentData{TSelf}.TitlePrefix" />
    public static string TitlePrefix => MapTitlePrefix;

    /// <summary>
    /// Gets or sets an <see cref="ImageId"/> representing the map image.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public IMapSubtypePrototype Content { get; set; } = new StaticMapPrototype();
}