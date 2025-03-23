using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Layouts;
using OpenTrackerNext.Core.Requirements;
using OpenTrackerNext.Core.UIPanes.Popup;
using OpenTrackerNext.Document;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.UIPanes;

/// <summary>
/// Represents the UI pane data within the editor.
/// </summary>
[Document]
public sealed partial class UIPanePrototype : ReactiveObject, ITitledDocumentData<UIPanePrototype>
{
    /// <summary>
    /// A <see cref="string"/> representing the UI pane title prefix.
    /// </summary>
    public const string UIPaneTitlePrefix = "UI Pane - ";
    
    /// <inheritdoc cref="ITitledDocumentData{TSelf}.TitlePrefix" />
    public static string TitlePrefix => UIPaneTitlePrefix;
    
    /// <summary>
    /// Gets or sets a <see cref="RequirementPrototype"/> representing the visibility requirement.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public RequirementPrototype Visibility { get; set; } = new();
    
    /// <summary>
    /// Gets or sets a <see cref="UIPanePopupPrototype"/> representing the UI pane popup data.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public IUIPanePopupPrototype Popup { get; set; } = new NullUIPanePopupPrototype();
    
    /// <summary>
    /// Gets or sets a <see cref="string"/> representing the title of the UI pane.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets a <see cref="LayoutId"/> representing the body layout ID.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public LayoutId Body { get; set; } = LayoutId.Empty;
}