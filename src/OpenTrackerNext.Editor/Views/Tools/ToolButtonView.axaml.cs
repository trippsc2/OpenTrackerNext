using System.Reactive.Disposables;
using Avalonia.ReactiveUI;
using OpenTrackerNext.Editor.ViewModels.Tools;
using ReactiveUI;

namespace OpenTrackerNext.Editor.Views.Tools;

/// <summary>
/// Represents the tool button control.
/// </summary>
public sealed partial class ToolButtonView : ReactiveUserControl<ToolButtonViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ToolButtonView"/> class.
    /// </summary>
    public ToolButtonView()
    {
        InitializeComponent();

        this.WhenActivated(
            disposables =>
            {
                this.OneWayBind(
                        ViewModel,
                        vm => vm.Content,
                        v => v.ToggleButton.Content)
                    .DisposeWith(disposables);
                this.OneWayBind(
                        ViewModel,
                        vm => vm.IsActive,
                        v => v.ToggleButton.IsChecked)
                    .DisposeWith(disposables);
                this.BindCommand(
                        ViewModel,
                        vm => vm.ToggleActivationCommand,
                        v => v.ToggleButton)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        ViewModel,
                        vm => vm.ToolTipHeader,
                        v => v.ToolTipHeaderLabel.Content)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        ViewModel,
                        vm => vm.ToolTipContent,
                        v => v.ToolTipContentLabel.Content)
                    .DisposeWith(disposables);
            });
    }
}