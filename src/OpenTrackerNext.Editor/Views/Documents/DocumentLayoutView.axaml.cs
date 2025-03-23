using System;
using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using OpenTrackerNext.Editor.ViewModels.Documents;
using ReactiveUI;

namespace OpenTrackerNext.Editor.Views.Documents;

/// <summary>
/// Represents the document layout view.
/// </summary>
public sealed partial class DocumentLayoutView : ReactiveUserControl<DocumentLayoutViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentLayoutView"/> class.
    /// </summary>
    public DocumentLayoutView()
    {
        InitializeComponent();

        this.WhenActivated(
            disposables =>
            {
                this.OneWayBind(
                        ViewModel,
                        vm => vm.LeftDock,
                        v => v.LeftContentControl.Content)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        ViewModel,
                        vm => vm.RightDock,
                        v => v.RightContentControl.Content)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        ViewModel,
                        vm => vm.RightDockIsVisible,
                        v => v.GridSplitter.IsVisible)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        ViewModel,
                        vm => vm.RightDockIsVisible,
                        v => v.RightContentControl.IsVisible)
                    .DisposeWith(disposables);

                if (ViewModel is null)
                {
                    return;
                }

                this.WhenAnyValue(x => x.ViewModel!.LeftDockColumnSpan)
                    .Subscribe(x => Grid.SetColumnSpan(LeftContentControl, x))
                    .DisposeWith(disposables);
            });
    }
}