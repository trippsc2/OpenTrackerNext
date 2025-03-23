using System.Collections.ObjectModel;
using OpenTrackerNext.Document;
using ReactiveUI;

namespace OpenTrackerNext.Core.Requirements.Aggregate;

/// <summary>
/// Represents an <see cref="AggregateRequirement"/> within the editor.
/// </summary>
[Document]
public sealed partial class AggregateRequirementPrototype : ReactiveObject, IRequirementSubtypePrototype
{
    /// <summary>
    /// Gets an <see cref="ObservableCollection{T}"/> of <see cref="RequirementPrototype"/> representing the
    /// requirements to be aggregated.
    /// </summary>
    [DocumentMember]
    public ObservableCollection<RequirementPrototype> Requirements { get; init; } = [];
}