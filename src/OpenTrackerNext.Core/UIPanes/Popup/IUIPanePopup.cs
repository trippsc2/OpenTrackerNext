using OpenTrackerNext.Core.Layouts;

namespace OpenTrackerNext.Core.UIPanes.Popup;

/// <summary>
/// Represents a UI pane popup.
/// </summary>
public interface IUIPanePopup
{
    /// <summary>
    /// Gets a <see cref="string"/> representing the icon of the popup.
    /// </summary>
    string Icon { get; }
    
    /// <summary>
    /// Gets a <see cref="ILayout"/> representing the body layout of the popup.
    /// </summary>
    ILayout Body { get; }
}