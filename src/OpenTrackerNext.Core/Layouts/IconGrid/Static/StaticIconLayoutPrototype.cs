using OpenTrackerNext.Core.Icons;
using OpenTrackerNext.Document;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.Layouts.IconGrid.Static;

/// <summary>
/// Represents a <see cref="StaticIconLayout"/> within the editor.
/// </summary>
[Document]
public sealed partial class StaticIconLayoutPrototype : ReactiveObject, IIconLayoutSubtypePrototype
{
    /// <summary>
    /// Gets or sets an <see cref="IconId"/> representing the icon.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public IconId Icon { get; set; } = IconId.Empty;
}