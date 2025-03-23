using OpenTrackerNext.Core.Images;
using OpenTrackerNext.Core.Requirements;
using OpenTrackerNext.Document;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.Maps.Dynamic;

/// <summary>
/// Represents a <see cref="DynamicMap"/> within the editor.
/// </summary>
[Document]
public sealed partial class DynamicMapPrototype : ReactiveObject, IMapSubtypePrototype
{
    /// <summary>
    /// Gets or sets a <see cref="IRequirementSubtypePrototype"/> representing the requirement for the map.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public RequirementPrototype Requirement { get; set; } = new();
    
    /// <summary>
    /// Gets or sets the <see cref="ImageId"/> representing the map image when the requirement is met.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public ImageId MetImage { get; set; } = ImageId.Empty;
    
    /// <summary>
    /// Gets or sets the <see cref="ImageId"/> representing the map image when the requirement is not met.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public ImageId UnmetImage { get; set; } = ImageId.Empty;
}