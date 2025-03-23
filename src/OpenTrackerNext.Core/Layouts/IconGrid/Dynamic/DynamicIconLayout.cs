using System.Reactive.Disposables;
using System.Reactive.Linq;
using OpenTrackerNext.Core.Icons;
using OpenTrackerNext.Core.Requirements;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.Layouts.IconGrid.Dynamic;

/// <summary>
/// Represents a dynamic <see cref="IIconLayout"/> object.
/// </summary>
public sealed class DynamicIconLayout : ReactiveObject, IIconLayout
{
    private readonly CompositeDisposable _disposables = new();
    private readonly IRequirement _requirement;

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicIconLayout"/> class.
    /// </summary>
    /// <param name="requirement">
    /// A <see cref="IRequirement"/> representing the requirement for the icon.
    /// </param>
    /// <param name="metIcon">
    /// An <see cref="IIcon"/> representing the icon to display when the requirement is met.
    /// </param>
    /// <param name="unmetIcon">
    /// An <see cref="IIcon"/> representing the icon to display when the requirement is not met.
    /// </param>
    public DynamicIconLayout(IRequirement requirement, IIcon metIcon, IIcon unmetIcon)
    {
        _requirement = requirement.DisposeWith(_disposables);
        CurrentIcon = unmetIcon;
        
        this.WhenAnyValue(x => x._requirement.IsMet)
            .Select(isMet => isMet ? metIcon : unmetIcon)
            .ToPropertyEx(this, x => x.CurrentIcon)
            .DisposeWith(_disposables);
    }

    /// <inheritdoc/>
    [Reactive]
    public int Height { get; set; }
    
    /// <inheritdoc/>
    [Reactive]
    public int Width { get; set; }

    /// <inheritdoc/>
    [ObservableAsProperty]
    public IIcon CurrentIcon { get; }

    /// <inheritdoc/>
    public void Dispose()
    {
        _disposables.Dispose();
    }
}