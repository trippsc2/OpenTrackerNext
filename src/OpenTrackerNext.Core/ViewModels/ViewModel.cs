using System;
using ReactiveUI;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;

namespace OpenTrackerNext.Core.ViewModels;

/// <summary>
/// Base class for view models.
/// </summary>
public abstract class ViewModel : ReactiveObject, IActivatableViewModel, IValidatableViewModel
{
    /// <inheritdoc/>
    public ViewModelActivator Activator { get; } = new();
    
    /// <inheritdoc/>
    public IValidationContext ValidationContext { get; } = new ValidationContext();

    /// <summary>
    /// Returns the type of the view for this view model.
    /// </summary>
    /// <returns>
    /// The <see cref="Type"/> of the view.
    /// </returns>
    public virtual Type GetViewType()
    {
        return typeof(IViewFor<>).MakeGenericType(GetType());
    }
}