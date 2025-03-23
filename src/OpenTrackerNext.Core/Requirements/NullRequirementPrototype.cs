using OpenTrackerNext.Document;
using ReactiveUI;

namespace OpenTrackerNext.Core.Requirements;

/// <summary>
/// Represents a null <see cref="IRequirementSubtypePrototype"/> object.
/// </summary>
[Document]
public sealed partial class NullRequirementPrototype : ReactiveObject, IRequirementSubtypePrototype;