using System.Collections.ObjectModel;
using System.Reactive;
using ReactiveUI;

namespace OpenTrackerNext.Editor.ViewModels.Tools;

/// <summary>
/// Represents a folder tool control view model.
/// </summary>
public interface IFolderToolViewModel : IActivatableViewModel, IReactiveObject
{
    /// <summary>
    /// Gets a <see cref="ReadOnlyObservableCollection{T}"/> of <see cref="IListBoxItemViewModel"/> representing the
    /// list box items.
    /// </summary>
    ReadOnlyObservableCollection<IListBoxItemViewModel> ListBoxItems { get; }
    
    /// <summary>
    /// Gets or sets a <see cref="IListBoxItemViewModel"/> representing the selected list box item.
    /// </summary>
    IListBoxItemViewModel? SelectedListBoxItem { get; set; }

    /// <summary>
    /// Gets a <see cref="ReactiveCommand{TParam,TResult}"/> representing the command to add a new file.
    /// </summary>
    ReactiveCommand<Unit, Unit> AddCommand { get; }
    
    /// <summary>
    /// Gets a <see cref="ReactiveCommand{TParam,TResult}"/> representing the command to open the selected file.
    /// </summary>
    ReactiveCommand<Unit, Unit> OpenCommand { get; }
    
    /// <summary>
    /// Gets a <see cref="ReactiveCommand{TParam,TResult}"/> representing the command to rename the selected file.
    /// </summary>
    ReactiveCommand<Unit, Unit> RenameCommand { get; }
    
    /// <summary>
    /// Gets a <see cref="ReactiveCommand{TParam,TResult}"/> representing the command to delete the selected file.
    /// </summary>
    ReactiveCommand<Unit, Unit> DeleteCommand { get; }
}