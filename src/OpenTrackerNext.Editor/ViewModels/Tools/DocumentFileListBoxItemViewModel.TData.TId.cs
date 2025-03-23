using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Entities;
using OpenTrackerNext.Core.Icons;
using OpenTrackerNext.Core.Ids;
using OpenTrackerNext.Core.Layouts;
using OpenTrackerNext.Core.MapLayouts;
using OpenTrackerNext.Core.Maps;
using OpenTrackerNext.Core.UIPanes;
using OpenTrackerNext.Core.ViewModels;
using OpenTrackerNext.Core.ViewModels.Menus;
using OpenTrackerNext.Editor.Documents;
using OpenTrackerNext.Editor.Pack;
using OpenTrackerNext.SplatRegistration;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Editor.ViewModels.Tools;

/// <summary>
/// Represents a document data file list box item view model for a <see cref="TData"/> file with an ID of
/// <see cref="TId"/>.
/// </summary>
/// <typeparam name="TData">
/// The type of the file.
/// </typeparam>
/// <typeparam name="TId">
/// The type of the ID of the file.
/// </typeparam>
[SplatGeneric(
    ConcreteType = typeof(DocumentFileListBoxItemViewModel<EntityPrototype, EntityId>),
    RegisterAsType = typeof(DocumentFileListBoxItemViewModel<EntityPrototype, EntityId>.Factory))]
[SplatGeneric(
    ConcreteType = typeof(DocumentFileListBoxItemViewModel<IconPrototype, IconId>),
    RegisterAsType = typeof(DocumentFileListBoxItemViewModel<IconPrototype, IconId>.Factory))]
[SplatGeneric(
    ConcreteType = typeof(DocumentFileListBoxItemViewModel<LayoutPrototype, LayoutId>),
    RegisterAsType = typeof(DocumentFileListBoxItemViewModel<LayoutPrototype, LayoutId>.Factory))]
[SplatGeneric(
    ConcreteType = typeof(DocumentFileListBoxItemViewModel<MapPrototype, MapId>),
    RegisterAsType = typeof(DocumentFileListBoxItemViewModel<MapPrototype, MapId>.Factory))]
[SplatGeneric(
    ConcreteType = typeof(DocumentFileListBoxItemViewModel<MapLayoutPrototype, MapLayoutId>),
    RegisterAsType = typeof(DocumentFileListBoxItemViewModel<MapLayoutPrototype, MapLayoutId>.Factory))]
[SplatGeneric(
    ConcreteType = typeof(DocumentFileListBoxItemViewModel<UIPanePrototype, UIPaneId>),
    RegisterAsType = typeof(DocumentFileListBoxItemViewModel<UIPanePrototype, UIPaneId>.Factory))]
public sealed class DocumentFileListBoxItemViewModel<TData, TId> : ViewModel, IListBoxItemViewModel
    where TData : class, ITitledDocumentData<TData>, new()
    where TId : struct, IGuidId<TId>
{
    private readonly IPackFolderService<TData, TId> _folderService;
    private readonly IDocumentFile<TData, TId> _file;

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentFileListBoxItemViewModel{TData,TId}"/> class.
    /// </summary>
    /// <param name="documentService">
    /// The <see cref="IDocumentService"/>.
    /// </param>
    /// <param name="folderService">
    /// The <see cref="IPackFolderService{TFile,TId}"/>.
    /// </param>
    /// <param name="file">
    /// An <see cref="IDocumentFile{TData}"/> of <see cref="TData"/> representing the file.
    /// </param>
    public DocumentFileListBoxItemViewModel(
        IDocumentService documentService,
        IPackFolderService<TData, TId> folderService,
        IDocumentFile<TData, TId> file)
    {
        _folderService = folderService;
        _file = file;

        Text = _file.FriendlyId;

        DoubleTapCommand = ReactiveCommand.Create(() => documentService.Open(_file));
        RenameCommand = ReactiveCommand.CreateFromTask(RenameAsync);
        DeleteCommand = ReactiveCommand.CreateFromTask(DeleteAsync);

        ContextMenuItems = new ObservableCollection<MenuItemViewModel>
        {
            new()
            {
                Icon = "mdi-folder-open",
                Header = "Open",
                Command = DoubleTapCommand
            },
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
    /// A factory method for creating new <see cref="DocumentFileListBoxItemViewModel{TData,TId}"/> objects.
    /// </summary>
    /// <param name="file">
    /// An <see cref="IDocumentFile{TData,TId}"/> of <see cref="TData"/> representing the file.
    /// </param>
    /// <returns>
    /// A new <see cref="DocumentFileListBoxItemViewModel{TData,TId}"/> object.
    /// </returns>
    public delegate DocumentFileListBoxItemViewModel<TData, TId> Factory(IDocumentFile<TData, TId> file); 
    
    /// <inheritdoc/>
    public IEnumerable<MenuItemViewModel> ContextMenuItems { get; }

    /// <inheritdoc/>
    [ObservableAsProperty]
    public string Text { get; }

    /// <inheritdoc/>
    public ReactiveCommand<Unit, Unit> DoubleTapCommand { get; }

    /// <summary>
    /// Gets a <see cref="ReactiveCommand{TParam,TResult}"/> representing the command to rename the file.
    /// </summary>
    public ReactiveCommand<Unit, Unit> RenameCommand { get; }
    
    /// <summary>
    /// Gets a <see cref="ReactiveCommand{TParam,TResult}"/> representing the command to delete the file.
    /// </summary>
    public ReactiveCommand<Unit, Unit> DeleteCommand { get; }
    
    /// <inheritdoc/>
    public override Type GetViewType()
    {
        return typeof(IViewFor<IListBoxItemViewModel>);
    }

    private Task RenameAsync()
    {
        return _folderService.RenameFileAsync(_file);
    }

    private Task DeleteAsync()
    {
        return _folderService.DeleteFileAsync(_file);
    }
}