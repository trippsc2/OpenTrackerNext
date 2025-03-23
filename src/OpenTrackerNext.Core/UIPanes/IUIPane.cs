using System.ComponentModel;
using OpenTrackerNext.Core.Layouts;

namespace OpenTrackerNext.Core.UIPanes;

/// <summary>
/// Represents a UI pane.
/// </summary>
public interface IUIPane : INotifyPropertyChanged
{
    /// <summary>
    /// Gets a <see cref="string"/> representing the name of the UI pane.
    /// </summary>
    string Title { get; }
    
    /// <summary>
    /// Gets a <see cref="ILayout"/> representing the body layout of the UI pane.
    /// </summary>
    ILayout Body { get; }
}