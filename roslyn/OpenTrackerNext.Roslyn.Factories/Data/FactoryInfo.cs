using System.Collections.Immutable;

namespace OpenTrackerNext.Roslyn.Factories.Data;

/// <summary>
/// Represents the information for factory implementations.
/// </summary>
/// <param name="FactoryGroups">
/// An <see cref="ImmutableArray{T}"/> of <see cref="FactoryGroupInfo"/> representing the factory groups.
/// </param>
/// <param name="SpecificFactories">
/// An <see cref="ImmutableArray{T}"/> of <see cref="SpecificFactoryInfo"/> representing the specific factories.
/// </param>
public readonly record struct FactoryInfo(
    ImmutableArray<FactoryGroupInfo> FactoryGroups,
    ImmutableArray<SpecificFactoryInfo> SpecificFactories);