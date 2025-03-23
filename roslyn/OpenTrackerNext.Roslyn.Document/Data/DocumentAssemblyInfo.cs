using System.Collections.Immutable;

namespace OpenTrackerNext.Roslyn.Document.Data;

/// <summary>
/// Represents the information needed to generate the implementation for document value equality for the assembly.
/// </summary>
/// <param name="AssemblyName">
/// A <see cref="string"/> representing the name of the assembly.
/// </param>
/// <param name="Classes">
/// An <see cref="ImmutableArray{T}"/> of <see cref="DocumentTypeInfo"/> representing the classes of the assembly.
/// </param>
public readonly record struct DocumentAssemblyInfo(string AssemblyName, ImmutableArray<DocumentTypeInfo> Classes);