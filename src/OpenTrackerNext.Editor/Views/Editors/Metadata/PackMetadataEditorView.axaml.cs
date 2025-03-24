using System.Reactive.Disposables;
using Avalonia.ReactiveUI;
using OpenTrackerNext.Editor.ViewModels.Editors.Metadata;
using ReactiveUI;

namespace OpenTrackerNext.Editor.Views.Editors.Metadata;

/// <inheritdoc />
public sealed partial class PackMetadataEditorView : ReactiveUserControl<PackMetadataEditorViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PackMetadataEditorView"/> class.
    /// </summary>
    public PackMetadataEditorView()
    {
        InitializeComponent();

        this.WhenActivated(
            disposables =>
            {
                this.Bind(
                        ViewModel,
                        vm => vm.Name,
                        v => v.NameTextBox.Text)
                    .DisposeWith(disposables);
                
                this.Bind(
                        ViewModel,
                        vm => vm.Author,
                        v => v.AuthorTextBox.Text)
                    .DisposeWith(disposables);

                this.Bind(
                        ViewModel,
                        vm => vm.MajorVersion,
                        v => v.MajorVersionNumericUpDown.Value,
                        vmToViewConverter: major => major,
                        viewToVmConverter: value => (int)(value ?? 0))
                    .DisposeWith(disposables);

                this.Bind(
                        ViewModel,
                        vm => vm.MinorVersion,
                        v => v.MinorVersionNumericUpDown.Value,
                        vmToViewConverter: minor => minor,
                        viewToVmConverter: value => (int)(value ?? 0))
                    .DisposeWith(disposables);

                this.Bind(
                        ViewModel,
                        vm => vm.BuildVersion,
                        v => v.BuildVersionNumericUpDown.Value,
                        vmToViewConverter: build => build,
                        viewToVmConverter: value => (int)(value ?? 0))
                    .DisposeWith(disposables);

                this.Bind(
                        ViewModel,
                        vm => vm.RevisionVersion,
                        v => v.RevisionVersionNumericUpDown.Value,
                        vmToViewConverter: revision => revision,
                        viewToVmConverter: value => (int)(value ?? 0))
                    .DisposeWith(disposables);
            });
    }
}