using System;
using System.ComponentModel;
using OpenTrackerNext.Core.Icons;

namespace OpenTrackerNext.Core.Layouts.IconGrid;

/// <summary>
/// Represents an icon within an icon grid layout.
/// </summary>
public interface IIconLayout : IDisposable, INotifyPropertyChanged
{
    /// <summary>
    /// Gets or sets an <see cref="int"/> representing the icon height.
    /// </summary>
    int Height { get; set; }
    
    /// <summary>
    /// Gets or sets an <see cref="int"/> representing the icon width.
    /// </summary>
    int Width { get; set; }
    
    /// <summary>
    /// Gets an <see cref="IIcon"/> representing the current icon.
    /// </summary>
    IIcon CurrentIcon { get; }
}