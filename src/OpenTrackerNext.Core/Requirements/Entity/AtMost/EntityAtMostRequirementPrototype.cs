using OpenTrackerNext.Core.Entities;
using OpenTrackerNext.Document;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.Requirements.Entity.AtMost;

/// <summary>
/// Represents an <see cref="EntityAtMostRequirement"/> within the editor.
/// </summary>
[Document]
public sealed partial class EntityAtMostRequirementPrototype : ReactiveObject, IRequirementSubtypePrototype
{
    /// <summary>
    /// Gets or sets an <see cref="EntityId"/> representing the required entity.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public EntityId Entity { get; set; }
    
    /// <summary>
    /// Gets or sets an <see cref="int"/> representing the required number of entities.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public int RequiredValue { get; set; }
}