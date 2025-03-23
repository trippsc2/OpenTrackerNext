namespace OpenTrackerNext.Roslyn.ServiceCollection.Data;

/// <summary>
/// Represents a service collection.
/// </summary>
/// <param name="NamespaceName">
/// A <see cref="string"/> representing the namespace of the service collection.
/// </param>
/// <param name="TypeName">
/// A <see cref="string"/> representing the type name of the service collection.
/// </param>
/// <param name="ServiceTypeName">
/// A <see cref="string"/> representing the type name of the service.
/// </param>
/// <param name="ServiceTypeNamespaceName">
/// A <see cref="string"/> representing the namespace of the service.
/// </param>
public readonly record struct ServiceCollectionInfo(
    string NamespaceName,
    string TypeName,
    string ServiceTypeName,
    string? ServiceTypeNamespaceName);