using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.Entities;

/// <inheritdoc cref="IEntity"/>
public sealed class Entity : ReactiveObject, IEntity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Entity"/> class.
    /// </summary>
    /// <param name="minimum">
    /// An <see cref="int"/> representing the minimum value of the entity.
    /// </param>
    /// <param name="starting">
    /// An <see cref="int"/> representing the starting value of the entity.
    /// </param>
    /// <param name="maximum">
    /// An <see cref="int"/> representing the maximum value of the entity.
    /// </param>
    public Entity(int minimum, int starting, int maximum)
    {
        Minimum = minimum;
        Starting = starting;
        Current = starting;
        Maximum = maximum;
    }
    
    /// <inheritdoc/>
    public int Minimum { get; }
    
    /// <inheritdoc/>
    public int Starting { get; }

    /// <inheritdoc/>
    public int Maximum { get; }

    /// <inheritdoc/>
    [Reactive]
    public int Current { get; set; }

    /// <inheritdoc/>
    public void Reset()
    {
        Current = Starting;
    }
}