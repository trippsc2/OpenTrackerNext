using System;

namespace OpenTrackerNext.Core.Exceptions;

/// <summary>
/// Represents an exception thrown when the storage folder fails to provide a bookmark ID.
/// </summary>
public sealed class StorageProviderFailureException : ApplicationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StorageProviderFailureException"/> class.
    /// </summary>
    public StorageProviderFailureException()
        : base("The storage folder failed to provide a bookmark ID.")
    {
    }
}