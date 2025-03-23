using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using OpenTrackerNext.Core.Images;
using OpenTrackerNext.Core.ViewModels;
using OpenTrackerNext.Core.ViewModels.Menus;
using OpenTrackerNext.Editor.Images;
using OpenTrackerNext.SplatRegistration;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Editor.ViewModels.Tools;

/// <summary>
/// Represents the image file list box item view model.
/// </summary>
[Splat(RegisterAsType = typeof(Factory))]
public sealed class ImageListBoxItemViewModel : ViewModel, IListBoxItemViewModel
{
    private readonly IImageService _imageService;
    private readonly IImageFile _file;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageListBoxItemViewModel"/> class.
    /// </summary>
    /// <param name="imageService">
    ///     The <see cref="IImageService"/>.
    /// </param>
    /// <param name="file">
    ///     An <see cref="IImageFile"/> representing the image file.
    /// </param>
    public ImageListBoxItemViewModel(IImageService imageService, IImageFile file)
    {
        _imageService = imageService;
        _file = file;

        Text = _file.FriendlyId;

        RenameCommand = ReactiveCommand.CreateFromTask(RenameAsync);
        DeleteCommand = ReactiveCommand.CreateFromTask(DeleteAsync);

        ContextMenuItems = new ObservableCollection<MenuItemViewModel>
        {
            new()
            {
                Icon = "mdi-rename",
                Header = "Rename...",
                Command = RenameCommand
            },
            new() { Header = "-" },
            new()
            {
                Icon = "mdi-delete",
                Header = "Delete...",
                Command = DeleteCommand
            }
        };

        this.WhenActivated(
            disposables =>
            {
                this.WhenAnyValue(x => x._file.FriendlyId)
                    .ToPropertyEx(
                        this,
                        x => x.Text,
                        scheduler: RxApp.MainThreadScheduler)
                    .DisposeWith(disposables);
            });
    }
    
    /// <summary>
    /// A factory method for creating new <see cref="ImageListBoxItemViewModel"/> from the specified image file.
    /// </summary>
    /// <param name="file">
    ///     An <see cref="IImageFile"/> representing the image file.
    /// </param>
    /// <returns>
    ///     A new <see cref="ImageListBoxItemViewModel"/> object.
    /// </returns>
    public delegate ImageListBoxItemViewModel Factory(IImageFile file);
    
    /// <inheritdoc/>
    public IEnumerable<MenuItemViewModel> ContextMenuItems { get; }

    /// <inheritdoc/>
    [ObservableAsProperty]
    public string Text { get; }

    /// <inheritdoc/>
    public ReactiveCommand<Unit, Unit> DoubleTapCommand { get; } = ReactiveCommand.Create(() => { });
    
    /// <summary>
    /// Gets a <see cref="ReactiveCommand{TParam,TResult}"/> representing the command to execute when renaming the file.
    /// </summary>
    public ReactiveCommand<Unit, Unit> RenameCommand { get; }
    
    /// <summary>
    /// Gets a <see cref="ReactiveCommand{TParam,TResult}"/> representing the command to execute when deleting the file.
    /// </summary>
    public ReactiveCommand<Unit, Unit> DeleteCommand { get; }
    
    /// <inheritdoc/>
    public override Type GetViewType()
    {
        return typeof(IViewFor<IListBoxItemViewModel>);
    }

    private Task RenameAsync()
    {
        return _imageService.RenameFileAsync(_file);
    }

    private Task DeleteAsync()
    {
        return _imageService.DeleteFileAsync(_file);
    }
}