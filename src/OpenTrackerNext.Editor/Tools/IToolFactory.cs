using System.Collections.Generic;

namespace OpenTrackerNext.Editor.Tools;

/// <summary>
/// Represents the creation logic for tools.
/// </summary>
public interface IToolFactory
{
    /// <summary>
    /// Returns a newly created list of tools.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of <see cref="Tool"/> representing the list of tools.
    /// </returns>
    IEnumerable<Tool> CreateTools();
}