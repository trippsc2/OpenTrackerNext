using System.Diagnostics.CodeAnalysis;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Icons;
using OpenTrackerNext.Core.UnitTests.Documents;

namespace OpenTrackerNext.Core.UnitTests.Icons;

[ExcludeFromCodeCoverage]
public sealed class IconNullDocumentFileTests()
    : NullDocumentFileTests<IconPrototype, IconId>(NullDocumentFile<IconPrototype, IconId>.Instance);