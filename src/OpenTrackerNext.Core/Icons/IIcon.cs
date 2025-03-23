using System;
using System.ComponentModel;
using OpenTrackerNext.Core.Icons.States;

namespace OpenTrackerNext.Core.Icons;

/// <summary>
/// Represents an icon.
/// </summary>
public interface IIcon : IDisposable, INotifyPropertyChanged
{
    /// <summary>
    /// Gets a <see cref="IIconState"/> representing the current state of the icon.
    /// </summary>
    IIconState CurrentState { get; }

    /// <summary>
    /// Executes the logic for a left click on the icon.
    /// </summary>
    void OnLeftClick();
    
    /// <summary>
    /// Executes the logic for a right click on the icon.
    /// </summary>
    void OnRightClick();
}