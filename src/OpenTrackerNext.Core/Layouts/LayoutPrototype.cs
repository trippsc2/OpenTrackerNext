using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Layouts.IconGrid;
using OpenTrackerNext.Document;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.Layouts;

/// <summary>
/// Represents a <see cref="ILayout"/> within the editor.
/// </summary>
[Document]
public sealed partial class LayoutPrototype : ReactiveObject, ITitledDocumentData<LayoutPrototype>
{
    /// <summary>
    /// A <see cref="string"/> representing the layouts folder name.
    /// </summary>
    public const string LayoutTitlePrefix = "Layout - ";
    
    /// <inheritdoc cref="ITitledDocumentData{TSelf}.TitlePrefix" />
    public static string TitlePrefix => LayoutTitlePrefix;

    /// <summary>
    /// Gets or sets a <see cref="ILayoutSubtypePrototype"/> representing the layout content.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public ILayoutSubtypePrototype Content { get; set; } = new IconGridLayoutPrototype();
}