using System.Reactive.Disposables;
using System.Reactive.Linq;
using OpenTrackerNext.Core.Entities;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.Requirements.Entity.AtMost;

/// <summary>
/// Represents a requirement that the specified entity must have at most the specified value.
/// </summary>
public sealed class EntityAtMostRequirement : ReactiveObject, IRequirement
{
    private readonly CompositeDisposable _disposables = new();
    private readonly IEntity _entity;

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityAtMostRequirement"/> class.
    /// </summary>
    /// <param name="entity">
    /// An <see cref="IEntity"/> representing the entity to be required.
    /// </param>
    /// <param name="requiredValue">
    /// An <see cref="int"/> representing the required entity value.
    /// </param>
    public EntityAtMostRequirement(IEntity entity, int requiredValue)
    {
        _entity = entity;
        
        this.WhenAnyValue(x => x._entity.Current)
            .Select(current => current <= requiredValue)
            .ToPropertyEx(this, x => x.IsMet)
            .DisposeWith(_disposables);
    }
    
    /// <inheritdoc/>
    [ObservableAsProperty]
    public bool IsMet { get; }

    /// <inheritdoc/>
    public void Dispose()
    {
        _disposables.Dispose();
    }
}