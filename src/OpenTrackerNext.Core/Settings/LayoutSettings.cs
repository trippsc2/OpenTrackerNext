using Avalonia.Controls;
using OpenTrackerNext.SplatRegistration;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.Settings;

/// <summary>
/// Represents the layout settings.
/// </summary>
[Splat]
[SplatSingleInstance]
public sealed class LayoutSettings : ReactiveObject
{
    /// <summary>
    /// Gets or sets a <see cref="Dock"/> representing the UI panel dock.
    /// </summary>
    [Reactive]
    public Dock UIPanelDock { get; set; } = Dock.Bottom;
}