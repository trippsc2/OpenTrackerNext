using System.Collections;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.ReactiveUI;
using OpenTrackerNext.Core.ViewModels.Menus;
using OpenTrackerNext.Editor.ViewModels.Tools;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;

namespace OpenTrackerNext.Editor.Views.Tools;

/// <summary>
/// Represents the list box item view.
/// </summary>
public sealed partial class ListBoxItemView : ReactiveUserControl<IListBoxItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ListBoxItemView"/> class.
    /// </summary>
    public ListBoxItemView()
    {
        InitializeComponent();

        this.WhenActivated(
            disposables =>
            {
                this.OneWayBind<IListBoxItemViewModel, ListBoxItemView, string, object?>(
                        ViewModel,
                        vm => vm.Text,
                        v => v.Label.Content)
                    .DisposeWith(disposables);

                this.OneWayBind<IListBoxItemViewModel, ListBoxItemView, IEnumerable<MenuItemViewModel>, IEnumerable?>(
                        ViewModel,
                        vm => vm.ContextMenuItems,
                        v => v.RightClickContextMenu.ItemsSource)
                    .DisposeWith(disposables);

                if (ViewModel is null)
                {
                    return;
                }

                this.Events()
                    .DoubleTapped
                    .Select(_ => Unit.Default)
                    .InvokeCommand(ViewModel.DoubleTapCommand)
                    .DisposeWith(disposables);
            });
    }
}