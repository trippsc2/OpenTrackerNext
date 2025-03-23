using System.Diagnostics.CodeAnalysis;
using StronglyTypedIds;

namespace OpenTrackerNext.Editor.Tools;

/// <summary>
/// Represents a string identifying a tool.
/// </summary>
[ExcludeFromCodeCoverage]
[StronglyTypedId(StronglyTypedIdBackingType.String, StronglyTypedIdConverter.None)]
public partial struct ToolId;