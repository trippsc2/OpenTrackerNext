using System;
using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using OpenTrackerNext.Editor.ViewModels.Tools;
using ReactiveUI;

namespace OpenTrackerNext.Editor.Views.Tools;

/// <summary>
/// Represents the tool pane control.
/// </summary>
public sealed partial class ToolPaneView : ReactiveUserControl<ToolPaneViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ToolPaneView"/> class.
    /// </summary>
    public ToolPaneView()
    {
        InitializeComponent();

        this.WhenActivated(
            disposables =>
            {
                this.OneWayBind(
                        ViewModel,
                        vm => vm.TopIsVisible,
                        v => v.TopToolContentControl.IsVisible)
                    .DisposeWith(disposables);
                this.OneWayBind(
                        ViewModel,
                        vm => vm.Top,
                        v => v.TopToolContentControl.Content)
                    .DisposeWith(disposables);
                this.WhenAnyValue(x => x.ViewModel!.TopRowSpan)
                    .Subscribe(x => Grid.SetRowSpan(TopToolContentControl, x))
                    .DisposeWith(disposables);

                this.OneWayBind(
                        ViewModel,
                        vm => vm.GridSplitterIsVisible,
                        v => v.GridSplitter.IsVisible)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        ViewModel,
                        vm => vm.BottomIsVisible,
                        v => v.BottomToolContentControl.IsVisible)
                    .DisposeWith(disposables);
                this.OneWayBind(
                        ViewModel,
                        vm => vm.Bottom,
                        v => v.BottomToolContentControl.Content)
                    .DisposeWith(disposables);
                this.WhenAnyValue(x => x.ViewModel!.BottomRow)
                    .Subscribe(x => Grid.SetRow(BottomToolContentControl, x))
                    .DisposeWith(disposables);
                this.WhenAnyValue(x => x.ViewModel!.BottomRowSpan)
                    .Subscribe(x => Grid.SetRowSpan(BottomToolContentControl, x))
                    .DisposeWith(disposables);
            });
    }
}