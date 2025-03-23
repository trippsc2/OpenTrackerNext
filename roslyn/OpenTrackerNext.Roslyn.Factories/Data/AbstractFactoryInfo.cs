namespace OpenTrackerNext.Roslyn.Factories.Data;

/// <summary>
/// Represents the information for an abstract factory.
/// </summary>
/// <param name="AbstractFactoryTypeName">
/// A <see cref="string"/> representing the name of the abstract factory class.
/// </param>
/// <param name="AbstractFactoryNamespace">
/// A <see cref="string"/> representing the namespace of the abstract factory class.
/// </param>
/// <param name="FactoryInterfaceTypeName">
/// A <see cref="string"/> representing the name of the factory interface.
/// </param>
/// <param name="BaseInputTypeName">
/// A <see cref="string"/> representing the name of the base input type.
/// </param>
/// <param name="OutputTypeName">
/// A <see cref="string"/> representing the name of the output type.
/// </param>
public readonly record struct AbstractFactoryInfo(
    string AbstractFactoryTypeName,
    string AbstractFactoryNamespace,
    string FactoryInterfaceTypeName,
    string BaseInputTypeName,
    string OutputTypeName);