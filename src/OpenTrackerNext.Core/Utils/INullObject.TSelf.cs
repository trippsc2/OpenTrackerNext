namespace OpenTrackerNext.Core.Utils;

/// <summary>
/// Represents a null object.
/// </summary>
/// <typeparam name="TSelf">
/// The type of the null object.
/// </typeparam>
public interface INullObject<out TSelf> : INullObject, ISingleton<TSelf>
    where TSelf : ISingleton<TSelf>;