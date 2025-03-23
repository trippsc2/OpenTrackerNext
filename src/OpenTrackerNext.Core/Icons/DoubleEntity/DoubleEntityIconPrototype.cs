using System.Collections.ObjectModel;
using OpenTrackerNext.Core.Entities;
using OpenTrackerNext.Core.Icons.States;
using OpenTrackerNext.Document;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.Icons.DoubleEntity;

/// <summary>
/// Represents a <see cref="DoubleEntityIcon"/> within the editor.
/// </summary>
[Document]
public sealed partial class DoubleEntityIconPrototype : ReactiveObject, IIconSubtypePrototype
{
    /// <summary>
    /// Gets or sets an <see cref="EntityId"/> representing the first entity.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public EntityId Entity1 { get; set; } = EntityId.Empty;

    /// <summary>
    /// Gets or sets an <see cref="EntityId"/> representing the second entity.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public EntityId Entity2 { get; set; } = EntityId.Empty;

    /// <summary>
    /// Gets an <see cref="ObservableCollection{T}"/> of <see cref="IconStatePrototype"/> representing the icon states.
    /// </summary>
    [DocumentMember]
    public ObservableCollection<IconStatePrototype> States { get; init; } = [];
}