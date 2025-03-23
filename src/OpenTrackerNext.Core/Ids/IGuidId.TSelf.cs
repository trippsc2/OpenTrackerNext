using System;

namespace OpenTrackerNext.Core.Ids;

/// <summary>
/// Represents a strongly typed ID based on a GUID.
/// </summary>
/// <typeparam name="TSelf">
/// The type of the derived class.
/// </typeparam>
public interface IGuidId<out TSelf> : IGuidId
    where TSelf : IGuidId<TSelf>, new()
{
    /// <summary>
    /// Creates a new <see cref="TSelf"/> object with a random GUID.
    /// </summary>
    /// <returns>
    /// A new <see cref="TSelf"/> object.
    /// </returns>
    static abstract TSelf New();

    /// <summary>
    /// Creates a new <see cref="TSelf"/> object with the specified GUID string.
    /// </summary>
    /// <param name="value">
    /// A <see cref="string"/> to be parsed into a GUID.
    /// </param>
    /// <returns>
    /// A new <see cref="TSelf"/> object.
    /// </returns>
    /// <exception cref="FormatException">
    /// Thrown when the string is not formatted correctly.
    /// </exception>
    static abstract TSelf Parse(string value);
}