using OpenTrackerNext.Core.Layouts;
using ReactiveUI;

namespace OpenTrackerNext.Core.UIPanes;

/// <summary>
/// Represents a UI pane.
/// </summary>
public sealed class UIPane : ReactiveObject, IUIPane
{
    /// <summary>
    /// Gets a <see cref="string"/> representing the title of the UI pane.
    /// </summary>
    public required string Title { get; init; }
    
    /// <summary>
    /// Gets a <see cref="ILayout"/> representing the body layout.
    /// </summary>
    public required ILayout Body { get; init; }
}