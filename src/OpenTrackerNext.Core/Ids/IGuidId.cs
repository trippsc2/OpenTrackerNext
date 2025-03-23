using System;

namespace OpenTrackerNext.Core.Ids;

/// <summary>
/// Represents a strongly typed ID based on a GUID.
/// </summary>
public interface IGuidId
{
    /// <summary>
    /// Gets a <see cref="string"/> representing the folder name related to this ID.
    /// </summary>
    static abstract string FolderName { get; }
    
    /// <summary>
    /// Gets a <see cref="Guid"/> representing the ID.
    /// </summary>
    Guid Value { get; }
}