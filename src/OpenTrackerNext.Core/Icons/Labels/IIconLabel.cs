using System;
using System.ComponentModel;

namespace OpenTrackerNext.Core.Icons.Labels;

/// <summary>
/// Represents an icon label.
/// </summary>
public interface IIconLabel : IDisposable, INotifyPropertyChanged
{
    /// <summary>
    /// Gets a <see cref="Nullable{T}"/> of <see cref="string"/> representing the icon label text.
    /// </summary>
    string? Text { get; }
    
    /// <summary>
    /// Gets a <see cref="IconLabelPosition"/> representing the icon label position.
    /// </summary>
    IconLabelPosition Position { get; }
}