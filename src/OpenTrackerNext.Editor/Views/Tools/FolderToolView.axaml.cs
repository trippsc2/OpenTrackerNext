using System.Reactive.Disposables;
using Avalonia.ReactiveUI;
using OpenTrackerNext.Editor.ViewModels.Tools;
using ReactiveUI;
using ViewLocator = OpenTrackerNext.Core.DataTemplates.ViewLocator;

namespace OpenTrackerNext.Editor.Views.Tools;

/// <summary>
/// Represents the folder tool control.
/// </summary>
public sealed partial class FolderToolView : ReactiveUserControl<IFolderToolViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FolderToolView"/> class.
    /// </summary>
    public FolderToolView()
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
                        vm => vm.OpenCommand,
                        v => v.OpenButton)
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