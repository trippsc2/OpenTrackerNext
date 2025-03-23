using System.Reactive.Disposables;
using Avalonia.ReactiveUI;
using OpenTrackerNext.Editor.ViewModels.Tools;
using ReactiveUI;
using ViewLocator = OpenTrackerNext.Core.DataTemplates.ViewLocator;

namespace OpenTrackerNext.Editor.Views.Tools;

/// <summary>
/// Represents the images tool control.
/// </summary>
public sealed partial class ImagesToolView : ReactiveUserControl<ImagesToolViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ImagesToolView"/> class.
    /// </summary>
    public ImagesToolView()
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
                        vm => vm.AddCommand,
                        v => v.AddButton)
                    .DisposeWith(disposables);

                this.BindCommand(
                        ViewModel,
                        vm => vm.RenameCommand,
                        v => v.RenameButton)
                    .DisposeWith(disposables);

                this.BindCommand(
                        ViewModel,
                        vm => vm.DeleteCommand,
                        v => v.DeleteButton)
                    .DisposeWith(disposables);
            });
    }
}