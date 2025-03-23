using System;
using OpenTrackerNext.Core.Icons.Labels;
using OpenTrackerNext.Core.Images;

namespace OpenTrackerNext.Core.Icons.States;

/// <summary>
/// Represents an icon state.
/// </summary>
public interface IIconState : IDisposable
{
    /// <summary>
    /// Gets an <see cref="IIconLabel"/> representing the icon label.
    /// </summary>
    IIconLabel Label { get; }
    
    /// <summary>
    /// Gets an <see cref="IImageFile"/> representing the icon image.
    /// </summary>
    IReadOnlyImageFile Image { get; }
}