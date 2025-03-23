using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.Requirements.Alternative;

/// <summary>
/// Represents a requirement that can be fulfilled by one of multiple requirements.
/// </summary>
public sealed class AlternativeRequirement : ReactiveObject, IRequirement
{
    private readonly CompositeDisposable _disposables = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="AlternativeRequirement"/> class.
    /// </summary>
    /// <param name="requirements">
    /// An <see cref="ObservableCollection{T}"/> of <see cref="IRequirement"/> to be combined.
    /// </param>
    public AlternativeRequirement(ObservableCollection<IRequirement> requirements)
    {
        requirements
            .ToObservableChangeSet()
            .WhenPropertyChanged(x => x.IsMet)
            .Select(_ => requirements.Any(x => x.IsMet))
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