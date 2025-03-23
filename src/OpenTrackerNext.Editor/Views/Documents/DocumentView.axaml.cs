using System.Reactive.Disposables;
using Avalonia.ReactiveUI;
using OpenTrackerNext.Editor.ViewModels.Documents;
using ReactiveUI;

namespace OpenTrackerNext.Editor.Views.Documents;

/// <summary>
/// Represents the document view.
/// </summary>
public sealed partial class DocumentView : ReactiveUserControl<DocumentViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentView"/> class.
    /// </summary>
    public DocumentView()
    {
        InitializeComponent();

        this.WhenActivated(
            disposables =>
            {
                this.OneWayBind(
                        ViewModel,
                        vm => vm.Body,
                        v => v.BodyContentControl.Content)
                    .DisposeWith(disposables);
            });
    }
}