using OpenTrackerNext.Core.Utils;
using ReactiveUI;

namespace OpenTrackerNext.Core.Requirements;

/// <summary>
/// Represents a null <see cref="IRequirement"/> object.
/// </summary>
public sealed class NullRequirement : ReactiveObject, INullObject<NullRequirement>, IRequirement
{
    private NullRequirement()
    {
    }

    /// <summary>
    /// Gets a singleton <see cref="NullRequirement"/> instance.
    /// </summary>
    public static NullRequirement Instance { get; } = new();

    /// <inheritdoc/>
    public bool IsMet => true;

    /// <inheritdoc/>
    public void Dispose()
    {
    }
}