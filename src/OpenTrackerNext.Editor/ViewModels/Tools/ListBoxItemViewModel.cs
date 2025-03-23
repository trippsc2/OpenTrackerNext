using System;
using System.Collections.Generic;
using System.Reactive;
using OpenTrackerNext.Core.ViewModels;
using OpenTrackerNext.Core.ViewModels.Menus;
using ReactiveUI;

namespace OpenTrackerNext.Editor.ViewModels.Tools;

/// <summary>
/// Represents a list box item control view model.
/// </summary>
public sealed class ListBoxItemViewModel : ViewModel, IListBoxItemViewModel
{
    /// <inheritdoc/>
    public required string Text { get; init; }

    /// <inheritdoc/>
    public IEnumerable<MenuItemViewModel> ContextMenuItems { get; init; } = Array.Empty<MenuItemViewModel>();

    /// <inheritdoc/>
    public required ReactiveCommand<Unit, Unit> DoubleTapCommand { get; init; }

    /// <inheritdoc/>
    public override Type GetViewType()
    {
        return typeof(IViewFor<IListBoxItemViewModel>);
    }
}