using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using OpenTrackerNext.Core.Results;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.ViewModels.Dialogs;

/// <summary>
/// Represents a yes/no dialog view model.
/// </summary>
public sealed class YesNoDialogViewModel : ViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="YesNoDialogViewModel"/> class.
    /// </summary>
    /// <param name="icon">
    /// The <see cref="DialogIcon"/> representing the icon.
    /// </param>
    public YesNoDialogViewModel(DialogIcon? icon)
    {
        IconIsVisible = icon is not null;
        IconValue = icon?.IconValue;

        this.WhenActivated(
            disposables =>
            {
                YesCommand = ReactiveCommand
                    .CreateFromTask(() => CloseAsync(YesNoDialogResult.Yes))
                    .DisposeWith(disposables);
                NoCommand = ReactiveCommand
                    .CreateFromTask(() => CloseAsync(YesNoDialogResult.No))
                    .DisposeWith(disposables);

                Disposable
                    .Create(
                        () =>
                        {
                            YesCommand = null;
                            NoCommand = null;
                        })
                    .DisposeWith(disposables);
            });
    }

    /// <summary>
    /// Gets a <see cref="double"/> representing the maximum width of the dialog box.
    /// </summary>
    public double MaxWidth { get; init; } = 480.0;
    
    /// <summary>
    /// Gets a value indicating whether the icon is visible.
    /// </summary>
    public bool IconIsVisible { get; }
    
    /// <summary>
    /// Gets a <see cref="string"/> representing the icon value.
    /// </summary>
    public string? IconValue { get; }
    
    /// <summary>
    /// Gets a <see cref="string"/> representing the title text.
    /// </summary>
    public required string Title { get; init; }
    
    /// <summary>
    /// Gets a <see cref="string"/> representing the message text.
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// Gets an <see cref="Interaction{TInput,TOutput}"/> with an input of <see cref="YesNoDialogResult"/> used to close
    /// the dialog.
    /// </summary>
    public Interaction<YesNoDialogResult, Unit> CloseInteraction { get; } = new(RxApp.MainThreadScheduler);

    /// <summary>
    /// Gets a <see cref="ReactiveCommand{TParam,TResult}"/> that closes the dialog with a result of <see cref="Yes"/>.
    /// </summary>
    [Reactive]
    public ReactiveCommand<Unit, Unit>? YesCommand { get; private set; }
    
    /// <summary>
    /// Gets a <see cref="ReactiveCommand{TParam,TResult}"/> that closes the dialog with a result of <see cref="No"/>.
    /// </summary>
    [Reactive]
    public ReactiveCommand<Unit, Unit>? NoCommand { get; private set; }
    
    private async Task CloseAsync(YesNoDialogResult result)
    {
        await CloseInteraction.Handle(result);
    }
}