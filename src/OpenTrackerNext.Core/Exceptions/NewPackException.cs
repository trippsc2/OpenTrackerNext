using System;

namespace OpenTrackerNext.Core.Exceptions;

/// <summary>
/// Represents an exception thrown when a new pack cannot be created.
/// </summary>
public sealed class NewPackException : ApplicationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NewPackException"/> class.
    /// </summary>
    /// <param name="message">
    ///     A <see cref="string"/> representing the exception message.
    /// </param>
    public NewPackException(string message)
        : base(message)
    {
    }
}