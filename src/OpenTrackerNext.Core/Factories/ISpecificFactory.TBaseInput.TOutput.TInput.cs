namespace OpenTrackerNext.Core.Factories;

/// <summary>
/// Represents the specific creation logic for <see cref="TOutput"/> objects created from <see cref="TInput"/> objects.
/// </summary>
/// <typeparam name="TBaseInput">
/// The type of the base input.
/// </typeparam>
/// <typeparam name="TOutput">
/// The type of the output.
/// </typeparam>
/// <typeparam name="TInput">
/// The type of the input.
/// </typeparam>
public interface ISpecificFactory<in TBaseInput, out TOutput, in TInput> : IFactory<TBaseInput, TOutput>
    where TBaseInput : notnull
    where TOutput : notnull
    where TInput : TBaseInput
{
    /// <summary>
    /// Returns a new <see cref="TOutput"/> object created from the specified <see cref="TInput"/> object.
    /// </summary>
    /// <param name="input">
    /// A <see cref="TInput"/> representing the input object.
    /// </param>
    /// <returns>
    /// A new <see cref="TOutput"/> object.
    /// </returns>
    TOutput Create(TInput input);
}