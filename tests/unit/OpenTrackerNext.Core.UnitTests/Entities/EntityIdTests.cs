using System.Diagnostics.CodeAnalysis;
using OpenTrackerNext.Core.Entities;
using OpenTrackerNext.Core.UnitTests.Ids;

namespace OpenTrackerNext.Core.UnitTests.Entities;

[ExcludeFromCodeCoverage]
public sealed class EntityIdTests() : IdTests<EntityId>(EntityId.EntitiesFolderName);