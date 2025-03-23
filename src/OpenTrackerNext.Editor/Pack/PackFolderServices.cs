using OpenTrackerNext.Core.Utils;
using OpenTrackerNext.ServiceCollection;
using OpenTrackerNext.SplatRegistration;

namespace OpenTrackerNext.Editor.Pack;

/// <summary>
/// Represents the service collection for pack folder services.
/// The contents are provided by source generator.
/// </summary>
[ServiceCollection]
[Splat(RegisterAsType = typeof(IServiceCollection<IPackFolderService>))]
[SplatSingleInstance]
public sealed partial class PackFolderServices : IServiceCollection<IPackFolderService>;