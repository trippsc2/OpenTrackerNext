using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Layout;
using Avalonia.Media;
using DynamicData;
using OpenTrackerNext.Core.ViewModels;
using OpenTrackerNext.Editor.Tools;
using OpenTrackerNext.SplatRegistration;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Editor.ViewModels.Tools;

/// <summary>
/// Represents a tool bar control view model.
/// </summary>
[Splat(RegisterAsType = typeof(Factory))]
public sealed class ToolBarViewModel : ViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ToolBarViewModel"/> class.
    /// </summary>
    /// <param name="toolService">
    /// The <see cref="IToolService"/>.
    /// </param>
    /// <param name="toolButtonFactory">
    /// An Autofac factory for creating new <see cref="ToolButtonViewModel"/> objects.
    /// </param>
    /// <param name="position">
    /// A <see cref="ToolBarPosition"/> representing the position of the tool bar.
    /// </param>
    public ToolBarViewModel(
        IToolService toolService,
        ToolButtonViewModel.Factory toolButtonFactory,
        ToolBarPosition position)
    {
        HorizontalAlignment = position.HorizontalAlignment;
        VerticalAlignment = position.VerticalAlignment;
        FlowDirection = position.FlowDirection;

        this.WhenActivated(
            disposables =>
            {
                toolService
                    .Connect()
                    .AutoRefresh(x => x.Position)
                    .Filter(x => x.Position == position)
                    .Transform(x => toolButtonFactory(x))
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(out var buttons)
                    .Subscribe()
                    .DisposeWith(disposables);

                Buttons = buttons;
            });
    }

    /// <summary>
    /// A factory method for creating new <see cref="ToolBarViewModel"/> objects for the specified position.
    /// </summary>
    /// <param name="position">
    /// A <see cref="ToolBarPosition"/> representing the position of the tool bar.
    /// </param>
    /// <returns>
    /// A new <see cref="ToolBarViewModel"/> object.
    /// </returns>
    public delegate ToolBarViewModel Factory(ToolBarPosition position);

    /// <summary>
    /// Gets a <see cref="HorizontalAlignment"/> representing the horizontal alignment of the tool bar.
    /// </summary>
    public HorizontalAlignment HorizontalAlignment { get; }
    
    /// <summary>
    /// Gets a <see cref="VerticalAlignment"/> representing the vertical alignment of the tool bar.
    /// </summary>
    public VerticalAlignment VerticalAlignment { get; }
    
    /// <summary>
    /// Gets a <see cref="FlowDirection"/> representing the flow direction of the tool bar.
    /// </summary>
    /// <remarks>
    /// <see cref="FlowDirection.LeftToRight"/> will place tool buttons from top to bottom.
    /// <see cref="FlowDirection.RightToLeft"/> will place tool buttons from bottom to top.
    /// </remarks>
    public FlowDirection FlowDirection { get; }

    /// <summary>
    /// Gets a <see cref="ReadOnlyObservableCollection{T}"/> of <see cref="ToolButtonViewModel"/> representing the tool
    /// buttons on the tool bar.
    /// </summary>
    [Reactive]
    public ReadOnlyObservableCollection<ToolButtonViewModel> Buttons { get; private set; } = null!;
}