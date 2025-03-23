using OpenTrackerNext.Core.Entities;
using OpenTrackerNext.Document;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.Requirements.Entity.Not;

/// <summary>
/// Represents an <see cref="EntityNotRequirement"/> within the editor.
/// </summary>
[Document]
public sealed partial class EntityNotRequirementPrototype : ReactiveObject, IRequirementSubtypePrototype
{
    /// <summary>
    /// Gets or sets an <see cref="EntityId"/> representing the required entity.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public EntityId Entity { get; set; } = EntityId.Empty;
    
    /// <summary>
    /// Gets or sets an <see cref="int"/> representing the excluded entity value.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public int ExcludedValue { get; set; }
}