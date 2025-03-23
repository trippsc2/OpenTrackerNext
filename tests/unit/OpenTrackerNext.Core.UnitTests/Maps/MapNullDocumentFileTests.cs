using System.Diagnostics.CodeAnalysis;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Maps;
using OpenTrackerNext.Core.UnitTests.Documents;

namespace OpenTrackerNext.Core.UnitTests.Maps;

[ExcludeFromCodeCoverage]
public sealed class MapNullDocumentFileTests()
    : NullDocumentFileTests<MapPrototype, MapId>(NullDocumentFile<MapPrototype, MapId>.Instance);