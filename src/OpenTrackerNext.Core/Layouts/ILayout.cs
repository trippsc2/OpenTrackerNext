using System;
using System.ComponentModel;
using Avalonia;

namespace OpenTrackerNext.Core.Layouts;

/// <summary>
/// Represents a UI layout.
/// </summary>
public interface ILayout : IDisposable, INotifyPropertyChanged
{
    /// <summary>
    /// Gets a <see cref="Thickness"/> representing the margin of the layout.
    /// </summary>
    Thickness Margin { get; }
}