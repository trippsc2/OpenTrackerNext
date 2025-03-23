using System.Diagnostics.CodeAnalysis;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Metadata;
using OpenTrackerNext.Core.UnitTests.Documents;

namespace OpenTrackerNext.Core.UnitTests.Metadata;

[ExcludeFromCodeCoverage]
public sealed class PackMetadataNullDocumentFileTests()
    : NullDocumentFileTests<PackMetadata>(NullDocumentFile<PackMetadata>.Instance);