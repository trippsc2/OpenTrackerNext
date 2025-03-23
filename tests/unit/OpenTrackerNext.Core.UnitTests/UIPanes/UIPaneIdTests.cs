using System.Diagnostics.CodeAnalysis;
using OpenTrackerNext.Core.UIPanes;
using OpenTrackerNext.Core.UnitTests.Ids;

namespace OpenTrackerNext.Core.UnitTests.UIPanes;

[ExcludeFromCodeCoverage]
public sealed class UIPaneIdTests() : IdTests<UIPaneId>(UIPaneId.UIPanesFolderName);