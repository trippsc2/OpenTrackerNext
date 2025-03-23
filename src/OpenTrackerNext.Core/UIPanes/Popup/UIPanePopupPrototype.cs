using OpenTrackerNext.Core.Layouts;
using OpenTrackerNext.Document;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.UIPanes.Popup;

/// <inheritdoc cref="IUIPanePopupPrototype" />
[Document]
public sealed partial class UIPanePopupPrototype : ReactiveObject, IUIPanePopupPrototype
{
    /// <summary>
    /// Gets or sets a <see cref="string"/> representing the button icon of the UI pane.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public string Icon { get; set; } = "cog";
    
    /// <summary>
    /// Gets or sets a <see cref="LayoutId"/> representing the body layout ID.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public LayoutId Body { get; set; } = LayoutId.Empty;
}