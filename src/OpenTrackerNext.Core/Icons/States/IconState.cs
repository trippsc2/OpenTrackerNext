using OpenTrackerNext.Core.Icons.Labels;
using OpenTrackerNext.Core.Images;
using ReactiveUI;

namespace OpenTrackerNext.Core.Icons.States;

/// <inheritdoc cref="IIconState" />
public sealed class IconState : ReactiveObject, IIconState
{
    /// <inheritdoc/>
    public required IIconLabel Label { get; init; }

    /// <inheritdoc/>
    public required IReadOnlyImageFile Image { get; init; }

    /// <inheritdoc/>
    public void Dispose()
    {
        Label.Dispose();
    }
}