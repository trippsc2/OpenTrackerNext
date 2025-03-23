using System.Collections.ObjectModel;
using OpenTrackerNext.Core.Entities;
using OpenTrackerNext.Core.Icons.States;
using OpenTrackerNext.Document;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.Icons.SingleEntity;

/// <summary>
/// Represents a <see cref="SingleEntityIcon"/> within the editor.
/// </summary>
[Document]
public sealed partial class SingleEntityIconPrototype : ReactiveObject, IIconSubtypePrototype
{
    /// <summary>
    /// Gets or sets an <see cref="EntityId"/> representing the entity.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public EntityId Entity { get; set; } = EntityId.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether to swap the left and right click actions.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public bool SwapClickActions { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to cycle the counts when at minimum/maximum.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public bool CycleEntityCounts { get; set; }

    /// <summary>
    /// Gets an <see cref="ObservableCollection{T}"/> of <see cref="IconStatePrototype"/> representing the icon states.
    /// </summary>
    [DocumentMember]
    public ObservableCollection<IconStatePrototype> States { get; init; } = [];
}