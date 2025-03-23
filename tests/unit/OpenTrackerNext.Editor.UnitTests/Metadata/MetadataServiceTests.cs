using System.Diagnostics.CodeAnalysis;
using OpenTrackerNext.Core.Metadata;
using OpenTrackerNext.Editor.Metadata;
using OpenTrackerNext.Editor.UnitTests.Pack;

namespace OpenTrackerNext.Editor.UnitTests.Metadata;

[ExcludeFromCodeCoverage]
public sealed class MetadataServiceTests : PackFileServiceTests<PackMetadata>
{
    public MetadataServiceTests() : base(MetadataService.MetadataFileName)
    {
        Subject = new MetadataService(DocumentService, FileFactory);
    }
}