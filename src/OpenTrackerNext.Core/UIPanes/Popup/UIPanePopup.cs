using OpenTrackerNext.Core.Layouts;

namespace OpenTrackerNext.Core.UIPanes.Popup;

/// <inheritdoc />
public sealed class UIPanePopup : IUIPanePopup
{
    /// <inheritdoc/>
    public required string Icon { get; init; }

    /// <inheritdoc/>
    public required ILayout Body { get; init; }
}