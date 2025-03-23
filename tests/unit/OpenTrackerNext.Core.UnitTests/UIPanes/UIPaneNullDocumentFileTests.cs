using System.Diagnostics.CodeAnalysis;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.UIPanes;
using OpenTrackerNext.Core.UnitTests.Documents;

namespace OpenTrackerNext.Core.UnitTests.UIPanes;

[ExcludeFromCodeCoverage]
public sealed class UIPaneNullDocumentFileTests()
    : NullDocumentFileTests<UIPanePrototype, UIPaneId>(NullDocumentFile<UIPanePrototype, UIPaneId>.Instance);