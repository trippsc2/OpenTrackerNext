using System.Reactive.Disposables;
using Avalonia.ReactiveUI;
using OpenTrackerNext.Editor.ViewModels.Tools;
using ReactiveUI;
using ViewLocator = OpenTrackerNext.Core.DataTemplates.ViewLocator;

namespace OpenTrackerNext.Editor.Views.Tools;

/// <summary>
/// Represents the global tool control.
/// </summary>
public sealed partial class GlobalToolView : ReactiveUserControl<GlobalToolViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GlobalToolView"/> class.
    /// </summary>
    public GlobalToolView()
    {
        InitializeComponent();

        ListBox.ItemTemplate = new ViewLocator();

        this.WhenActivated(
            disposables =>
            {
                this.OneWayBind(
                        ViewModel,
                        vm => vm.ListBoxItems,
                        v => v.ListBox.ItemsSource)
                    .DisposeWith(disposables);
                this.Bind(
                        ViewModel,
                        vm => vm.SelectedListBoxItem,
                        v => v.ListBox.SelectedItem)
                    .DisposeWith(disposables);

                this.BindCommand(
                        ViewModel,
                        vm => vm.OpenCommand,
                        v => v.OpenButton)
                    .DisposeWith(disposables);
            });
    }
}