using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using OpenTrackerNext.Core.Images;
using OpenTrackerNext.Core.ViewModels;
using OpenTrackerNext.Editor.Images;
using OpenTrackerNext.SplatRegistration;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Editor.ViewModels.Tools;

/// <summary>
/// Represents the images tool control view model.
/// </summary>
[Splat]
[SplatSingleInstance]
public sealed class ImagesToolViewModel : ViewModel
{
    private readonly IImageService _imageService;

    private readonly ImageListBoxItemViewModel.Factory _listBoxItemFactory;

    private readonly ReactiveCommand<Unit, Unit> _disabledCommand =
        ReactiveCommand.Create(() => { }, new Subject<bool>());

    /// <summary>
    /// Initializes a new instance of the <see cref="ImagesToolViewModel"/> class.
    /// </summary>
    /// <param name="imageService">
    ///     The <see cref="IImageService"/>.
    /// </param>
    /// <param name="listBoxItemFactory">
    ///     An Autofac factory for creating new <see cref="ImageListBoxItemViewModel"/> objects.
    /// </param>
    public ImagesToolViewModel(IImageService imageService, ImageListBoxItemViewModel.Factory listBoxItemFactory)
    {
        _imageService = imageService;
        _listBoxItemFactory = listBoxItemFactory;

        AddCommand = ReactiveCommand.CreateFromTask(AddAsync);
        RenameCommand = null!;
        DeleteCommand = null!;

        this.WhenActivated(
            disposables =>
            {
                var imagesObservable = imageService.Connect()
                    .Filter(x => x is not NullImageFile);

                var comparer = SortExpressionComparer<ImageListBoxItemViewModel>.Ascending(x => x.Text);

                imagesObservable
                    .Transform(CreateListBoxItemViewModel)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .SortAndBind(out var listBoxItems, comparer)
                    .Subscribe()
                    .DisposeWith(disposables);

                ListBoxItems = listBoxItems;
                
                this.WhenAnyValue(x => x.SelectedListBoxItem)
                    .Select(x => x?.RenameCommand ?? _disabledCommand)
                    .ToPropertyEx(
                        this,
                        x => x.RenameCommand,
                        scheduler: RxApp.MainThreadScheduler)
                    .DisposeWith(disposables);

                this.WhenAnyValue(x => x.SelectedListBoxItem)
                    .Select(x => x?.DeleteCommand ?? _disabledCommand)
                    .ToPropertyEx(
                        this,
                        x => x.DeleteCommand,
                        scheduler: RxApp.MainThreadScheduler)
                    .DisposeWith(disposables);
            });
    }

    /// <summary>
    /// Gets a <see cref="ReadOnlyObservableCollection{T}"/> of <see cref="ImageListBoxItemViewModel"/> representing
    /// the list box items.
    /// </summary>
    [Reactive]
    public ReadOnlyObservableCollection<ImageListBoxItemViewModel> ListBoxItems { get; private set; } = null!;
    
    /// <summary>
    /// Gets or sets a <see cref="ImageListBoxItemViewModel"/> representing the selected list box item.
    /// </summary>
    [Reactive]
    public ImageListBoxItemViewModel? SelectedListBoxItem { get; set; }
    
    /// <summary>
    /// Gets a <see cref="ReactiveCommand{TParam,TResult}"/> representing the command to add an image.
    /// </summary>
    [Reactive]
    public ReactiveCommand<Unit, Unit> AddCommand { get; private set; }
    
    /// <summary>
    /// Gets a <see cref="ReactiveCommand{TParam,TResult}"/> representing the command to rename the selected image.
    /// </summary>
    [ObservableAsProperty]
    public ReactiveCommand<Unit, Unit> RenameCommand { get; }
    
    /// <summary>
    /// Gets a <see cref="ReactiveCommand{TParam,TResult}"/> representing the command to delete the selected image.
    /// </summary>
    [ObservableAsProperty]
    public ReactiveCommand<Unit, Unit> DeleteCommand { get; }
    
    private ImageListBoxItemViewModel CreateListBoxItemViewModel(IImageFile imageFile)
    {
        return _listBoxItemFactory(imageFile);
    }

    private async Task AddAsync()
    {
        await _imageService
            .AddFilesAsync()
            .ConfigureAwait(false);
    }
}