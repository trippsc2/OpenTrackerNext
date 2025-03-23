namespace OpenTrackerNext.Core.Utils;

/// <summary>
/// Represents a singleton interface.
/// </summary>
/// <typeparam name="TSelf">
/// The type of the singleton.
/// </typeparam>
public interface ISingleton<out TSelf>
    where TSelf : ISingleton<TSelf>
{
    /// <summary>
    /// Gets a singleton <typeparamref name="TSelf"/> instance.
    /// </summary>
    static abstract TSelf Instance { get; }
}