namespace OpenTrackerNext.Core.Equality;

/// <summary>
/// Represents data that can be made value equal to another object.
/// </summary>
public interface IMakeValueEqual
{
    /// <summary>
    /// Makes this object value equal to another object.
    /// </summary>
    /// <param name="other">
    /// An <see cref="object"/> to which this object will be made value equal.
    /// </param>
    /// <returns>
    /// A <see cref="bool"/> representing whether the object was made value equal.
    /// </returns>
    bool MakeValueEqualTo(object other);
}