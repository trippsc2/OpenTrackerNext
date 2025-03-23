namespace OpenTrackerNext.Roslyn.SplatRegistration.Data;

/// <summary>
/// Represents a ReactiveUI view to be registered with Splat.
/// </summary>
/// <param name="TypeName">
/// A <see cref="string"/> representing the type name.
/// </param>
/// <param name="RegisterAsTypeName">
/// A <see cref="string"/> representing the type to register the type as.
/// </param>
public readonly record struct SplatReactiveViewRegistrationInfo(string TypeName, string RegisterAsTypeName);