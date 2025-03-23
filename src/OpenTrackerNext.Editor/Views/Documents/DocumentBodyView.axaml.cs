using System.Reactive.Disposables;
using Avalonia.ReactiveUI;
using OpenTrackerNext.Editor.ViewModels.Documents;
using ReactiveUI;

namespace OpenTrackerNext.Editor.Views.Documents;

/// <summary>
/// Represents the document body view.
/// </summary>
public sealed partial class DocumentBodyView : ReactiveUserControl<DocumentBodyViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentBodyView"/> class.
    /// </summary>
    public DocumentBodyView()
    {
        InitializeComponent();

        this.WhenActivated(
            disposables =>
            {
                this.OneWayBind(
                        ViewModel,
                        vm => vm.Content,
                        v => v.ContentControl.Content)
                    .DisposeWith(disposables);

                this.BindCommand(
                        ViewModel,
                        vm => vm.RevertCommand,
                        v => v.RevertButton)
                    .DisposeWith(disposables);

                this.BindCommand(
                        ViewModel,
                        vm => vm.SaveCommand,
                        v => v.SaveButton)
                    .DisposeWith(disposables);
            });
    }
}