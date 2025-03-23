using System;

namespace OpenTrackerNext.Core.Ids;

/// <summary>
/// Extends <see cref="Guid"/> with additional methods.
/// </summary>
public static class GuidExtensions
{
    /// <summary>
    /// Returns a lowercase string representation of the <see cref="Guid"/>.
    /// </summary>
    /// <param name="guid">
    /// The <see cref="Guid"/> to be converted.
    /// </param>
    /// <returns>
    /// A <see cref="string"/> representing the <see cref="Guid"/> in lowercase format.
    /// </returns>
    public static string ToLowercaseString(this Guid guid)
    {
        return guid.ToString().ToLowerInvariant();
    }
}