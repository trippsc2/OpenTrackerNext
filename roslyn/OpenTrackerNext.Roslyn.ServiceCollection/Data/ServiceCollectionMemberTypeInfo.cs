namespace OpenTrackerNext.Roslyn.ServiceCollection.Data;

/// <summary>
/// Represents the information about a service collection member type.
/// </summary>
/// <param name="TypeQualifiedName">
/// A <see cref="string"/> representing the fully qualified name of the type.
/// </param>
/// <param name="RegisteredAsTypeQualifiedName">
/// A <see cref="string"/> representing the Splat type name of the type.
/// </param>
public readonly record struct ServiceCollectionMemberTypeInfo(
    string TypeQualifiedName,
    string RegisteredAsTypeQualifiedName);