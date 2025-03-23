using System.Diagnostics.CodeAnalysis;
using OpenTrackerNext.Core.Maps;
using OpenTrackerNext.Core.UnitTests.Ids;

namespace OpenTrackerNext.Core.UnitTests.Maps;

[ExcludeFromCodeCoverage]
public sealed class MapIdTests() : IdTests<MapId>(MapId.MapsFolderName);