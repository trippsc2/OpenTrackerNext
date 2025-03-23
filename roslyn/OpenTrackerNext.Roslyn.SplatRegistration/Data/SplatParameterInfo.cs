namespace OpenTrackerNext.Roslyn.SplatRegistration.Data;

/// <summary>
/// Represents a constructor or delegate parameter in a Splat registration.
/// </summary>
/// <param name="TypeName">
/// A <see cref="string"/> representing the name of the type.
/// </param>
/// <param name="ParameterName">
/// A <see cref="string"/> representing the name of the parameter.
/// </param>
/// <param name="IsNullable">
/// A <see cref="bool"/> representing whether the parameter is nullable.
/// </param>
public readonly record struct SplatParameterInfo(string TypeName, string ParameterName, bool IsNullable);