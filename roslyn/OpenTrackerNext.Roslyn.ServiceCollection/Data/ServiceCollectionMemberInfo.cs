namespace OpenTrackerNext.Roslyn.ServiceCollection.Data;

/// <summary>
/// Represents the information about a service collection member.
/// </summary>
/// <param name="TypeInfo">
/// A <see cref="ServiceCollectionMemberTypeInfo"/> representing the type information.
/// </param>
/// <param name="CollectionInfo">
/// A <see cref="ServiceCollectionInfo"/> representing the collection information.
/// </param>
public readonly record struct ServiceCollectionMemberInfo(
    ServiceCollectionMemberTypeInfo TypeInfo,
    ServiceCollectionInfo CollectionInfo);