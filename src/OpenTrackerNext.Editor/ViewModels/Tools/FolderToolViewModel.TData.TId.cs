using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Entities;
using OpenTrackerNext.Core.Icons;
using OpenTrackerNext.Core.Ids;
using OpenTrackerNext.Core.Layouts;
using OpenTrackerNext.Core.MapLayouts;
using OpenTrackerNext.Core.Maps;
using OpenTrackerNext.Core.UIPanes;
using OpenTrackerNext.Core.ViewModels;
using OpenTrackerNext.Editor.Documents;
using OpenTrackerNext.Editor.Pack;
using OpenTrackerNext.SplatRegistration;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveCommand = ReactiveUI.ReactiveCommand;

namespace OpenTrackerNext.Editor.ViewModels.Tools;

/// <summary>
/// Represents a folder tool view model for a folder containing <see cref="TData"/> files with an ID of
/// <see cref="TId"/>.
/// </summary>
/// <typeparam name="TData">
/// The type of the files within the folder.
/// </typeparam>
/// <typeparam name="TId">
/// The type of the ID of the files within the folder.
/// </typeparam>
[SplatGeneric(ConcreteType = typeof(FolderToolViewModel<EntityPrototype, EntityId>))]
[SplatGeneric(ConcreteType = typeof(FolderToolViewModel<IconPrototype, IconId>))]
[SplatGeneric(ConcreteType = typeof(FolderToolViewModel<LayoutPrototype, LayoutId>))]
[SplatGeneric(ConcreteType = typeof(FolderToolViewModel<MapPrototype, MapId>))]
[SplatGeneric(ConcreteType = typeof(FolderToolViewModel<MapLayoutPrototype, MapLayoutId>))]
[SplatGeneric(ConcreteType = typeof(FolderToolViewModel<UIPanePrototype, UIPaneId>))]
[SplatSingleInstance]
public sealed class FolderToolViewModel<TData, TId> : ViewModel, IFolderToolViewModel
    where TData : class, ITitledDocumentData<TData>, new()
    where TId : struct, IGuidId<TId>
{
    private readonly IDocumentService _documentService;
    private readonly IPackFolderService<TData, TId> _folderService;
    private readonly DocumentFileListBoxItemViewModel<TData, TId>.Factory _listBoxItemFactory;

    private readonly ReactiveCommand<Unit, Unit> _disabledCommand =
        ReactiveCommand.Create(() => { }, new Subject<bool>());

    /// <summary>
    /// Initializes a new instance of the <see cref="FolderToolViewModel{TData,TId}"/> class.
    /// </summary>
    /// <param name="documentService">
    /// The <see cref="IDocumentService"/>.
    /// </param>
    /// <param name="folderService">
    /// The <see cref="IPackFolderService{TData,TId}"/> of <see cref="TData"/> files identified by
    /// <see cref="TId"/>.
    /// </param>
    /// <param name="listBoxItemFactory">
    /// A factory method for creating new <see cref="DocumentFileListBoxItemViewModel{TData,TId}"/> objects.
    /// </param>
    public FolderToolViewModel(
            IDocumentService documentService,
            IPackFolderService<TData, TId> folderService,
            DocumentFileListBoxItemViewModel<TData, TId>.Factory listBoxItemFactory)
    {
        _documentService = documentService;
        _folderService = folderService;
        _listBoxItemFactory = listBoxItemFactory;

        ListBoxItems = null!;

        AddCommand = ReactiveCommand.CreateFromTask(AddAsync);
        OpenCommand = null!;
        RenameCommand = null!;
        DeleteCommand = null!;

        this.WhenActivated(
            disposables =>
            {
                var folderItemsObservable = folderService
                    .Connect()
                    .Filter(x => x is not NullDocumentFile<TData, TId>);
                
                var comparer = SortExpressionComparer<IListBoxItemViewModel>.Ascending(x => x.Text);

                folderItemsObservable
                    .Transform(CreateListBoxItemViewModel)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .SortAndBind(out var listBoxItems, comparer)
                    .Subscribe()
                    .DisposeWith(disposables);

                ListBoxItems = listBoxItems;
                
                this.WhenAnyValue(x => x.SelectedListBoxItem)
                    .Select(
                        x =>
                            (x as DocumentFileListBoxItemViewModel<TData, TId>)?.DoubleTapCommand ?? _disabledCommand)
                    .ToPropertyEx(
                        this,
                        x => x.OpenCommand,
                        scheduler: RxApp.MainThreadScheduler)
                    .DisposeWith(disposables);

                this.WhenAnyValue(x => x.SelectedListBoxItem)
                    .Select(
                        x =>
                            (x as DocumentFileListBoxItemViewModel<TData, TId>)?.RenameCommand ?? _disabledCommand)
                    .ToPropertyEx(
                        this,
                        x => x.RenameCommand,
                        scheduler: RxApp.MainThreadScheduler)
                    .DisposeWith(disposables);

                this.WhenAnyValue(x => x.SelectedListBoxItem)
                    .Select(
                        x =>
                            (x as DocumentFileListBoxItemViewModel<TData, TId>)?.DeleteCommand ?? _disabledCommand)
                    .ToPropertyEx(
                        this,
                        x => x.DeleteCommand,
                        scheduler: RxApp.MainThreadScheduler)
                    .DisposeWith(disposables);
            });
    }

    /// <inheritdoc/>
    [Reactive]
    public ReadOnlyObservableCollection<IListBoxItemViewModel> ListBoxItems { get; private set; }
    
    /// <inheritdoc/>
    [Reactive]
    public IListBoxItemViewModel? SelectedListBoxItem { get; set; }

    /// <inheritdoc/>
    public ReactiveCommand<Unit, Unit> AddCommand { get; }
    
    /// <inheritdoc/>
    [ObservableAsProperty]
    public ReactiveCommand<Unit, Unit> OpenCommand { get; }
    
    /// <inheritdoc/>
    [ObservableAsProperty]
    public ReactiveCommand<Unit, Unit> RenameCommand { get; }
    
    /// <inheritdoc/>
    [ObservableAsProperty]
    public ReactiveCommand<Unit, Unit> DeleteCommand { get; }
    
    /// <inheritdoc/>
    public override Type GetViewType()
    {
        return typeof(IViewFor<IFolderToolViewModel>);
    }

    private IListBoxItemViewModel CreateListBoxItemViewModel(IDocumentFile<TData, TId> file)
    {
        return _listBoxItemFactory(file);
    }

    private async Task AddAsync()
    {
        var newFile = await _folderService.AddFileAsync().ConfigureAwait(false);

        if (newFile is not null)
        {
            _documentService.Open(newFile);
        }
    }
}