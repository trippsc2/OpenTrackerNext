using System.Diagnostics.CodeAnalysis;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Layouts;
using OpenTrackerNext.Core.UnitTests.Documents;

namespace OpenTrackerNext.Core.UnitTests.Layouts;

[ExcludeFromCodeCoverage]
public sealed class LayoutNullDocumentFileTests()
    : NullDocumentFileTests<LayoutPrototype, LayoutId>(NullDocumentFile<LayoutPrototype, LayoutId>.Instance);