using Ardalis.SmartEnum;

namespace OpenTrackerNext.Editor.Tools;

/// <summary>
/// Represents the tool pane side.
/// </summary>
public sealed class ToolPaneSide : SmartEnum<ToolPaneSide>
{
    /// <summary>
    /// A <see cref="ToolPaneSide"/> representing the left tool pane side.
    /// </summary>
    public static readonly ToolPaneSide Left;
    
    /// <summary>
    /// A <see cref="ToolPaneSide"/> representing the right tool pane side.
    /// </summary>
    public static readonly ToolPaneSide Right;

    /// <summary>
    /// Initializes static members of the <see cref="ToolPaneSide"/> class.
    /// </summary>
    static ToolPaneSide()
    {
        Left = new ToolPaneSide(nameof(Left), 0)
        {
            Top = ToolBarPosition.TopLeft,
            Bottom = ToolBarPosition.BottomLeft
        };
        Right = new ToolPaneSide(nameof(Right), 1)
        {
            Top = ToolBarPosition.TopRight,
            Bottom = ToolBarPosition.BottomRight
        };
    }

    private ToolPaneSide(string name, int value)
        : base(name, value)
    {
    }

    /// <summary>
    /// Gets a <see cref="ToolBarPosition"/> representing the top toolbar position.
    /// </summary>
    public required ToolBarPosition Top { get; init; }
    
    /// <summary>
    /// Gets a <see cref="ToolBarPosition"/> representing the bottom toolbar position.
    /// </summary>
    public required ToolBarPosition Bottom { get; init; }
}