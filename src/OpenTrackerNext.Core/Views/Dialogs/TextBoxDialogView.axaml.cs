using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;
using DialogHostAvalonia;
using OpenTrackerNext.Core.ViewModels.Dialogs;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;

namespace OpenTrackerNext.Core.Views.Dialogs;

/// <summary>
/// Represents a text box dialog.
/// </summary>
public sealed partial class TextBoxDialogView : ReactiveUserControl<TextBoxDialogViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TextBoxDialogView"/> class.
    /// </summary>
    public TextBoxDialogView()
    {
        InitializeComponent();

        this.WhenActivated(
            disposables =>
            {
                this.OneWayBind(
                        ViewModel,
                        vm => vm.MaxWidth,
                        v => v.MaxWidth)
                    .DisposeWith(disposables);
                this.OneWayBind(
                        ViewModel,
                        vm => vm.MinWidth,
                        v => v.MinWidth)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        ViewModel,
                        vm => vm.IconIsVisible,
                        v => v.DialogIcon.IsVisible)
                    .DisposeWith(disposables);
                this.OneWayBind(
                        ViewModel,
                        vm => vm.IconValue,
                        v => v.DialogIcon.Value)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        ViewModel,
                        vm => vm.Title,
                        v => v.DialogTitleTextBox.Text)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        ViewModel,
                        vm => vm.Message,
                        v => v.DialogMessageTextBox.Text)
                    .DisposeWith(disposables);

                this.Bind(
                        ViewModel,
                        vm => vm.InputText,
                        v => v.DialogInputTextBox.Text)
                    .DisposeWith(disposables);

                this.BindValidation(
                        ViewModel,
                        vm => vm.InputText,
                        v => v.ErrorLabel.Content)
                    .DisposeWith(disposables);

                this.BindCommand(
                        ViewModel,
                        vm => vm.OkCommand,
                        v => v.OkButton)
                    .DisposeWith(disposables);

                this.BindCommand(
                        ViewModel,
                        vm => vm.CancelCommand,
                        v => v.CancelButton)
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