using System;
using System.Collections.Generic;
using System.Reactive;
using System.Windows.Input;
using Avalonia.Input;
using ReactiveUI;

namespace OpenTrackerNext.Core.ViewModels.Menus;

/// <summary>
/// Represents the menu item control view model.
/// </summary>
public sealed class MenuItemViewModel : ViewModel
{
    /// <summary>
    /// Gets a <see cref="string"/> representing the icon ID.
    /// </summary>
    public string? Icon { get; init; }
    
    /// <summary>
    /// Gets a <see cref="string"/> representing the header text.
    /// </summary>
    public required string Header { get; init; }
    
    /// <summary>
    /// Gets a <see cref="string"/> representing the input gesture text.
    /// </summary>
    public KeyGesture? InputGestureText { get; init; }
    
    /// <summary>
    /// Gets a <see cref="ICommand"/> representing the command to execute for this menu item.
    /// </summary>
    public ReactiveCommand<Unit, Unit>? Command { get; init; }

    /// <summary>
    /// Gets an <see cref="IEnumerable{T}"/> of <see cref="MenuItemViewModel"/> representing the children of
    /// this menu item.
    /// </summary>
    public IEnumerable<MenuItemViewModel> Children { get; init; } = Array.Empty<MenuItemViewModel>();
}