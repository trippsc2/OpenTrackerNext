using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using OpenTrackerNext.Core.Requirements;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.Layouts.Dynamic;

/// <summary>
/// Represents a dynamic <see cref="ILayout"/> object.
/// </summary>
public sealed class DynamicLayout : ReactiveObject, ILayout
{
    private readonly CompositeDisposable _disposables = new();
    private readonly IRequirement _requirement;

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicLayout"/> class.
    /// </summary>
    /// <param name="requirement">
    /// A <see cref="IRequirement"/> representing the requirement for the layout.
    /// </param>
    /// <param name="metLayout">
    /// A <see cref="ILayout"/> representing the layout to display when the requirement is met.
    /// </param>
    /// <param name="unmetLayout">
    /// A <see cref="ILayout"/> representing the layout to display when the requirement is not met.
    /// </param>
    public DynamicLayout(IRequirement requirement, ILayout metLayout, ILayout unmetLayout)
    {
        _requirement = requirement.DisposeWith(_disposables);
        CurrentLayout = unmetLayout;
        
        this.WhenAnyValue(x => x._requirement.IsMet)
            .Select(isMet => isMet ? metLayout : unmetLayout)
            .ToPropertyEx(this, x => x.CurrentLayout)
            .DisposeWith(_disposables);
    }

    /// <inheritdoc/>
    public Thickness Margin => default;
    
    /// <summary>
    /// Gets a <see cref="ILayout"/> representing the layout to display.
    /// </summary>
    [ObservableAsProperty]
    public ILayout CurrentLayout { get; }

    /// <inheritdoc/>
    public void Dispose()
    {
        _disposables.Dispose();
    }
}