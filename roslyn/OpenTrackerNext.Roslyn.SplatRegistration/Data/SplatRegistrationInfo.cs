using System.Collections.Immutable;

namespace OpenTrackerNext.Roslyn.SplatRegistration.Data;

/// <summary>
/// Represents a Splat registration for a non-generic concrete type.
/// </summary>
/// <param name="ConcreteTypeName">
/// A <see cref="string"/> representing the name of the concrete type.
/// </param>
/// <param name="RegisterAsTypeName">
/// A <see cref="string"/> representing the name of the type to be registered as.
/// </param>
/// <param name="IsSingleInstance">
/// A <see cref="bool"/> representing whether the concrete type is a single instance.
/// </param>
/// <param name="IsDelegate">
/// A <see cref="bool"/> representing whether the concrete type is a delegate.
/// </param>
/// <param name="ConstructorParameters">
/// An <see cref="ImmutableArray{T}"/> of <see cref="SplatParameterInfo"/> representing the constructor parameters.
/// </param>
/// <param name="DelegateParameters">
/// An <see cref="ImmutableArray{T}"/> of <see cref="SplatParameterInfo"/> representing the delegate parameters.
/// </param>
public readonly record struct SplatRegistrationInfo(
    string ConcreteTypeName,
    string RegisterAsTypeName,
    bool IsSingleInstance,
    bool IsDelegate,
    ImmutableArray<SplatParameterInfo> ConstructorParameters,
    ImmutableArray<SplatParameterInfo> DelegateParameters);