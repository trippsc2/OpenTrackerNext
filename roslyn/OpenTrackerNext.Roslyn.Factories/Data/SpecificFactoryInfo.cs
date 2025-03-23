namespace OpenTrackerNext.Roslyn.Factories.Data;

/// <summary>
/// Represents the information for a factory.
/// </summary>
/// <param name="SpecificFactoryTypeQualifiedName">
/// A <see cref="string"/> representing the name of the specific factory class.
/// </param>
/// <param name="SpecificFactoryTypeName">
/// A <see cref="string"/> representing the name of the specific factory type name.
/// </param>
/// <param name="SpecificFactoryTypeNamespace">
/// A <see cref="string"/> representing the name of the specific factory type namespace.
/// </param>
/// <param name="BaseInputTypeName">
/// A <see cref="string"/> representing the name of the base input type.
/// </param>
/// <param name="OutputTypeName">
/// A <see cref="string"/> representing the name of the output type.
/// </param>
/// <param name="InputTypeName">
/// A <see cref="string"/> representing the name of the input type.
/// </param>
public readonly record struct SpecificFactoryInfo(
    string SpecificFactoryTypeQualifiedName,
    string SpecificFactoryTypeName,
    string SpecificFactoryTypeNamespace,
    string BaseInputTypeName,
    string OutputTypeName,
    string InputTypeName);
