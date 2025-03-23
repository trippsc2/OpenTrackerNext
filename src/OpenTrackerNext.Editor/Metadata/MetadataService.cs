using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Metadata;
using OpenTrackerNext.Editor.Documents;
using OpenTrackerNext.Editor.Pack;
using OpenTrackerNext.SplatRegistration;

namespace OpenTrackerNext.Editor.Metadata;

/// <summary>
/// Represents the logic for managing the pack metadata.
/// </summary>
[Splat(RegisterAsType = typeof(IPackFileService<PackMetadata>))]
[SplatSingleInstance]
public sealed class MetadataService : PackFileService<PackMetadata>
{
    /// <summary>
    /// A <see cref="string"/> representing the metadata file name.
    /// </summary>
    public const string MetadataFileName = "metadata.json";

    /// <summary>
    /// Initializes a new instance of the <see cref="MetadataService"/> class.
    /// </summary>
    /// <param name="documentService">
    /// The <see cref="IDocumentService"/>.
    /// </param>
    /// <param name="fileFactory">
    /// An Autofac factory for creating new <see cref="IDocumentFile"/> of <see cref="PackMetadata"/> objects.
    /// </param>
    public MetadataService(IDocumentService documentService, IDocumentFile<PackMetadata>.Factory fileFactory)
        : base(documentService, fileFactory, MetadataFileName)
    {
    }
}