using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.ViewModels.Dialogs;

/// <summary>
/// Represents an exception dialog view model.
/// </summary>
public sealed class ExceptionDialogViewModel : ViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionDialogViewModel"/> class.
    /// </summary>
    /// <param name="exception">
    /// An <see cref="Exception"/> that is to be represented by the dialog box.
    /// </param>
    /// <param name="title">
    /// A <see cref="string"/> representing the dialog title. Null will use the default "Exception" title.
    /// </param>
    public ExceptionDialogViewModel(Exception exception, string? title = null)
    {
        Title = title ?? "Exception";
        Message = exception.Message;
        StackTrace = exception.StackTrace;
        StackTraceIsVisible = exception.StackTrace is not null;

        this.WhenActivated(
            disposables =>
            {
                CloseCommand = ReactiveCommand
                    .CreateFromTask(CloseAsync)
                    .DisposeWith(disposables);

                Disposable
                    .Create(() => CloseCommand = null)
                    .DisposeWith(disposables);
            });
    }

    /// <summary>
    /// Gets a <see cref="string"/> representing the dialog title.
    /// </summary>
    public string Title { get; }
    
    /// <summary>
    /// Gets a <see cref="string"/> representing the exception message.
    /// </summary>
    public string? Message { get; }
    
    /// <summary>
    /// Gets a <see cref="string"/> representing the exception stack trace.
    /// </summary>
    public string? StackTrace { get; }
    
    /// <summary>
    /// Gets a value indicating whether the stack trace is visible.
    /// </summary>
    public bool StackTraceIsVisible { get; }

    /// <summary>
    /// Gets an <see cref="Interaction{TInput,TOutput}"/> that closes the dialog.
    /// </summary>
    public Interaction<Unit, Unit> CloseInteraction { get; } = new(RxApp.MainThreadScheduler);

    /// <summary>
    /// Gets a <see cref="ReactiveCommand{TParam,TResult}"/> that closes the dialog.
    /// </summary>
    [Reactive]
    public ReactiveCommand<Unit, Unit>? CloseCommand { get; private set; }
    
    private async Task CloseAsync()
    {
        await CloseInteraction.Handle(Unit.Default);
    }
}