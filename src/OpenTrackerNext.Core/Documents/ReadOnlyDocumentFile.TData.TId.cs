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

namespace OpenTrackerNext.Core.Documents;

/// <inheritdoc />
[SplatGeneric(
    ConcreteType = typeof(ReadOnlyDocumentFile<EntityPrototype, EntityId>),
    RegisterAsType = typeof(IReadOnlyDocumentFile<EntityPrototype, EntityId>.Factory))]
[SplatGeneric(
    ConcreteType = typeof(ReadOnlyDocumentFile<IconPrototype, IconId>),
    RegisterAsType = typeof(IReadOnlyDocumentFile<IconPrototype, IconId>.Factory))]
[SplatGeneric(
    ConcreteType = typeof(ReadOnlyDocumentFile<LayoutPrototype, LayoutId>),
    RegisterAsType = typeof(IReadOnlyDocumentFile<LayoutPrototype, LayoutId>.Factory))]
[SplatGeneric(
    ConcreteType = typeof(ReadOnlyDocumentFile<MapLayoutPrototype, MapLayoutId>),
    RegisterAsType = typeof(IReadOnlyDocumentFile<MapLayoutPrototype, MapLayoutId>.Factory))]
[SplatGeneric(
    ConcreteType = typeof(ReadOnlyDocumentFile<MapPrototype, MapId>),
    RegisterAsType = typeof(IReadOnlyDocumentFile<MapPrototype, MapId>.Factory))]
[SplatGeneric(
    ConcreteType = typeof(ReadOnlyDocumentFile<UIPanePrototype, UIPaneId>),
    RegisterAsType = typeof(IReadOnlyDocumentFile<UIPanePrototype, UIPaneId>.Factory))]
public sealed class ReadOnlyDocumentFile<TData, TId>
    : IReadOnlyDocumentFile<TData, TId>
    where TData : class, ITitledDocumentData<TData>, new()
    where TId : struct, IGuidId<TId>
{
    private readonly IReadOnlyStorageFile _file;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyDocumentFile{TData,TId}"/> class.
    /// </summary>
    /// <param name="file">
    /// A <see cref="IReadOnlyStorageFile"/> representing the file.
    /// </param>
    public ReadOnlyDocumentFile(IReadOnlyStorageFile file)
    {
        _file = file;
        Id = TId.Parse(file.Name);
    }

    /// <inheritdoc/>
    public TData Data { get; private set; } = null!;

    /// <inheritdoc/>
    public TId Id { get; }

    /// <inheritdoc/>
    public async Task LoadDataFromFileAsync()
    {
        var stream = _file.OpenRead();
        await using (stream.ConfigureAwait(false))
        {
            try
            {
                var namedJsonData = await JsonContext
                    .DeserializeAsync<NamedData<TData>>(stream)
                    .ConfigureAwait(false);

                Data = namedJsonData.Data;
            }
            catch (JsonException)
            {
            }
        }
    }
}