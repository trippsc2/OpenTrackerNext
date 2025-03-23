using OpenTrackerNext.Document;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.Requirements.UIPanel;

/// <summary>
/// Represents the <see cref="UIPanelDockRequirement"/> prototype.
/// </summary>
[Document]
public sealed partial class UIPanelDockRequirementPrototype : ReactiveObject, IRequirementSubtypePrototype
{
    /// <summary>
    /// Gets or sets a <see cref="UIPanelDock"/> representing the required UI pane direction.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public UIPanelDock UIPanelDock { get; set; } = UIPanelDock.Left;
}