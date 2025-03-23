using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using OpenTrackerNext.Core.Metadata;
using OpenTrackerNext.Core.ViewModels;
using OpenTrackerNext.Core.ViewModels.Menus;
using OpenTrackerNext.Editor.Documents;
using OpenTrackerNext.Editor.Pack;
using OpenTrackerNext.SplatRegistration;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Editor.ViewModels.Tools;

/// <summary>
/// Represents the global settings tool view model.
/// </summary>
[Splat]
[SplatSingleInstance]
public sealed class GlobalToolViewModel : ViewModel
{
    /// <summary>
    /// A <see cref="string"/> representing the name of the metadata list box item.
    /// </summary>
    public const string MetadataItemName = "Metadata";

    private readonly IDocumentService _documentService;
    private readonly IPackFileService<PackMetadata> _metadataService;

    private readonly ReactiveCommand<Unit, Unit> _disabledCommand =
        ReactiveCommand.Create(() => { }, new Subject<bool>());

    /// <summary>
    /// Initializes a new instance of the <see cref="GlobalToolViewModel"/> class.
    /// </summary>
    /// <param name="documentService">
    /// The <see cref="IDocumentService"/>.
    /// </param>
    /// <param name="metadataService">
    /// The <see cref="IPackFileService{PackMetadata}"/>.
    /// </param>
    public GlobalToolViewModel(IDocumentService documentService, IPackFileService<PackMetadata> metadataService)
    {
        _documentService = documentService;
        _metadataService = metadataService;

        OpenCommand = null!;

        this.WhenActivated(
            disposables =>
            {
                ListBoxItems.Add(CreateMetadataListBoxItem());

                this.WhenAnyValue(x => x.SelectedListBoxItem)
                    .Select(x => x?.DoubleTapCommand ?? _disabledCommand)
                    .ToPropertyEx(
                        this,
                        x => x.OpenCommand,
                        scheduler: RxApp.MainThreadScheduler)
                    .DisposeWith(disposables);

                Disposable
                    .Create(() => ListBoxItems.Clear())
                    .DisposeWith(disposables);
            });
    }

    /// <summary>
    /// Gets an <see cref="ObservableCollection{T}"/> of <see cref="ListBoxItemViewModel"/> representing the list box
    /// items.
    /// </summary>
    public ObservableCollection<IListBoxItemViewModel> ListBoxItems { get; } = [];

    /// <summary>
    /// Gets or sets a <see cref="ListBoxItemViewModel"/> representing the selected list box item.
    /// </summary>
    [Reactive]
    public IListBoxItemViewModel? SelectedListBoxItem { get; set; }

    /// <summary>
    /// Gets a <see cref="ReactiveCommand{TParam,TResult}"/> representing the command to open the document represented
    /// by the selected list box item.
    /// </summary>
    [ObservableAsProperty]
    public ReactiveCommand<Unit, Unit> OpenCommand { get; }
    
    private ListBoxItemViewModel CreateMetadataListBoxItem()
    {
        var openCommand = ReactiveCommand.Create(
            () =>
            {
                if (_metadataService.File is null)
                {
                    return;
                }

                _documentService.Open(_metadataService.File);
            });

        return new ListBoxItemViewModel
        {
            Text = "Metadata",
            DoubleTapCommand = openCommand,
            ContextMenuItems = new ObservableCollection<MenuItemViewModel>
            {
                new()
                {
                    Icon = "mdi-folder-open",
                    Header = "Open",
                    Command = openCommand
                }
            }
        };
    }
}