using OpenTrackerNext.Core.ViewModels;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Editor.Tools;

/// <summary>
/// Represents a tool.
/// </summary>
public sealed class Tool : ReactiveObject
{
    /// <summary>
    /// Gets a <see cref="ToolId"/> representing the tool ID.
    /// </summary>
    public required ToolId Id { get; init; }
    
    /// <summary>
    /// Gets a <see cref="string"/> representing the tool title.
    /// </summary>
    public required string Title { get; init; }
    
    /// <summary>
    /// Gets a <see cref="ViewModel"/> representing the tool content view model.
    /// </summary>
    public required ViewModel Content { get; init; }
    
    /// <summary>
    /// Gets a <see cref="string"/> representing the tool header text.
    /// </summary>
    public string? ToolTipHeader { get; init; }
    
    /// <summary>
    /// Gets a <see cref="string"/> representing the tool content text.
    /// </summary>
    public string? ToolTipContent { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the tool is active.
    /// </summary>
    [Reactive]
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Gets or sets a <see cref="ToolBarPosition"/> representing the tool position.
    /// </summary>
    [Reactive]
    public required ToolBarPosition Position { get; set; }
}