using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;
using DialogHostAvalonia;
using OpenTrackerNext.Core.ViewModels.Dialogs;
using ReactiveUI;

namespace OpenTrackerNext.Core.Views.Dialogs;

/// <summary>
/// Represents a yes/no dialog.
/// </summary>
public sealed partial class YesNoDialogView : ReactiveUserControl<YesNoDialogViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="YesNoDialogView"/> class.
    /// </summary>
    public YesNoDialogView()
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
                        vm => vm.IconIsVisible,
                        v => v.Icon.IsVisible)
                    .DisposeWith(disposables);
                this.OneWayBind(
                        ViewModel,
                        vm => vm.IconValue,
                        v => v.Icon.Value)
                    .DisposeWith(disposables);

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

                this.BindCommand(
                        ViewModel,
                        vm => vm.YesCommand,
                        v => v.YesButton)
                    .DisposeWith(disposables);

                this.BindCommand<YesNoDialogView, YesNoDialogViewModel, ReactiveCommand<Unit, Unit>, Button>(
                        ViewModel,
                        vm => vm.NoCommand,
                        v => v.NoButton)
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