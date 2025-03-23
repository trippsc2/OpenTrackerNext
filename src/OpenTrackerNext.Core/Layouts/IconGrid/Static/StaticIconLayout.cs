using OpenTrackerNext.Core.Icons;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.Layouts.IconGrid.Static;

/// <summary>
/// Represents a static <see cref="IIconLayout"/> object.
/// </summary>
public sealed class StaticIconLayout : ReactiveObject, IIconLayout
{
    /// <inheritdoc/>
    [Reactive]
    public int Height { get; set; }
    
    /// <inheritdoc/>
    [Reactive]
    public int Width { get; set; }

    /// <inheritdoc/>
    public required IIcon CurrentIcon { get; init; }

    /// <inheritdoc/>
    public void Dispose()
    {
    }
}