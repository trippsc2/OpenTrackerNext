using System.Diagnostics.CodeAnalysis;
using OpenTrackerNext.Core.Icons;
using OpenTrackerNext.Core.UnitTests.Ids;

namespace OpenTrackerNext.Core.UnitTests.Icons;

[ExcludeFromCodeCoverage]
public sealed class IconIdTests() : IdTests<IconId>(IconId.IconsFolderName);