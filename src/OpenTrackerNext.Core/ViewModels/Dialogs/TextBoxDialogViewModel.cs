using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using OpenTrackerNext.Core.Results;
using OpenTrackerNext.Core.Validation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;

namespace OpenTrackerNext.Core.ViewModels.Dialogs;

/// <summary>
/// Represents a text box dialog view model.
/// </summary>
public sealed class TextBoxDialogViewModel : ViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TextBoxDialogViewModel"/> class.
    /// </summary>
    /// <param name="icon">
    /// A <see cref="DialogIcon"/> representing the icon.
    /// </param>
    /// <param name="validationRules">
    /// A <see cref="List{T}"/> of <see cref="ValidationRule{T}"/> of <see cref="string"/> representing the validation
    /// rules for the input text.
    /// </param>
    public TextBoxDialogViewModel(DialogIcon? icon = null, List<ValidationRule<string?>>? validationRules = null)
    {
        validationRules ??= [];

        IconIsVisible = icon is not null;
        IconValue = icon?.IconValue;

        this.WhenActivated(
            disposables =>
            {
                foreach (var validationRule in validationRules)
                {
                    this.ValidationRule(
                            vm => vm.InputText,
                            validationRule.Rule,
                            validationRule.FailureMessage)
                        .DisposeWith(disposables);
                }

                OkCommand = ReactiveCommand
                    .CreateFromTask(() => CloseAsync(OperationResult.Success), this.IsValid())
                    .DisposeWith(disposables);
                CancelCommand = ReactiveCommand
                    .CreateFromTask(() => CloseAsync(OperationResult.Cancel))
                    .DisposeWith(disposables);

                Disposable
                    .Create(
                        () =>
                        {
                            OkCommand = null;
                            CancelCommand = null;
                        })
                    .DisposeWith(disposables);
            });
    }

    /// <summary>
    /// Gets a <see cref="double"/> representing the maximum width of the dialog.
    /// </summary>
    public double MaxWidth { get; init; } = 480.0;
    
    /// <summary>
    /// Gets a <see cref="double"/> representing the maximum width of the dialog.
    /// </summary>
    public double MinWidth { get; init; } = 410.0;
    
    /// <summary>
    /// Gets a value indicating whether a <see cref="bool"/> representing whether the icon is visible.
    /// </summary>
    public bool IconIsVisible { get; }
    
    /// <summary>
    /// Gets a <see cref="string"/> representing the icon value.
    /// </summary>
    public string? IconValue { get; }
    
    /// <summary>
    /// Gets a <see cref="string"/> representing the header text.
    /// </summary>
    public required string Title { get; init; }
    
    /// <summary>
    /// Gets a <see cref="string"/> representing the content text.
    /// </summary>
    public required string Message { get; init; }
    
    /// <summary>
    /// Gets or sets a <see cref="string"/> representing the input text.
    /// </summary>
    [Reactive]
    public string InputText { get; set; } = string.Empty;

    /// <summary>
    /// Gets an <see cref="Interaction{TInput,TOutput}"/> with input of <see cref="OperationResult"/> used to close the
    /// dialog.
    /// </summary>
    public Interaction<OperationResult, Unit> CloseInteraction { get; } = new(RxApp.MainThreadScheduler);

    /// <summary>
    /// Gets a <see cref="ReactiveCommand{TParam,TResult}"/> representing closing the dialog by clicking OK.
    /// </summary>
    [Reactive]
    public ReactiveCommand<Unit, Unit>? OkCommand { get; private set; }
    
    /// <summary>
    /// Gets a <see cref="ReactiveCommand{TParam,TResult}"/> representing closing the dialog by clicking Cancel.
    /// </summary>
    [Reactive]
    public ReactiveCommand<Unit, Unit>? CancelCommand { get; private set; }
    
    private async Task CloseAsync(OperationResult result)
    {
        await CloseInteraction.Handle(result);
    }
}