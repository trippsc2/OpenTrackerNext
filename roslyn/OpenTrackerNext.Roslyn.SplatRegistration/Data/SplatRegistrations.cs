using System.Collections.Immutable;

namespace OpenTrackerNext.Roslyn.SplatRegistration.Data;

/// <summary>
/// Represents a Splat registrations.
/// </summary>
/// <param name="AssemblyName">
/// A <see cref="string"/> representing the name of the assembly.
/// </param>
/// <param name="ReactiveViewRegistrations">
/// An <see cref="ImmutableArray{T}"/> of <see cref="SplatReactiveViewRegistrationInfo"/> representing the reactive
/// view registrations.
/// </param>
/// <param name="NonGenericRegistrations">
/// An <see cref="ImmutableArray{T}"/> of <see cref="SplatRegistrationInfo"/> representing the non-generic
/// registrations.
/// </param>
/// <param name="GenericRegistrations">
/// An <see cref="ImmutableArray{T}"/> of <see cref="ImmutableArray{T}"/> of <see cref="SplatRegistrationInfo"/>
/// representing the generic registrations.
/// </param>
public readonly record struct SplatRegistrations(
    string? AssemblyName,
    ImmutableArray<SplatReactiveViewRegistrationInfo?> ReactiveViewRegistrations,
    ImmutableArray<SplatRegistrationInfo?> NonGenericRegistrations,
    ImmutableArray<ImmutableArray<SplatRegistrationInfo>> GenericRegistrations);