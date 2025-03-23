using System.Reactive.Disposables;
using Avalonia.ReactiveUI;
using OpenTrackerNext.Editor.ViewModels.Tools;
using ReactiveUI;

namespace OpenTrackerNext.Editor.Views.Tools;

/// <summary>
/// Represents the tool bar control.
/// </summary>
public sealed partial class ToolBarView : ReactiveUserControl<ToolBarViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ToolBarView"/> class.
    /// </summary>
    public ToolBarView()
    {
        InitializeComponent();

        this.WhenActivated(
            disposables =>
            {
                this.Bind(
                        ViewModel,
                        vm => vm.HorizontalAlignment,
                        v => v.HorizontalAlignment)
                    .DisposeWith(disposables);
                this.Bind(
                        ViewModel,
                        vm => vm.VerticalAlignment,
                        v => v.VerticalAlignment)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        ViewModel,
                        vm => vm.Buttons,
                        v => v.ItemsControl.ItemsSource)
                    .DisposeWith(disposables);
            });
    }
}