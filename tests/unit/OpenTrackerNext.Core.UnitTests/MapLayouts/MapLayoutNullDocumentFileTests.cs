using System.Diagnostics.CodeAnalysis;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.MapLayouts;
using OpenTrackerNext.Core.UnitTests.Documents;

namespace OpenTrackerNext.Core.UnitTests.MapLayouts;

[ExcludeFromCodeCoverage]
public sealed class MapLayoutNullDocumentFileTests()
    : NullDocumentFileTests<MapLayoutPrototype, MapLayoutId>(NullDocumentFile<MapLayoutPrototype, MapLayoutId>.Instance);