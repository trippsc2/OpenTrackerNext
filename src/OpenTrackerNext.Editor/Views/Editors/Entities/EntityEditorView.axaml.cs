using System.Reactive.Disposables;
using Avalonia.ReactiveUI;
using OpenTrackerNext.Editor.ViewModels.Editors.Entities;
using ReactiveUI;

namespace OpenTrackerNext.Editor.Views.Editors.Entities;

/// <inheritdoc />
public sealed partial class EntityEditorView : ReactiveUserControl<EntityEditorViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EntityEditorView"/> class.
    /// </summary>
    public EntityEditorView()
    {
        InitializeComponent();

        this.WhenActivated(
            disposables =>
            {
                this.Bind(
                        ViewModel,
                        vm => vm.Minimum,
                        v => v.MinimumNumericUpDown.Value,
                        vmToViewConverter: minimum => minimum,
                        viewToVmConverter: value => (int)(value ?? 0))
                    .DisposeWith(disposables);
                
                this.Bind(
                        ViewModel,
                        vm => vm.Starting,
                        v => v.StartingNumericUpDown.Value,
                        vmToViewConverter: starting => starting,
                        viewToVmConverter: value => (int)(value ?? 0))
                    .DisposeWith(disposables);
                
                this.Bind(
                        ViewModel,
                        vm => vm.Maximum,
                        v => v.MaximumNumericUpDown.Value,
                        vmToViewConverter: maximum => maximum,
                        viewToVmConverter: value => (int)(value ?? 0))
                    .DisposeWith(disposables);
                
                this.OneWayBind(
                        ViewModel,
                        vm => vm.MinimumMaximumValue,
                        v => v.MinimumNumericUpDown.Maximum)
                    .DisposeWith(disposables);
                
                this.OneWayBind(
                        ViewModel,
                        vm => vm.StartingMinimumValue,
                        v => v.StartingNumericUpDown.Minimum)
                    .DisposeWith(disposables);
                
                this.OneWayBind(
                        ViewModel,
                        vm => vm.StartingMaximumValue,
                        v => v.StartingNumericUpDown.Maximum)
                    .DisposeWith(disposables);
                
                this.OneWayBind(
                        ViewModel,
                        vm => vm.MaximumMinimumValue,
                        v => v.MaximumNumericUpDown.Minimum)
                    .DisposeWith(disposables);
            });
    }
}