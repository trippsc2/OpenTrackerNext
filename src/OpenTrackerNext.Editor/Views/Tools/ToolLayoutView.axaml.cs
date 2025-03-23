using System;
using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using OpenTrackerNext.Editor.Controls;
using OpenTrackerNext.Editor.Tools;
using OpenTrackerNext.Editor.ViewModels.Tools;
using ReactiveUI;

namespace OpenTrackerNext.Editor.Views.Tools;

/// <summary>
/// Represents the tool layout control.
/// </summary>
public sealed partial class ToolLayoutView : ReactiveUserControl<ToolLayoutViewModel>
{
    private static readonly SolidColorBrush HitBoxHighlightColor = SolidColorBrush.Parse("#660078D7");
    private HitBoxControl? _adornedHitBox;
    private Control? _adorner;

    /// <summary>
    /// Initializes a new instance of the <see cref="ToolLayoutView"/> class.
    /// </summary>
    public ToolLayoutView()
    {
        InitializeComponent();

        TopLeftHitBoxControl.Position = ToolBarPosition.TopLeft;
        TopRightHitBoxControl.Position = ToolBarPosition.TopRight;
        BottomLeftHitBoxControl.Position = ToolBarPosition.BottomLeft;
        BottomRightHitBoxControl.Position = ToolBarPosition.BottomRight;

        this.WhenActivated(
            disposables =>
            {
                this.OneWayBind(
                        ViewModel,
                        vm => vm.TopLeftToolBar,
                        v => v.TopLeftToolBarContentControl.Content)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        ViewModel,
                        vm => vm.TopRightToolBar,
                        v => v.TopRightToolBarContentControl.Content)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        ViewModel,
                        vm => vm.BottomLeftToolBar,
                        v => v.BottomLeftToolBarContentControl.Content)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        ViewModel,
                        vm => vm.BottomRightToolBar,
                        v => v.BottomRightToolBarContentControl.Content)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        ViewModel,
                        vm => vm.LeftToolPaneIsVisible,
                        v => v.LeftToolPaneContentControl.IsVisible)
                    .DisposeWith(disposables);
                this.OneWayBind(
                        ViewModel,
                        vm => vm.LeftToolPane,
                        v => v.LeftToolPaneContentControl.Content)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        ViewModel,
                        vm => vm.RightToolPaneIsVisible,
                        v => v.RightToolPaneContentControl.IsVisible)
                    .DisposeWith(disposables);
                this.OneWayBind(
                        ViewModel,
                        vm => vm.RightToolPane,
                        v => v.RightToolPaneContentControl.Content)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        ViewModel,
                        vm => vm.CenterContent,
                        v => v.CenterContentControl.Content)
                    .DisposeWith(disposables);
                this.WhenAnyValue(x => x.ViewModel!.CenterColumn)
                    .Subscribe(x => Grid.SetColumn(CenterContentPanel, x))
                    .DisposeWith(disposables);
                this.WhenAnyValue(x => x.ViewModel!.CenterColumnSpan)
                    .Subscribe(x => Grid.SetColumnSpan(CenterContentPanel, x))
                    .DisposeWith(disposables);

                this.OneWayBind(
                        ViewModel,
                        vm => vm.LeftToolPaneIsVisible,
                        v => v.LeftGridSplitter.IsVisible)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        ViewModel,
                        vm => vm.RightToolPaneIsVisible,
                        v => v.RightGridSplitter.IsVisible)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        ViewModel,
                        vm => vm.DragHitBoxesAreActive,
                        v => v.TopLeftHitBoxControl.IsHitTestVisible)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        ViewModel,
                        vm => vm.DragHitBoxesAreActive,
                        v => v.TopRightHitBoxControl.IsHitTestVisible)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        ViewModel,
                        vm => vm.DragHitBoxesAreActive,
                        v => v.BottomLeftHitBoxControl.IsHitTestVisible)
                    .DisposeWith(disposables);

                this.OneWayBind(
                        ViewModel,
                        vm => vm.DragHitBoxesAreActive,
                        v => v.BottomRightHitBoxControl.IsHitTestVisible)
                    .DisposeWith(disposables);

                AddHandler(DragDrop.DragOverEvent, OnDragOver);
                AddHandler(DragDrop.DragEnterEvent, OnDragEnter);
                AddHandler(DragDrop.DragLeaveEvent, OnDragLeave);
                AddHandler(DragDrop.DropEvent, OnDrop);

                Disposable
                    .Create(
                        () =>
                        {
                            RemoveHandler(DragDrop.DragOverEvent, OnDragOver);
                            RemoveHandler(DragDrop.DragEnterEvent, OnDragEnter);
                            RemoveHandler(DragDrop.DragLeaveEvent, OnDragLeave);
                            RemoveHandler(DragDrop.DropEvent, OnDrop);
                        })
                    .DisposeWith(disposables);
            });
    }

    private static void OnDragOver(object? sender, DragEventArgs eventArgs)
    {
        eventArgs.DragEffects &= DragDropEffects.Move;
    }

    private void OnDragEnter(object? sender, DragEventArgs eventArgs)
    {
        if (eventArgs.Source is not HitBoxControl control)
        {
            return;
        }

        RemoveAdorner();
        var layer = AdornerLayer.GetAdornerLayer(control);

        if (layer is null)
        {
            return;
        }

        _adornedHitBox = control;
        _adorner = new Panel
        {
            Background = HitBoxHighlightColor,
            IsHitTestVisible = false,
            [AdornerLayer.AdornedElementProperty] = control
        };
        ((ISetLogicalParent)_adorner).SetParent(control);
        layer.Children.Add(_adorner);
    }

    private void OnDragLeave(object? sender, DragEventArgs eventArgs)
    {
        RemoveAdorner();
    }

    private void OnDrop(object? sender, DragEventArgs eventArgs)
    {
        RemoveAdorner();

        if (eventArgs.Source is not HitBoxControl control)
        {
            return;
        }

        if (eventArgs.Data.Get(ToolService.ToolFormat) is not ToolViewModel tool)
        {
            return;
        }

        tool.MoveCommand.Execute(control.Position).Subscribe().Dispose();
    }

    private void RemoveAdorner()
    {
        if (_adornedHitBox is null)
        {
            return;
        }

        if (_adorner is null)
        {
            return;
        }

        var layer = AdornerLayer.GetAdornerLayer(_adornedHitBox);

        if (layer is null)
        {
            return;
        }

        layer.Children.Remove(_adorner);
        ((ISetLogicalParent)_adorner).SetParent(null);
        _adorner = null;
        _adornedHitBox = null;
    }
}