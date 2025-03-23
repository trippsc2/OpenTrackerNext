using System.Collections.Immutable;

namespace OpenTrackerNext.Roslyn.Factories.Data;

/// <summary>
/// Represents the information for a factory implementation.
/// </summary>
/// <param name="AbstractFactoryInfo">
/// A <see cref="AbstractFactoryInfo"/> representing the abstract factory information.
/// </param>
/// <param name="SpecificFactories">
/// An <see cref="ImmutableArray{T}"/> of <see cref="SpecificFactoryInfo"/> representing the specific factories.
/// </param>
public readonly record struct FactoryGroupInfo(
    AbstractFactoryInfo AbstractFactoryInfo,
    ImmutableArray<SpecificFactoryInfo> SpecificFactories);