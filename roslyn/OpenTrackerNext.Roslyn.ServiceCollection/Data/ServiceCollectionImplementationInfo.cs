using System.Collections.Immutable;

namespace OpenTrackerNext.Roslyn.ServiceCollection.Data;

/// <summary>
/// Represents the information about a service collection implementation.
/// </summary>
/// <param name="ServiceCollectionInfo">
/// A <see cref="ServiceCollectionInfo"/> representing the service collection information.
/// </param>
/// <param name="MemberTypesInfo">
/// An <see cref="ImmutableArray{T}"/> of <see cref="ServiceCollectionMemberTypeInfo"/> representing the member
/// type information.
/// </param>
public readonly record struct ServiceCollectionImplementationInfo(
    ServiceCollectionInfo ServiceCollectionInfo,
    ImmutableArray<ServiceCollectionMemberTypeInfo> MemberTypesInfo);