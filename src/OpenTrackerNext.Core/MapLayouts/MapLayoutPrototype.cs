using System.Collections.ObjectModel;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Document;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.MapLayouts;

/// <summary>
/// Represents a <see cref="IMapLayout"/> within the editor.
/// </summary>
[Document]
public sealed partial class MapLayoutPrototype : ReactiveObject, ITitledDocumentData<MapLayoutPrototype>
{
    /// <summary>
    /// A <see cref="string"/> representing the title prefix of a map layout.
    /// </summary>
    public const string MapLayoutTitlePrefix = "Map Layout - ";
    
    /// <inheritdoc cref="ITitledDocumentData{TSelf}.TitlePrefix" />
    public static string TitlePrefix => MapLayoutTitlePrefix;

    /// <summary>
    /// Gets or sets a <see cref="string"/> representing the name of the map layout.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets an <see cref="ObservableCollection{T}"/> of <see cref="MapLayoutMapPrototype"/> representing the maps of
    /// the map layout.
    /// </summary>
    [DocumentMember]
    public ObservableCollection<MapLayoutMapPrototype> Maps { get; init; } = [];
}