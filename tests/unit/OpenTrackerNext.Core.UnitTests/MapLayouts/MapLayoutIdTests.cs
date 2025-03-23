using System.Diagnostics.CodeAnalysis;
using OpenTrackerNext.Core.MapLayouts;
using OpenTrackerNext.Core.UnitTests.Ids;

namespace OpenTrackerNext.Core.UnitTests.MapLayouts;

[ExcludeFromCodeCoverage]
public sealed class MapLayoutIdTests() : IdTests<MapLayoutId>(MapLayoutId.MapLayoutsFolderName);