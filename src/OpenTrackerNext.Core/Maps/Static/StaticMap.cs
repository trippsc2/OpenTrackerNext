using OpenTrackerNext.Core.Images;
using ReactiveUI;

namespace OpenTrackerNext.Core.Maps.Static;

/// <inheritdoc cref="IMap" />
public sealed class StaticMap : ReactiveObject, IMap
{
    /// <inheritdoc/>
    public required IReadOnlyImageFile Image { get; init; }

    /// <inheritdoc/>
    public void Dispose()
    {
    }
}