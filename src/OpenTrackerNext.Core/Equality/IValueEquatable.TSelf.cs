namespace OpenTrackerNext.Core.Equality;

/// <summary>
/// Represents a class that can be compared for value equality.
/// </summary>
/// <typeparam name="TSelf">
/// The type of the class that implements this interface.
/// </typeparam>
public interface IValueEquatable<in TSelf> : IValueEquatable
    where TSelf : IValueEquatable<TSelf>
{
    /// <summary>
    /// Returns whether the object is value equal to the specified object.
    /// </summary>
    /// <param name="other">
    /// A <typeparamref name="TSelf"/> representing the object to compare with this object.
    /// </param>
    /// <returns>
    /// A <see cref="bool"/> representing whether the object is value equal to the specified object.
    /// </returns>
    bool ValueEquals(TSelf other);
}