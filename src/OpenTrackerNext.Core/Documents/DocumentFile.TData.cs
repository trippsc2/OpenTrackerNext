using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using OpenTrackerNext.Core.Json;
using OpenTrackerNext.Core.Metadata;
using OpenTrackerNext.Core.Storage;
using OpenTrackerNext.SplatRegistration;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.Documents;

/// <inheritdoc cref="IDocumentFile{TData}"/>
[SplatGeneric(
    ConcreteType = typeof(DocumentFile<PackMetadata>),
    RegisterAsType = typeof(IDocumentFile<PackMetadata>.Factory))]
public sealed class DocumentFile<TData> : ReactiveObject, IDocumentFile<TData>
    where TData : ITitledDocumentData<TData>, new()
{
    private readonly CompositeDisposable _disposables = new();
    private readonly IStorageFile _file;

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentFile{TData}"/> class.
    /// </summary>
    /// <param name="file">
    /// A <see cref="IStorageFile"/> representing the document file.
    /// </param>
    public DocumentFile(IStorageFile file)
    {
        _file = file;
        TitlePrefix = TData.TitlePrefix;

        this
            .WhenAnyValue(x => x.OpenedInDocuments)
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
    public string FriendlyId => string.Empty;

    /// <inheritdoc/>
    public TData SavedData { get; } = new();

    /// <inheritdoc/>
    public TData WorkingData { get; } = new();

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
                var data = await JsonContext
                    .DeserializeAsync<TData>(stream)
                    .ConfigureAwait(false);

                SavedData.MakeValueEqualTo(data);
                WorkingData.MakeValueEqualTo(data);
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
        var stream = _file.OpenWrite();
        await using (stream.ConfigureAwait(false))
        {
            await JsonContext
                .SerializeAsync(stream, WorkingData)
                .ConfigureAwait(false);
        }

        SavedData.MakeValueEqualTo(WorkingData);
    }

    /// <inheritdoc/>
    public void Delete()
    {
        _file.Delete();
    }
}