using OpenTrackerNext.Core.Layouts;
using OpenTrackerNext.Core.Utils;
using ReactiveUI;

namespace OpenTrackerNext.Core.UIPanes;

/// <summary>
/// Represents a null <see cref="IUIPane"/> object.
/// </summary>
public sealed class NullUIPane : ReactiveObject, INullObject<NullUIPane>, IUIPane
{
    private NullUIPane()
    {
    }

    /// <summary>
    /// Gets a singleton instance of the <see cref="NullUIPane"/> class.
    /// </summary>
    public static NullUIPane Instance { get; } = new();

    /// <inheritdoc/>
    public string Title => string.Empty;

    /// <inheritdoc/>
    public ILayout Body => NullLayout.Instance;
}