using System.Diagnostics.CodeAnalysis;
using OpenTrackerNext.Core.Layouts;
using OpenTrackerNext.Core.UnitTests.Ids;

namespace OpenTrackerNext.Core.UnitTests.Layouts;

[ExcludeFromCodeCoverage]
public sealed class LayoutIdTests() : IdTests<LayoutId>(LayoutId.LayoutsFolderName);