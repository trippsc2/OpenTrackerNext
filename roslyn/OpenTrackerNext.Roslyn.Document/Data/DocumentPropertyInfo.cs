using System.Collections.ObjectModel;

namespace OpenTrackerNext.Roslyn.Document.Data;

/// <summary>
/// Represents information about property members needed to generate the implementation of document value equality.
/// </summary>
/// <param name="PropertyTypeName">
/// A <see cref="string"/> representing the name of the property type.
/// </param>
/// <param name="PropertyName">
/// A <see cref="string"/> representing the name of the property.
/// </param>
/// <param name="IsObservableCollection">
/// A <see cref="bool"/> representing whether the property is an <see cref="ObservableCollection{T}"/>.
/// </param>
/// <param name="IsValueTypeOrImmutableReference">
/// A <see cref="bool"/> representing whether the property is a value type or an immutable reference type.
/// </param>
/// <param name="IsNullableReferenceType">
/// A <see cref="bool"/> representing whether the property is a nullable reference type.
/// </param>
public readonly record struct DocumentPropertyInfo(
    string PropertyTypeName,
    string PropertyName,
    bool IsObservableCollection,
    bool IsValueTypeOrImmutableReference,
    bool IsNullableReferenceType);