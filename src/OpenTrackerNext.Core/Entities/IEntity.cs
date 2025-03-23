using System.ComponentModel;

namespace OpenTrackerNext.Core.Entities;

/// <summary>
/// Represents a data entity whose state affects the tracker.
/// </summary>
public interface IEntity : INotifyPropertyChanged
{
    /// <summary>
    /// Gets an <see cref="int"/> representing the minimum state value.
    /// </summary>
    int Minimum { get; }
    
    /// <summary>
    /// Gets an <see cref="int"/> representing the starting state value.
    /// </summary>
    int Starting { get; }
    
    /// <summary>
    /// Gets an <see cref="int"/> representing the maximum state value.
    /// </summary>
    int Maximum { get; }
    
    /// <summary>
    /// Gets or sets an <see cref="int"/> representing the current state value.
    /// </summary>
    int Current { get; set; }

    /// <summary>
    /// Resets the entity to its starting state.
    /// </summary>
    void Reset();
}