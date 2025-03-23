using System.Reactive.Disposables;
using System.Reactive.Linq;
using OpenTrackerNext.Core.Entities;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.Requirements.Entity.Exact;

/// <summary>
/// Represents a requirement that the specified entity must have the exact specified value.
/// </summary>
public sealed class EntityExactRequirement : ReactiveObject, IRequirement
{
    private readonly CompositeDisposable _disposables = new();
    private readonly IEntity _entity;

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityExactRequirement"/> class.
    /// </summary>
    /// <param name="entity">
    /// An <see cref="IEntity"/> representing the entity to be required.
    /// </param>
    /// <param name="requiredValue">
    /// An <see cref="int"/> representing the required entity value.
    /// </param>
    public EntityExactRequirement(IEntity entity, int requiredValue)
    {
        _entity = entity;

        this.WhenAnyValue(x => x._entity.Current)
            .Select(current => current == requiredValue)
            .ToPropertyEx(this, x => x.IsMet)
            .DisposeWith(_disposables);
    }

    /// <inheritdoc/>
    [ObservableAsProperty]
    public bool IsMet { get; }

    /// <inheritdoc/>
    public void Dispose()
    {
    }
}