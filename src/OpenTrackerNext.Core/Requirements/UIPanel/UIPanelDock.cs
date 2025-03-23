using System.Linq;
using System.Text.Json.Serialization;
using Ardalis.SmartEnum;
using Ardalis.SmartEnum.SystemTextJson;
using Avalonia.Controls;
using OpenTrackerNext.Core.Utils;

namespace OpenTrackerNext.Core.Requirements.UIPanel;

/// <summary>
/// Represents the direction of UI panes.
/// </summary>
[JsonConverter(typeof(SmartEnumNameConverter<UIPanelDock, int>))]
public sealed class UIPanelDock : SmartEnum<UIPanelDock>, INamedSmartEnum
{
    /// <summary>
    /// A <see cref="UIPanelDock"/> representing UI panes on the left.
    /// </summary>
    public static readonly UIPanelDock Left;

    /// <summary>
    /// A <see cref="UIPanelDock"/> representing UI panes on the bottom.
    /// </summary>
    public static readonly UIPanelDock Bottom;

    /// <summary>
    /// A <see cref="UIPanelDock"/> representing UI panes on the right.
    /// </summary>
    public static readonly UIPanelDock Right;
    
    /// <summary>
    /// A <see cref="UIPanelDock"/> representing UI panes on the top.
    /// </summary>
    public static readonly UIPanelDock Top;

    /// <summary>
    /// A <see cref="UIPanelDock"/> representing UI panes on the left or right.
    /// </summary>
    public static readonly UIPanelDock LeftOrRight;
    
    /// <summary>
    /// A <see cref="UIPanelDock"/> representing UI panes on the top or bottom.
    /// </summary>
    public static readonly UIPanelDock TopOrBottom;
    
    private readonly Dock[] _matchingDocks;

    static UIPanelDock()
    {
        Left = new UIPanelDock("Left", 0, Dock.Left);
        Bottom = new UIPanelDock("Bottom", 1, Dock.Bottom);
        Right = new UIPanelDock("Right", 2, Dock.Right);
        Top = new UIPanelDock("Top", 3, Dock.Top);
        
        LeftOrRight = new UIPanelDock("LeftOrRight", 4, Dock.Left, Dock.Right)
        {
            DisplayName = "Left or Right"
        };
        
        TopOrBottom = new UIPanelDock("TopOrBottom", 5, Dock.Top, Dock.Bottom)
        {
            DisplayName = "Top or Bottom"
        };
    }
    
    private UIPanelDock(string name, int value, params Dock[] matchingDocks)
        : base(name, value)
    {
        _matchingDocks = matchingDocks;
        DisplayName = name;
    }

    /// <inheritdoc/>
    public string DisplayName { get; private init; }

    /// <summary>
    /// Returns whether the specified <see cref="Dock"/> matches this <see cref="UIPanelDock"/>.
    /// </summary>
    /// <param name="dock">
    /// The <see cref="Dock"/> to be checked.
    /// </param>
    /// <returns>
    /// A <see cref="bool"/> representing whether the specified <see cref="Dock"/> matches this
    /// <see cref="UIPanelDock"/>.
    /// </returns>
    public bool IsMatchingDock(Dock dock)
    {
        return _matchingDocks.Contains(dock);
    }
}