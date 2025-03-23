using System;
using Avalonia.Controls;
using OpenTrackerNext.Editor.Tools;

namespace OpenTrackerNext.Editor.Controls;

/// <summary>
/// Represents a drop target for a tool.
/// </summary>
public sealed class HitBoxControl : Panel
{
    /// <summary>
    /// Gets or sets a <see cref="ToolBarPosition"/> representing the position to place a tool dropped in this hit box.
    /// </summary>
    public ToolBarPosition Position { get; set; } = ToolBarPosition.TopLeft;

    /// <inheritdoc/>
    protected override Type StyleKeyOverride => typeof(Panel);
}