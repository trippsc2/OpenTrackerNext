namespace OpenTrackerNext.Core.Equality;

/// <summary>
/// Represents a class that can be compared for value equality.
/// </summary>
public interface IValueEquatable
{
    /// <summary>
    /// Returns a value indicating whether this instance is equal to a specified object.
    /// </summary>
    /// <param name="obj">
    /// An <see cref="object"/> to compare with this instance.
    /// </param>
    /// <returns>
    /// A <see cref="bool"/> representing whether this instance is equal to the specified object.
    /// </returns>
    bool ValueEquals(object? obj);
}