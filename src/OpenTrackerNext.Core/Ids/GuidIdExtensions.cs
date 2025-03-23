namespace OpenTrackerNext.Core.Ids;

/// <summary>
/// Extends <see cref="IGuidId"/> or <see cref="IGuidId{TSelf}"/> with additional methods.
/// </summary>
public static class GuidIdExtensions
{
    /// <summary>
    /// Returns a lowercase string representation of the <see cref="IGuidId"/>.
    /// </summary>
    /// <param name="guidId">
    /// The <see cref="IGuidId"/> to be converted.
    /// </param>
    /// <returns>
    /// A <see cref="string"/> representing the <see cref="IGuidId"/> in lowercase format.
    /// </returns>
    public static string ToLowercaseString(this IGuidId guidId)
    {
        return guidId.Value.ToLowercaseString();
    }
}