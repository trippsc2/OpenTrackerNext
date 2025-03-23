using System.Reactive.Disposables;
using System.Reactive.Linq;
using OpenTrackerNext.Core.Images;
using OpenTrackerNext.Core.Requirements;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.Maps.Dynamic;

/// <summary>
/// Represents a map that changes based on a <see cref="IRequirement"/>.
/// </summary>
public sealed class DynamicMap : ReactiveObject, IMap
{
    private readonly CompositeDisposable _disposables = new();
    private readonly IRequirement _requirement;
    private readonly IReadOnlyImageFile _metImage;
    private readonly IReadOnlyImageFile _unmetImage;

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicMap"/> class.
    /// </summary>
    /// <param name="requirement">
    /// A <see cref="IRequirement"/> representing the requirement to be checked.
    /// </param>
    /// <param name="metImage">
    /// A <see cref="IReadOnlyImageFile"/> representing the map image when the requirement is met.
    /// </param>
    /// <param name="unmetImage">
    /// A <see cref="IReadOnlyImageFile"/> representing the map image when the requirement is not met.
    /// </param>
    public DynamicMap(IRequirement requirement, IReadOnlyImageFile metImage, IReadOnlyImageFile unmetImage)
    {
        _requirement = requirement.DisposeWith(_disposables);
        _metImage = metImage;
        _unmetImage = unmetImage;
        Image = unmetImage;
        
        this.WhenAnyValue(x => x._requirement.IsMet)
            .Select(GetImage)
            .ToPropertyEx(this, x => x.Image)
            .DisposeWith(_disposables);
    }
    
    /// <inheritdoc/>
    [ObservableAsProperty]
    public IReadOnlyImageFile Image { get; }

    /// <inheritdoc/>
    public void Dispose()
    {
        _disposables.Dispose();
    }

    private IReadOnlyImageFile GetImage(bool isMet)
    {
        return isMet ? _metImage : _unmetImage;
    }
}