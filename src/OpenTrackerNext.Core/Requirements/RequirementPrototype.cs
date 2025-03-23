using OpenTrackerNext.Document;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.Requirements;

/// <summary>
/// Represents an <see cref="IRequirement"/> within the editor.
/// </summary>
[Document]
public sealed partial class RequirementPrototype : ReactiveObject
{
    /// <summary>
    /// Gets or sets the <see cref="IRequirementSubtypePrototype"/> representing the requirement content.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public IRequirementSubtypePrototype Content { get; set; } = new NullRequirementPrototype();
}