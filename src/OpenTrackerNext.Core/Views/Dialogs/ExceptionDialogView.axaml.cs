using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;
using DialogHostAvalonia;
using OpenTrackerNext.Core.ViewModels.Dialogs;
using ReactiveUI;

namespace OpenTrackerNext.Core.Views.Dialogs;

/// <summary>
/// Represents an exception dialog.
/// </summary>
public sealed partial class ExceptionDialogView : ReactiveUserControl<ExceptionDialogViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionDialogView"/> class.
    /// </summary>
    public ExceptionDialogView()
    {
        InitializeComponent();

        this.WhenActivated(
            disposables =>
            {
                this.OneWayBind(
                        ViewModel,
                        vm => vm.Title,
                        v => v.TitleTextBox.Text)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        ViewModel,
                        vm => vm.Message,
                        v => v.MessageTextBox.Text)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        ViewModel,
                        vm => vm.StackTrace,
                        v => v.StackTraceTextBox.Text)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        ViewModel,
                        vm => vm.StackTraceIsVisible,
                        v => v.StackTraceExpander.IsVisible)
                    .DisposeWith(disposables);

                this.BindCommand(
                        ViewModel,
                        vm => vm.CloseCommand,
                        v => v.OkButton)
                    .DisposeWith(disposables);

                if (ViewModel is null)
                {
                    return;
                }

                ViewModel!.CloseInteraction.RegisterHandler(
                        context =>
                        {
                            context.SetOutput(Unit.Default);

                            if (DialogHost?.IsOpen ?? false)
                            {
                                DialogHost.CloseDialogCommand.Execute(context.Input);
                            }
                        })
                    .DisposeWith(disposables);
            });
    }
    
    private DialogHost? DialogHost => this
        .GetVisualAncestors()
        .OfType<DialogHost>()
        .FirstOrDefault();
}