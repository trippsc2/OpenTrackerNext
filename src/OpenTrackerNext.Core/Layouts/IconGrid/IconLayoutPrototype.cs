using OpenTrackerNext.Core.Layouts.IconGrid.Static;
using OpenTrackerNext.Document;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.Layouts.IconGrid;

/// <summary>
/// Represents an icon within an icon grid layout within the editor.
/// </summary>
[Document]
public sealed partial class IconLayoutPrototype : ReactiveObject
{
    /// <summary>
    /// Gets or sets an <see cref="IIconLayoutSubtypePrototype"/> representing the content of the layout.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public IIconLayoutSubtypePrototype Content { get; set; } = new StaticIconLayoutPrototype();
}