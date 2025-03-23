using Ardalis.SmartEnum;
using Avalonia.Layout;
using Avalonia.Media;

namespace OpenTrackerNext.Editor.Tools;

/// <summary>
/// Represents in which toolbar position a tool is located.
/// </summary>
public sealed class ToolBarPosition : SmartEnum<ToolBarPosition>
{
    /// <summary>
    /// A <see cref="ToolBarPosition"/> representing the top left toolbar position.
    /// </summary>
    public static readonly ToolBarPosition TopLeft;
    
    /// <summary>
    /// A <see cref="ToolBarPosition"/> representing the top right toolbar position.
    /// </summary>
    public static readonly ToolBarPosition TopRight;
    
    /// <summary>
    /// A <see cref="ToolBarPosition"/> representing the bottom left toolbar position.
    /// </summary>
    public static readonly ToolBarPosition BottomLeft;
    
    /// <summary>
    /// A <see cref="ToolBarPosition"/> representing the bottom right toolbar position.
    /// </summary>
    public static readonly ToolBarPosition BottomRight;

    /// <summary>
    /// Initializes static members of the <see cref="ToolBarPosition"/> class.
    /// </summary>
    static ToolBarPosition()
    {
        TopLeft = new ToolBarPosition(nameof(TopLeft), 0)
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            FlowDirection = FlowDirection.LeftToRight,
            ButtonAngle = -90.0
        };
        TopRight = new ToolBarPosition(nameof(TopRight), 1)
        {
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Top,
            FlowDirection = FlowDirection.LeftToRight,
            ButtonAngle = 90.0
        };
        BottomLeft = new ToolBarPosition(nameof(BottomLeft), 2)
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Bottom,
            FlowDirection = FlowDirection.RightToLeft,
            ButtonAngle = -90.0
        };
        BottomRight = new ToolBarPosition(nameof(BottomRight), 3)
        {
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Bottom,
            FlowDirection = FlowDirection.RightToLeft,
            ButtonAngle = 90.0
        };
    }

    private ToolBarPosition(string name, int value)
        : base(name, value)
    {
    }
    
    /// <summary>
    /// Gets a <see cref="HorizontalAlignment"/> representing the horizontal alignment of the toolbar for this position.
    /// </summary>
    public required HorizontalAlignment HorizontalAlignment { get; init; }
    
    /// <summary>
    /// Gets a <see cref="VerticalAlignment"/> representing the vertical alignment of the toolbar for this position.
    /// </summary>
    public required VerticalAlignment VerticalAlignment { get; init; }
    
    /// <summary>
    /// Gets a <see cref="FlowDirection"/> representing the direction in which the toolbar buttons will populate.
    /// RightToLeft will reverse the order of the buttons.
    /// </summary>
    public required FlowDirection FlowDirection { get; init; }
    
    /// <summary>
    /// Gets a <see cref="double"/> representing the angle of the toolbar button.
    /// </summary>
    public required double ButtonAngle { get; init; }
}