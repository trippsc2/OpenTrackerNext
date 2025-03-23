using System.Reactive.Disposables;
using Avalonia.ReactiveUI;
using OpenTrackerNext.Editor.ViewModels.Documents;
using ReactiveUI;

namespace OpenTrackerNext.Editor.Views.Documents;

/// <summary>
/// Represents the document dock view.
/// </summary>
public sealed partial class DocumentDockView : ReactiveUserControl<DocumentDockViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentDockView"/> class.
    /// </summary>
    public DocumentDockView()
    {
        InitializeComponent();

        this.WhenActivated(
            disposables =>
            {
                this.OneWayBind(
                        ViewModel,
                        vm => vm.Documents,
                        v => v.TabControl.ItemsSource)
                    .DisposeWith(disposables);
                this.Bind(
                        ViewModel,
                        vm => vm.ActiveDocument,
                        v => v.TabControl.SelectedContent)
                    .DisposeWith(disposables);
            });
    }
}