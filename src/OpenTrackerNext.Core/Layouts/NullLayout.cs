using Avalonia;
using OpenTrackerNext.Core.Utils;
using ReactiveUI;

namespace OpenTrackerNext.Core.Layouts;

/// <summary>
/// Represents a null <see cref="ILayout"/> object.
/// </summary>
public sealed class NullLayout : ReactiveObject, ILayout, INullObject<NullLayout>
{
    private NullLayout()
    {
    }

    /// <summary>
    /// Gets a singleton instance of the <see cref="NullLayout"/> class.
    /// </summary>
    public static NullLayout Instance { get; } = new();

    /// <inheritdoc />
    public Thickness Margin => default;

    /// <inheritdoc/>
    public void Dispose()
    {
    }
}