using System.Reactive.Disposables;
using System.Reactive.Linq;
using OpenTrackerNext.Core.Settings;
using OpenTrackerNext.SplatRegistration;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.Requirements.UIPanel;

/// <summary>
/// Represents a requirement that the UI pane direction is a specified value or set of values.
/// </summary>
[Splat(RegisterAsType = typeof(Factory))]
public sealed class UIPanelDockRequirement : ReactiveObject, IRequirement
{
    private readonly CompositeDisposable _disposables = new();
    private readonly LayoutSettings _layoutSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="UIPanelDockRequirement"/> class.
    /// </summary>
    /// <param name="layoutSettings">
    /// The <see cref="LayoutSettings"/>.
    /// </param>
    /// <param name="direction">
    /// A <see cref="UIPanelDock"/> representing the required direction.
    /// </param>
    public UIPanelDockRequirement(LayoutSettings layoutSettings, UIPanelDock direction)
    {
        _layoutSettings = layoutSettings;

        this.WhenAnyValue(x => x._layoutSettings.UIPanelDock)
            .Select(direction.IsMatchingDock)
            .ToPropertyEx(this, x => x.IsMet)
            .DisposeWith(_disposables);
    }
    
    /// <summary>
    /// A factory method for creating new <see cref="UIPanelDockRequirement"/> objects.
    /// </summary>
    /// <param name="direction">
    /// A <see cref="UIPanelDock"/> representing the required direction.
    /// </param>
    /// <returns>
    /// A new <see cref="UIPanelDockRequirement"/> object.
    /// </returns>
    public delegate UIPanelDockRequirement Factory(UIPanelDock direction);
    
    /// <inheritdoc/>
    [ObservableAsProperty]
    public bool IsMet { get; }

    /// <inheritdoc/>
    public void Dispose()
    {
        _disposables.Dispose();
    }
}