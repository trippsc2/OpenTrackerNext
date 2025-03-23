using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive;
using OpenTrackerNext.Core.ViewModels.Menus;
using ReactiveUI;

namespace OpenTrackerNext.Editor.ViewModels.Tools;

/// <summary>
/// Represents a list box item control view model.
/// </summary>
public interface IListBoxItemViewModel : IActivatableViewModel, INotifyPropertyChanged
{
    /// <summary>
    /// Gets a <see cref="string"/> representing the list box item text.
    /// </summary>
    string Text { get; }
    
    /// <summary>
    /// Gets an <see cref="ObservableCollection{T}"/> of <see cref="MenuItemViewModel"/> representing the context menu
    /// items of this list box item.
    /// </summary>
    IEnumerable<MenuItemViewModel> ContextMenuItems { get; }

    /// <summary>
    /// Gets a <see cref="ReactiveCommand{TParam,TResult}"/> to execute when double tapping the list box item.
    /// </summary>
    ReactiveCommand<Unit, Unit> DoubleTapCommand { get; }
}