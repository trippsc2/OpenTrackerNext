using System;
using System.ComponentModel;
using OpenTrackerNext.Core.Images;

namespace OpenTrackerNext.Core.Maps;

/// <summary>
/// Represents a map.
/// </summary>
public interface IMap : IDisposable, INotifyPropertyChanged
{
    /// <summary>
    /// Gets a <see cref="IReadOnlyImageFile"/> representing the map image.
    /// </summary>
    IReadOnlyImageFile Image { get; }
}