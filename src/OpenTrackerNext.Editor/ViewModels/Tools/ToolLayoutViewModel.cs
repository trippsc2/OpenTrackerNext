using System.Reactive.Disposables;
using System.Reactive.Linq;
using OpenTrackerNext.Core.ViewModels;
using OpenTrackerNext.Editor.Tools;
using OpenTrackerNext.Editor.ViewModels.Documents;
using OpenTrackerNext.SplatRegistration;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Editor.ViewModels.Tools;

/// <summary>
/// Represents the tool layout control view model.
/// </summary>
[Splat]
public sealed class ToolLayoutViewModel : ViewModel
{
    private readonly IToolService _toolService;
    private readonly ReactiveProperty<Tool?> _activeTopLeftTool;
    private readonly ReactiveProperty<Tool?> _activeTopRightTool;
    private readonly ReactiveProperty<Tool?> _activeBottomLeftTool;
    private readonly ReactiveProperty<Tool?> _activeBottomRightTool;

    /// <summary>
    /// Initializes a new instance of the <see cref="ToolLayoutViewModel"/> class.
    /// </summary>
    /// <param name="toolService">
    /// The <see cref="IToolService"/>.
    /// </param>
    /// <param name="centerContent">
    /// The <see cref="DocumentLayoutViewModel"/>.
    /// </param>
    /// <param name="toolBarFactory">
    /// An Autofac factory for creating new <see cref="ToolBarViewModel"/> objects.
    /// </param>
    /// <param name="toolPaneFactory">
    /// An Autofac factory for creating new <see cref="ToolPaneViewModel"/> objects.
    /// </param>
    public ToolLayoutViewModel(
        IToolService toolService,
        DocumentLayoutViewModel centerContent,
        ToolBarViewModel.Factory toolBarFactory,
        ToolPaneViewModel.Factory toolPaneFactory)
    {
        _toolService = toolService;

        _activeTopLeftTool = toolService.ActiveTools[ToolBarPosition.TopLeft];
        _activeTopRightTool = toolService.ActiveTools[ToolBarPosition.TopRight];
        _activeBottomLeftTool = toolService.ActiveTools[ToolBarPosition.BottomLeft];
        _activeBottomRightTool = toolService.ActiveTools[ToolBarPosition.BottomRight];

        TopLeftToolBar = toolBarFactory(ToolBarPosition.TopLeft);
        TopRightToolBar = toolBarFactory(ToolBarPosition.TopRight);
        BottomLeftToolBar = toolBarFactory(ToolBarPosition.BottomLeft);
        BottomRightToolBar = toolBarFactory(ToolBarPosition.BottomRight);

        LeftToolPane = toolPaneFactory(ToolPaneSide.Left);
        RightToolPane = toolPaneFactory(ToolPaneSide.Right);

        CenterContent = centerContent;

        this.WhenActivated(
            disposables =>
            {
                this.WhenAnyValue(
                        x => x._activeTopLeftTool.Value,
                        x => x._activeBottomLeftTool.Value,
                        (topLeft, bottomLeft) => topLeft is not null || bottomLeft is not null)
                    .ToPropertyEx(
                        this,
                        x => x.LeftToolPaneIsVisible,
                        scheduler: RxApp.MainThreadScheduler)
                    .DisposeWith(disposables);
                this.WhenAnyValue(
                        x => x._activeTopRightTool.Value,
                        x => x._activeBottomRightTool.Value,
                        (topRight, bottomRight) => topRight is not null || bottomRight is not null)
                    .ToPropertyEx(
                        this,
                        x => x.RightToolPaneIsVisible,
                        scheduler: RxApp.MainThreadScheduler)
                    .DisposeWith(disposables);

                this.WhenAnyValue(
                        x => x._activeTopLeftTool.Value,
                        x => x._activeBottomLeftTool.Value,
                        (topLeft, bottomLeft) => topLeft is not null || bottomLeft is not null)
                    .Select(x => x ? 3 : 1)
                    .ToPropertyEx(
                        this,
                        x => x.CenterColumn,
                        scheduler: RxApp.MainThreadScheduler)
                    .DisposeWith(disposables);
                this.WhenAnyValue(
                        x => x.LeftToolPaneIsVisible,
                        x => x.RightToolPaneIsVisible,
                        (left, right) => 5 - (left ? 2 : 0) - (right ? 2 : 0))
                    .ToPropertyEx(
                        this,
                        x => x.CenterColumnSpan,
                        scheduler: RxApp.MainThreadScheduler)
                    .DisposeWith(disposables);

                this.WhenAnyValue(x => x._toolService.DragHitBoxesAreActive)
                    .ToPropertyEx(
                        this,
                        x => x.DragHitBoxesAreActive,
                        scheduler: RxApp.MainThreadScheduler)
                    .DisposeWith(disposables);
            });
    }

    /// <summary>
    /// Gets a <see cref="ToolBarViewModel"/> representing the top left tool bar.
    /// </summary>
    public ToolBarViewModel TopLeftToolBar { get; }
    
    /// <summary>
    /// Gets a <see cref="ToolBarViewModel"/> representing the top right tool bar.
    /// </summary>
    public ToolBarViewModel TopRightToolBar { get; }
    
    /// <summary>
    /// Gets a <see cref="ToolBarViewModel"/> representing the bottom left tool bar.
    /// </summary>
    public ToolBarViewModel BottomLeftToolBar { get; }
    
    /// <summary>
    /// Gets a <see cref="ToolBarViewModel"/> representing the bottom right tool bar.
    /// </summary>
    public ToolBarViewModel BottomRightToolBar { get; }
    
    /// <summary>
    /// Gets a <see cref="ToolPaneViewModel"/> representing the left tool pane.
    /// </summary>
    public ToolPaneViewModel LeftToolPane { get; }
    
    /// <summary>
    /// Gets a <see cref="ToolPaneViewModel"/> representing the right tool pane.
    /// </summary>
    public ToolPaneViewModel RightToolPane { get; }
    
    /// <summary>
    /// Gets a <see cref="DocumentLayoutViewModel"/> representing the center document layout content.
    /// </summary>
    public DocumentLayoutViewModel CenterContent { get; }

    /// <summary>
    /// Gets a value indicating whether a <see cref="bool"/> representing whether the left tool pane is visible.
    /// </summary>
    [ObservableAsProperty]
    public bool LeftToolPaneIsVisible { get; }
    
    /// <summary>
    /// Gets a value indicating whether a <see cref="bool"/> representing whether the right tool pane is visible.
    /// </summary>
    [ObservableAsProperty]
    public bool RightToolPaneIsVisible { get; }
    
    /// <summary>
    /// Gets a <see cref="int"/> representing the column in which the center content is placed.
    /// </summary>
    [ObservableAsProperty]
    public int CenterColumn { get; }
    
    /// <summary>
    /// Gets a <see cref="int"/> representing the number of columns across which the center content will span.
    /// </summary>
    [ObservableAsProperty]
    public int CenterColumnSpan { get; }
    
    /// <summary>
    /// Gets a value indicating whether the drag and drop hit boxes are active.
    /// </summary>
    [ObservableAsProperty]
    public bool DragHitBoxesAreActive { get; }
}