using OpenTrackerNext.Core.Entities;
using OpenTrackerNext.Core.Icons.Labels;
using OpenTrackerNext.Core.Images;
using OpenTrackerNext.Document;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.Icons.Counted;

/// <summary>
/// Represents a <see cref="CountedIcon"/> within the editor.
/// </summary>
[Document]
public sealed partial class CountedIconPrototype : ReactiveObject, IIconSubtypePrototype
{
    /// <summary>
    /// Gets or sets an <see cref="EntityId"/> representing the entity.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public EntityId Entity { get; set; } = EntityId.Empty;

    /// <summary>
    /// Gets or sets an <see cref="ImageId"/> representing the default image.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public ImageId DefaultImage { get; set; } = ImageId.Empty;
    
    /// <summary>
    /// Gets or sets a value indicating whether the icon has a disabled image.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public bool HasDisabledImage { get; set; }

    /// <summary>
    /// Gets or sets an <see cref="ImageId"/> representing the disabled image.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public ImageId DisabledImage { get; set; } = ImageId.Empty;

    /// <summary>
    /// Gets or sets an <see cref="IconLabelPosition"/> representing the label position.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public IconLabelPosition LabelPosition { get; set; } = IconLabelPosition.BottomRight;

    /// <summary>
    /// Gets or sets a value indicating whether a <see cref="bool"/> representing whether the label is hidden at the
    /// minimum entity value.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public bool HideLabelAtMinimum { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether a <see cref="bool"/> representing whether the label should have an
    /// asterisk at the maximum entity value. 
    /// </summary>
    [DocumentMember]
    [Reactive]
    public bool AddAsteriskAtMaximum { get; set; } = true;

    /// <summary>
    /// Gets or sets a <see cref="IndeterminateState"/> representing whether and at what value the entity has an
    /// indeterminate state.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public IndeterminateState IndeterminateState { get; set; } = IndeterminateState.None;

    /// <summary>
    /// Gets or sets a value indicating whether the click actions should be reversed.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public bool SwapClickActions { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity counts should cycle after maximum or minimum.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public bool CycleEntityCounts { get; set; }
}