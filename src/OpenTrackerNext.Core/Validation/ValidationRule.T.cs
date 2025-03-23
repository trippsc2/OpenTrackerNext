using System;

namespace OpenTrackerNext.Core.Validation;

/// <summary>
/// Represents a validation rule.
/// </summary>
/// <typeparam name="T">
/// The type of the property to be validated.
/// </typeparam>
public sealed class ValidationRule<T>
{
    /// <summary>
    /// Gets a <see cref="Func{TResult}"/> with a <see cref="T"/> parameter and a <see cref="bool"/> output
    /// representing the logic for evaluating the rule.
    /// </summary>
    public required Func<T, bool> Rule { get; init; }
    
    /// <summary>
    /// Gets a <see cref="string"/> representing the message to display if the validation rule is violated.
    /// </summary>
    public required string FailureMessage { get; init; }
}