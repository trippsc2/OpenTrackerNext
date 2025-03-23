using System;
using OpenTrackerNext.Core.Utils;
using ReactiveUI;

namespace OpenTrackerNext.Core.Entities;

/// <summary>
/// Represents a null <see cref="IEntity"/> object.
/// </summary>
public sealed class NullEntity : ReactiveObject, IEntity, INullObject<NullEntity>
{
    private NullEntity()
    {
    }

    /// <inheritdoc />
    public static NullEntity Instance { get; } = new();

    /// <inheritdoc/>
    public int Minimum => 0;

    /// <inheritdoc/>
    public int Starting => 0;

    /// <inheritdoc/>
    public int Maximum => 0;

    /// <inheritdoc/>
    public int Current
    {
        get => 0;
        set => throw new NotSupportedException("NullEntity is immutable.");
    }

    /// <inheritdoc/>
    public void Reset()
    {
    }
}