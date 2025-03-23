using System.Reactive.Disposables;
using System.Reactive.Linq;
using OpenTrackerNext.Core.Entities;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.Requirements.Entity.Not;

/// <summary>
/// Represents a requirement that the value of the specified entity is anything but the specified value.
/// </summary>
public sealed class EntityNotRequirement : ReactiveObject, IRequirement
{
    private readonly CompositeDisposable _disposables = new();
    private readonly IEntity _entity;

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityNotRequirement"/> class.
    /// </summary>
    /// <param name="entity">
    /// An <see cref="IEntity"/> representing the entity to be required.
    /// </param>
    /// <param name="excludedValue">
    /// An <see cref="int"/> representing the excluded entity value.
    /// </param>
    public EntityNotRequirement(IEntity entity, int excludedValue)
    {
        _entity = entity;

        this.WhenAnyValue(x => x._entity.Current)
            .Select(current => current != excludedValue)
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