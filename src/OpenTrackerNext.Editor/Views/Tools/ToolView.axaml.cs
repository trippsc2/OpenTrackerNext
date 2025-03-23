using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Input;
using Avalonia.ReactiveUI;
using OpenTrackerNext.Editor.Tools;
using OpenTrackerNext.Editor.ViewModels.Tools;
using ReactiveUI;

namespace OpenTrackerNext.Editor.Views.Tools;

/// <summary>
/// Represents the tool control.
/// </summary>
public sealed partial class ToolView : ReactiveUserControl<ToolViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ToolView"/> class.
    /// </summary>
    public ToolView()
    {
        InitializeComponent();

        this.WhenActivated(
            disposables =>
            {
                this.OneWayBind(
                        ViewModel,
                        vm => vm.Title,
                        v => v.TitleLabel.Content)
                    .DisposeWith(disposables);

                this.BindCommand(
                        ViewModel,
                        vm => vm.DeactivateCommand,
                        v => v.CloseButton)
                    .DisposeWith(disposables);

                if (ViewModel is null)
                {
                    return;
                }

                TitleBarBorder.PointerPressed += OnButtonDrag;

                Disposable
                    .Create(() => TitleBarBorder.PointerPressed -= OnButtonDrag)
                    .DisposeWith(disposables);
            });
    }

    // ReSharper disable once AsyncVoidMethod
    private async void OnButtonDrag(object? sender, PointerPressedEventArgs args)
    {
        if (ViewModel is null)
        {
            return;
        }

        var data = new DataObject();
        data.Set(ToolService.ToolFormat, ViewModel);

        await ViewModel.EnableDragHitBoxesCommand.Execute(Unit.Default);

        await DragDrop.DoDragDrop(args, data, DragDropEffects.Move).ConfigureAwait(true);

        await ViewModel.DisableDragHitBoxesCommand.Execute(Unit.Default);
    }
}