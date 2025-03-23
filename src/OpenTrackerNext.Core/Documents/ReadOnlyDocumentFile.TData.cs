using System.Text.Json;
using System.Threading.Tasks;
using OpenTrackerNext.Core.Json;
using OpenTrackerNext.Core.Metadata;
using OpenTrackerNext.Core.Storage;
using OpenTrackerNext.SplatRegistration;

namespace OpenTrackerNext.Core.Documents;

/// <inheritdoc cref="IReadOnlyDocumentFile{TData}"/>
[SplatGeneric(
    ConcreteType = typeof(ReadOnlyDocumentFile<PackMetadata>),
    RegisterAsType = typeof(IReadOnlyDocumentFile<PackMetadata>.Factory))]
public sealed class ReadOnlyDocumentFile<TData> : IReadOnlyDocumentFile<TData>
    where TData : class, ITitledDocumentData<TData>, new()
{
    private readonly IReadOnlyStorageFile _file;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyDocumentFile{TData}"/> class.
    /// </summary>
    /// <param name="file">
    /// A <see cref="IReadOnlyStorageFile"/> representing the file.
    /// </param>
    public ReadOnlyDocumentFile(IReadOnlyStorageFile file)
    {
        _file = file;
    }

    /// <inheritdoc/>
    public TData Data { get; private set; } = null!;

    /// <inheritdoc/>
    public async Task LoadDataFromFileAsync()
    {
        var stream = _file.OpenRead();
        await using (stream.ConfigureAwait(false))
        {
            try
            {
                Data = await JsonContext
                    .DeserializeAsync<TData>(stream)
                    .ConfigureAwait(false);
            }
            catch (JsonException)
            {
            }
        }
    }
}