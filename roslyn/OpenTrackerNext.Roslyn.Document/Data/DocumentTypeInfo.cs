using System.Collections.Immutable;

namespace OpenTrackerNext.Roslyn.Document.Data;

/// <summary>
/// Represents information needed to generate the implementation of document value equality for the type.
/// </summary>
/// <param name="ImplementAsExtensionMethod">
/// A <see cref="bool"/> representing whether the implementation should be an extension method.
/// </param>
/// <param name="NamespaceName">
/// A <see cref="string"/> representing the namespace of the class.
/// </param>
/// <param name="ClassName">
/// A <see cref="string"/> representing the name of the class.
/// </param>
/// <param name="TaggedWithIDocumentData">
/// A <see cref="bool"/> representing whether the class is tagged with IDocumentData{TSelf} interface.
/// </param>
/// <param name="Properties">
/// An <see cref="ImmutableArray{T}"/> of <see cref="DocumentPropertyInfo"/> representing the properties of the
/// class.
/// </param>
public readonly record struct DocumentTypeInfo(
    bool ImplementAsExtensionMethod,
    string? NamespaceName,
    string ClassName,
    bool TaggedWithIDocumentData,
    ImmutableArray<DocumentPropertyInfo> Properties);