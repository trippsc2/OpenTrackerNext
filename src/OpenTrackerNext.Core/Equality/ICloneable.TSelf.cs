using System;

namespace OpenTrackerNext.Core.Equality;

/// <summary>
/// Represents an object that can be cloned.
/// </summary>
/// <typeparam name="TSelf">
/// The type of the object that implements this interface.
/// </typeparam>
public interface ICloneable<out TSelf> : ICloneable
    where TSelf : ICloneable<TSelf>
{
    /// <summary>
    /// Returns a new <typeparamref name="TSelf"/> instance that is value equal to this instance. 
    /// </summary>
    /// <returns>
    /// A new <typeparamref name="TSelf"/> instance that is value equal to this instance.
    /// </returns>
    new TSelf Clone();
}