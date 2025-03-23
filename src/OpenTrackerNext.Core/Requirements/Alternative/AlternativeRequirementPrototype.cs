using System.Collections.ObjectModel;
using OpenTrackerNext.Document;
using ReactiveUI;

namespace OpenTrackerNext.Core.Requirements.Alternative;

/// <summary>
/// Represents an <see cref="AlternativeRequirement"/> within the editor.
/// </summary>
[Document]
public sealed partial class AlternativeRequirementPrototype : ReactiveObject, IRequirementSubtypePrototype
{
    /// <summary>
    /// Gets an <see cref="ObservableCollection{T}"/> of <see cref="RequirementPrototype"/> representing the
    /// requirements to be aggregated.
    /// </summary>
    [DocumentMember]
    public ObservableCollection<RequirementPrototype> Requirements { get; init; } = [];
}