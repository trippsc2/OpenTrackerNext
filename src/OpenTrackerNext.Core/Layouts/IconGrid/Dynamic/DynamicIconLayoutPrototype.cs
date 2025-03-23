using OpenTrackerNext.Core.Icons;
using OpenTrackerNext.Core.Requirements;
using OpenTrackerNext.Document;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.Layouts.IconGrid.Dynamic;

/// <summary>
/// Represents a <see cref="DynamicIconLayout"/> within the editor.
/// </summary>
[Document]
public sealed partial class DynamicIconLayoutPrototype : ReactiveObject, IIconLayoutSubtypePrototype
{
    /// <summary>
    /// Gets or sets the <see cref="RequirementPrototype"/> that determines which icon to display.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public RequirementPrototype Requirement { get; set; } = new();
    
    /// <summary>
    /// Gets or sets an <see cref="IconId"/> representing the icon to display when the requirement is met.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public IconId MetIcon { get; set; } = IconId.Empty;
    
    /// <summary>
    /// Gets or sets an <see cref="IconId"/> representing the icon to display when the requirement is not met.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public IconId UnmetIcon { get; set; } = IconId.Empty;
}