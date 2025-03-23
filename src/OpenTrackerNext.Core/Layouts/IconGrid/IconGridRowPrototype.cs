using System.Collections.ObjectModel;
using OpenTrackerNext.Document;
using ReactiveUI;

namespace OpenTrackerNext.Core.Layouts.IconGrid;

/// <summary>
/// Represents a row of icons in a <see cref="IconGridLayoutPrototype"/> within the editor.
/// </summary>
[Document]
public sealed partial class IconGridRowPrototype : ReactiveObject
{
    /// <summary>
    /// Gets an <see cref="ObservableCollection{T}"/> of <see cref="IconLayoutPrototype"/> representing the icons in the
    /// row.
    /// </summary>
    [DocumentMember]
    public ObservableCollection<IconLayoutPrototype> Icons { get; init; } = [];
}