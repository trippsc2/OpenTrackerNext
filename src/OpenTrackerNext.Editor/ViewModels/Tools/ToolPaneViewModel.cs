using System.Reactive.Disposables;
using System.Reactive.Linq;
using OpenTrackerNext.Core.ViewModels;
using OpenTrackerNext.Editor.Tools;
using OpenTrackerNext.SplatRegistration;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Editor.ViewModels.Tools;

/// <summary>
/// Represents a tool pane view model.
/// </summary>
[Splat(RegisterAsType = typeof(Factory))]
public sealed class ToolPaneViewModel : ViewModel
{
    private readonly ReactiveProperty<Tool?> _activeTopTool;
    private readonly ReactiveProperty<Tool?> _activeBottomTool;

    /// <summary>
    /// Initializes a new instance of the <see cref="ToolPaneViewModel"/> class.
    /// </summary>
    /// <param name="toolService">
    /// The <see cref="IToolService"/>.
    /// </param>
    /// <param name="toolFactory">
    /// An Autofac factory for creating new <see cref="ToolViewModel"/> objects.
    /// </param>
    /// <param name="side">
    /// A <see cref="ToolPaneSide"/> representing the side of the tool pane.
    /// </param>
    public ToolPaneViewModel(IToolService toolService, ToolViewModel.Factory toolFactory, ToolPaneSide side)
    {
        var topPosition = side.Top;
        var bottomPosition = side.Bottom;

        _activeTopTool = toolService.ActiveTools[topPosition];
        _activeBottomTool = toolService.ActiveTools[bottomPosition];

        this.WhenActivated(
            disposables =>
            {
                this.WhenAnyValue(x => x._activeTopTool.Value)
                    .Select(x => x is not null)
                    .ToPropertyEx(
                        this,
                        x => x.TopIsVisible,
                        scheduler: RxApp.MainThreadScheduler)
                    .DisposeWith(disposables);
                this.WhenAnyValue(x => x._activeBottomTool.Value)
                    .Select(x => x is not null)
                    .ToPropertyEx(
                        this,
                        x => x.BottomIsVisible,
                        scheduler: RxApp.MainThreadScheduler)
                    .DisposeWith(disposables);
                this.WhenAnyValue(
                        x => x._activeTopTool.Value,
                        x => x._activeBottomTool.Value,
                        (top, bottom) => top is not null && bottom is not null)
                    .ToPropertyEx(
                        this,
                        x => x.GridSplitterIsVisible,
                        scheduler: RxApp.MainThreadScheduler)
                    .DisposeWith(disposables);

                this.WhenAnyValue(x => x._activeBottomTool.Value)
                    .Select(x => x is null ? 3 : 1)
                    .ToPropertyEx(
                        this,
                        x => x.TopRowSpan,
                        scheduler: RxApp.MainThreadScheduler)
                    .DisposeWith(disposables);
                this.WhenAnyValue(x => x._activeTopTool.Value)
                    .Select(x => x is null ? 0 : 2)
                    .ToPropertyEx(
                        this,
                        x => x.BottomRow,
                        scheduler: RxApp.MainThreadScheduler)
                    .DisposeWith(disposables);
                this.WhenAnyValue(x => x._activeTopTool.Value)
                    .Select(x => x is null ? 3 : 1)
                    .ToPropertyEx(
                        this,
                        x => x.BottomRowSpan,
                        scheduler: RxApp.MainThreadScheduler)
                    .DisposeWith(disposables);

                this.WhenAnyValue(x => x._activeTopTool.Value)
                    .Select(x => x is not null ? toolFactory(x) : null)
                    .ToPropertyEx(
                        this,
                        x => x.Top,
                        scheduler: RxApp.MainThreadScheduler)
                    .DisposeWith(disposables);
                this.WhenAnyValue(x => x._activeBottomTool.Value)
                    .Select(x => x is not null ? toolFactory(x) : null)
                    .ToPropertyEx(
                        this,
                        x => x.Bottom,
                        scheduler: RxApp.MainThreadScheduler)
                    .DisposeWith(disposables);
            });
    }

    /// <summary>
    /// A factory method for creating new <see cref="ToolPaneViewModel"/> objects for the specified side.
    /// </summary>
    /// <param name="side">
    /// A <see cref="ToolPaneSide"/> representing the side of the tool pane.
    /// </param>
    /// <returns>
    /// A new <see cref="ToolPaneViewModel"/> object.
    /// </returns>
    public delegate ToolPaneViewModel Factory(ToolPaneSide side);

    /// <summary>
    /// Gets a value indicating whether the top tool pane is visible.
    /// </summary>
    [ObservableAsProperty]
    public bool TopIsVisible { get; }
    
    /// <summary>
    /// Gets a value indicating whether the bottom tool pane is visible.
    /// </summary>
    [ObservableAsProperty]
    public bool BottomIsVisible { get; }
    
    /// <summary>
    /// Gets a value indicating whether the grid splitter is visible.
    /// </summary>
    [ObservableAsProperty]
    public bool GridSplitterIsVisible { get; }
    
    /// <summary>
    /// Gets a <see cref="int"/> representing the row in which the top tool pane is located.
    /// </summary>
    [ObservableAsProperty]
    public int TopRowSpan { get; }
    
    /// <summary>
    /// Gets a <see cref="int"/> representing the row in which the bottom tool pane is located.
    /// </summary>
    [ObservableAsProperty]
    public int BottomRow { get; }
    
    /// <summary>
    /// Gets a <see cref="int"/> representing the number of rows across which the bottom tool pane will span.
    /// </summary>
    [ObservableAsProperty]
    public int BottomRowSpan { get; }
    
    /// <summary>
    /// Gets a <see cref="ToolViewModel"/> representing the top tool view model.
    /// </summary>
    [ObservableAsProperty]
    public ToolViewModel? Top { get; }
    
    /// <summary>
    /// Gets a <see cref="ToolViewModel"/> representing the bottom tool view model.
    /// </summary>
    [ObservableAsProperty]
    public ToolViewModel? Bottom { get; }
}