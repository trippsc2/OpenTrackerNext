namespace OpenTrackerNext.Core.Equality;

/// <summary>
/// Represents data that can be made value equal to another object.
/// </summary>
/// <typeparam name="TSelf">
/// The type to which this object can be made value equal.
/// </typeparam>
public interface IMakeValueEqual<in TSelf> : IMakeValueEqual
    where TSelf : IMakeValueEqual<TSelf>, new()
{
    /// <summary>
    /// Makes this object value equal to the specified object.
    /// </summary>
    /// <param name="other">
    /// A <typeparamref name="TSelf"/> to which this object will be made value equal.
    /// </param>
    void MakeValueEqualTo(TSelf other);
}