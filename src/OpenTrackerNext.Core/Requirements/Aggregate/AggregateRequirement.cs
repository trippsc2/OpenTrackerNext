using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.Requirements.Aggregate;

/// <summary>
/// Represents a requirement that is a combination of other requirements.
/// </summary>
public sealed class AggregateRequirement : ReactiveObject, IRequirement
{
    private readonly CompositeDisposable _disposables = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRequirement"/> class.
    /// </summary>
    /// <param name="requirements">
    /// An <see cref="ObservableCollection{T}"/> of <see cref="IRequirement"/> to be combined.
    /// </param>
    public AggregateRequirement(ObservableCollection<IRequirement> requirements)
    {
        requirements
            .ToObservableChangeSet()
            .WhenPropertyChanged(x => x.IsMet)
            .Select(_ => requirements.All(x => x.IsMet))
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