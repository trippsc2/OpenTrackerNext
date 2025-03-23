using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Icons.Static;
using OpenTrackerNext.Document;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.Icons;

/// <summary>
/// Represents an <see cref="IIcon"/> within the editor.
/// </summary>
[Document]
public sealed partial class IconPrototype : ReactiveObject, ITitledDocumentData<IconPrototype>
{
    /// <summary>
    /// A <see cref="string"/> representing the title prefix of the icon.
    /// </summary>
    public const string IconTitlePrefix = "Icon - ";
    
    /// <inheritdoc cref="ITitledDocumentData{TSelf}.TitlePrefix" />
    public static string TitlePrefix => IconTitlePrefix;

    /// <summary>
    /// Gets or sets a <see cref="IIconSubtypePrototype"/> representing the icon content.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public IIconSubtypePrototype Content { get; set; } = new StaticIconPrototype();
}