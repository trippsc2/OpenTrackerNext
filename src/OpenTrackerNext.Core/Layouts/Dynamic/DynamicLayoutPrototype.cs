using OpenTrackerNext.Core.Requirements;
using OpenTrackerNext.Document;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.Layouts.Dynamic;

/// <summary>
/// Represents a <see cref="DynamicLayout"/> within the editor.
/// </summary>
[Document]
public sealed partial class DynamicLayoutPrototype : ReactiveObject, ILayoutSubtypePrototype
{
    /// <inheritdoc/>
    [DocumentMember]
    [Reactive]
    public double LeftMargin { get; set; }
    
    /// <inheritdoc/>
    [DocumentMember]
    [Reactive]
    public double TopMargin { get; set; }
    
    /// <inheritdoc/>
    [DocumentMember]
    [Reactive]
    public double RightMargin { get; set; }
    
    /// <inheritdoc/>
    [DocumentMember]
    [Reactive]
    public double BottomMargin { get; set; }
    
    /// <summary>
    /// Gets or sets a <see cref="RequirementPrototype"/> representing the requirement for the layout to be
    /// displayed.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public RequirementPrototype Requirement { get; set; } = new();
    
    /// <summary>
    /// Gets or sets a <see cref="LayoutId"/> representing the layout to display when the requirement is met.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public LayoutId MetLayout { get; set; } = LayoutId.Empty;
    
    /// <summary>
    /// Gets or sets a <see cref="LayoutId"/> representing the layout to display when the requirement is not met.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public LayoutId UnmetLayout { get; set; } = LayoutId.Empty;
}