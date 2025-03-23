using System.Diagnostics.CodeAnalysis;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Entities;
using OpenTrackerNext.Core.UnitTests.Documents;

namespace OpenTrackerNext.Core.UnitTests.Entities;

[ExcludeFromCodeCoverage]
public sealed class EntityNullDocumentFileTests()
    : NullDocumentFileTests<EntityPrototype, EntityId>(NullDocumentFile<EntityPrototype, EntityId>.Instance);