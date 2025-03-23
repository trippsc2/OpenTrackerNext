using System.Collections.Generic;
using Avalonia;
using ReactiveUI;

namespace OpenTrackerNext.Core.Layouts.UniformGrid;

/// <summary>
/// Represents a uniform grid layout.
/// </summary>
public sealed class UniformGridLayout : ReactiveObject, ILayout
{
    /// <inheritdoc/>
    public required Thickness Margin { get; init; }

    /// <summary>
    /// Gets an <see cref="int"/> representing the number of columns in the grid.
    /// </summary>
    public required int Columns { get; init; }
    
    /// <summary>
    /// Gets an <see cref="int"/> representing the number of rows in the grid.
    /// </summary>
    public required int Rows { get; init; }
    
    /// <summary>
    /// Gets a <see cref="IReadOnlyList{T}"/> of <see cref="ILayout"/> representing the items in the grid.
    /// </summary>
    public required IReadOnlyList<ILayout> Items { get; init; }

    /// <inheritdoc/>
    public void Dispose()
    {
    }
}