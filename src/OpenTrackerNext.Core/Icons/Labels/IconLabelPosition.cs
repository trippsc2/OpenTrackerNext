using System.Text.Json.Serialization;
using Ardalis.SmartEnum;
using Ardalis.SmartEnum.SystemTextJson;
using Avalonia.Layout;
using OpenTrackerNext.Core.Utils;

namespace OpenTrackerNext.Core.Icons.Labels;

/// <summary>
/// Represents the position of an icon label.
/// </summary>
[JsonConverter(typeof(SmartEnumNameConverter<IconLabelPosition, int>))]
public sealed class IconLabelPosition : SmartEnum<IconLabelPosition>, INamedSmartEnum
{
    /// <summary>
    /// A <see cref="IconLabelPosition"/> representing the top left label position.
    /// </summary>
    public static readonly IconLabelPosition TopLeft;
    
    /// <summary>
    /// A <see cref="IconLabelPosition"/> representing the top center label position.
    /// </summary>
    public static readonly IconLabelPosition TopCenter;
    
    /// <summary>
    /// A <see cref="IconLabelPosition"/> representing the top right label position.
    /// </summary>
    public static readonly IconLabelPosition TopRight;
    
    /// <summary>
    /// A <see cref="IconLabelPosition"/> representing the center left label position.
    /// </summary>
    public static readonly IconLabelPosition CenterLeft;
    
    /// <summary>
    /// A <see cref="IconLabelPosition"/> representing the center label position.
    /// </summary>
    public static readonly IconLabelPosition Center;
    
    /// <summary>
    /// A <see cref="IconLabelPosition"/> representing the center right label position.
    /// </summary>
    public static readonly IconLabelPosition CenterRight;
    
    /// <summary>
    /// A <see cref="IconLabelPosition"/> representing the bottom left label position.
    /// </summary>
    public static readonly IconLabelPosition BottomLeft;
    
    /// <summary>
    /// A <see cref="IconLabelPosition"/> representing the bottom center label position.
    /// </summary>
    public static readonly IconLabelPosition BottomCenter;
    
    /// <summary>
    /// A <see cref="IconLabelPosition"/> representing the bottom right label position.
    /// </summary>
    public static readonly IconLabelPosition BottomRight;

    /// <summary>
    /// Initializes static members of the <see cref="IconLabelPosition"/> class.
    /// </summary>
    static IconLabelPosition()
    {
        TopLeft = new IconLabelPosition(nameof(TopLeft), 0)
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            DisplayName = "Top Left"
        };
        TopCenter = new IconLabelPosition(nameof(TopCenter), 1)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Top,
            DisplayName = "Top Center"
        };
        TopRight = new IconLabelPosition(nameof(TopRight), 2)
        {
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Top,
            DisplayName = "Top Right"
        };
        CenterLeft = new IconLabelPosition(nameof(CenterLeft), 3)
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center,
            DisplayName = "Center Left"
        };
        Center = new IconLabelPosition(nameof(Center), 4)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            DisplayName = "Center"
        };
        CenterRight = new IconLabelPosition(nameof(CenterRight), 5)
        {
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center,
            DisplayName = "Center Right"
        };
        BottomLeft = new IconLabelPosition(nameof(BottomLeft), 6)
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Bottom,
            DisplayName = "Bottom Left"
        };
        BottomCenter = new IconLabelPosition(nameof(BottomCenter), 7)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Bottom,
            DisplayName = "Bottom Center"
        };
        BottomRight = new IconLabelPosition(nameof(BottomRight), 8)
        {
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Bottom,
            DisplayName = "Bottom Right"
        };
    }

    private IconLabelPosition(string name, int value)
        : base(name, value)
    {
    }

    /// <summary>
    /// Gets a <see cref="string"/> representing the display name of the label position.
    /// </summary>
    public required string DisplayName { get; init; }
    
    /// <summary>
    /// Gets a <see cref="HorizontalAlignment"/> representing the horizontal alignment of the label.
    /// </summary>
    public required HorizontalAlignment HorizontalAlignment { get; init; }
    
    /// <summary>
    /// Gets a <see cref="VerticalAlignment"/> representing the vertical alignment of the label.
    /// </summary>
    public required VerticalAlignment VerticalAlignment { get; init; }
}