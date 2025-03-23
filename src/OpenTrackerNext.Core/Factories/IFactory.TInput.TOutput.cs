namespace OpenTrackerNext.Core.Factories;

/// <summary>
/// Represents a factory.
/// </summary>
/// <typeparam name="TInput">
/// The type of the input.
/// </typeparam>
/// <typeparam name="TOutput">
/// The type of the output.
/// </typeparam>
public interface IFactory<in TInput, out TOutput>
    where TInput : notnull
    where TOutput : notnull
{
    /// <summary>
    /// Creates a new <typeparamref name="TOutput"/> object.
    /// </summary>
    /// <param name="input">
    /// A <see cref="TInput"/> representing the input.
    /// </param>
    /// <returns>
    /// A new <typeparamref name="TOutput"/> object.
    /// </returns>
    TOutput Create(TInput input);
}