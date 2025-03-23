using System.Collections.Generic;
using Avalonia;
using ReactiveUI;

namespace OpenTrackerNext.Core.Layouts.IconGrid;

/// <summary>
/// Represents an icon grid layout.
/// </summary>
public sealed class IconGridLayout : ReactiveObject, ILayout
{
    /// <inheritdoc/>
    public required Thickness Margin { get; init; }
    
    /// <summary>
    /// Gets a <see cref="double"/> representing the horizontal spacing between icons.
    /// </summary>
    public required double HorizontalSpacing { get; init; }
    
    /// <summary>
    /// Gets a <see cref="double"/> representing the vertical spacing between icons.
    /// </summary>
    public required double VerticalSpacing { get; init; }

    /// <summary>
    /// Gets a <see cref="IReadOnlyList{T}"/> of <see cref="IReadOnlyList{T}"/> of <see cref="IIconLayout"/>
    /// representing the icons in the grid by row.
    /// </summary>
    public required IReadOnlyList<IReadOnlyList<IIconLayout>> Icons { get; init; }

    /// <inheritdoc/>
    public void Dispose()
    {
        foreach (var row in Icons)
        {
            foreach (var iconLayout in row)
            {
                iconLayout.Dispose();
            }
        }
    }
}