using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using OpenTrackerNext.Core.Entities;
using OpenTrackerNext.Core.Icons;
using OpenTrackerNext.Core.Ids;
using OpenTrackerNext.Core.Json;
using OpenTrackerNext.Core.Layouts;
using OpenTrackerNext.Core.MapLayouts;
using OpenTrackerNext.Core.Maps;
using OpenTrackerNext.Core.Storage;
using OpenTrackerNext.Core.UIPanes;
using OpenTrackerNext.SplatRegistration;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.Documents;

/// <inheritdoc cref="IDocumentFile{TData,TId}"/>
[SplatGeneric(
    ConcreteType = typeof(DocumentFile<EntityPrototype, EntityId>),
    RegisterAsType = typeof(IDocumentFile<EntityPrototype, EntityId>.Factory))]
[SplatGeneric(
    ConcreteType = typeof(DocumentFile<IconPrototype, IconId>),
    RegisterAsType = typeof(IDocumentFile<IconPrototype, IconId>.Factory))]
[SplatGeneric(
    ConcreteType = typeof(DocumentFile<LayoutPrototype, LayoutId>),
    RegisterAsType = typeof(IDocumentFile<LayoutPrototype, LayoutId>.Factory))]
[SplatGeneric(
    ConcreteType = typeof(DocumentFile<MapLayoutPrototype, MapLayoutId>),
    RegisterAsType = typeof(IDocumentFile<MapLayoutPrototype, MapLayoutId>.Factory))]
[SplatGeneric(
    ConcreteType = typeof(DocumentFile<MapPrototype, MapId>),
    RegisterAsType = typeof(IDocumentFile<MapPrototype, MapId>.Factory))]
[SplatGeneric(
    ConcreteType = typeof(DocumentFile<UIPanePrototype, UIPaneId>),
    RegisterAsType = typeof(IDocumentFile<UIPanePrototype, UIPaneId>.Factory))]
public sealed class DocumentFile<TData, TId> : ReactiveObject, IDocumentFile<TData, TId>
    where TData : class, ITitledDocumentData<TData>, new()
    where TId : IGuidId<TId>, new()
{
    private readonly CompositeDisposable _disposables = new();
    private readonly IStorageFile _file;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentFile{TData,TId}"/> class.
    /// </summary>
    /// <param name="file">
    /// A <see cref="IStorageFile"/> representing the document file.
    /// </param>
    public DocumentFile(IStorageFile file)
    {
        _file = file;
        Id = TId.Parse(file.Name);
        TitlePrefix = TData.TitlePrefix;
        FriendlyId = string.Empty;

        this.WhenAnyValue(x => x.OpenedInDocuments)
            .Select(x => x > 0)
            .ToPropertyEx(this, x => x.IsOpened)
            .DisposeWith(_disposables);

        this.WhenAnyValue(x => x.IsOpened)
            .Where(x => x)
            .Subscribe(_ => Revert())
            .DisposeWith(_disposables);
    }
    
    /// <inheritdoc/>
    public string TitlePrefix { get; }
    
    /// <inheritdoc/>
    public TData SavedData { get; } = new();

    /// <inheritdoc/>
    public TData WorkingData { get; } = new();

    /// <inheritdoc/>
    public TId Id { get; }
    
    /// <inheritdoc/>
    [Reactive]
    public string FriendlyId { get; private set; }

    /// <inheritdoc/>
    [Reactive]
    public int OpenedInDocuments { get; set; }

    [ObservableAsProperty]
    private bool IsOpened { get; }

    /// <inheritdoc/>
    public void Dispose()
    {
        _disposables.Dispose();
    }

    /// <inheritdoc/>
    public async Task LoadDataFromFileAsync()
    {
        var stream = _file.OpenRead();
        await using (stream.ConfigureAwait(false))
        {
            try
            {
                var namedData = await JsonContext
                    .DeserializeAsync<NamedData<TData>>(stream)
                    .ConfigureAwait(false);

                FriendlyId = namedData.Name;
                SavedData.MakeValueEqualTo(namedData.Data);
                WorkingData.MakeValueEqualTo(namedData.Data);
            }
            catch (JsonException)
            {
            }
        }
    }

    /// <inheritdoc/>
    public void Revert()
    {
        WorkingData.MakeValueEqualTo(SavedData);
    }

    /// <inheritdoc/>
    public async Task SaveAsync()
    {
        await SaveDataToFileAsync(WorkingData).ConfigureAwait(false);
        SavedData.MakeValueEqualTo(WorkingData);
    }

    /// <inheritdoc/>
    public void Delete()
    {
        _file.Delete();
    }

    /// <inheritdoc/>
    public async Task RenameAsync(string newName)
    {
        FriendlyId = newName;
        await SaveDataToFileAsync(SavedData).ConfigureAwait(false);
    }

    private async Task SaveDataToFileAsync(TData data)
    {
        var stream = _file.OpenWrite();
        await using (stream.ConfigureAwait(false))
        {
            var namedData = new NamedData<TData>
            {
                Name = FriendlyId,
                Data = data
            };
            
            await JsonContext
                .SerializeAsync(stream, namedData)
                .ConfigureAwait(false);
        }
    }
}