using Microsoft.CodeAnalysis;

namespace OpenTrackerNext.Roslyn.Semantic;

/// <summary>
/// Extends <see cref="INamedTypeSymbol"/> with additional methods.
/// </summary>
public static class NamedTypeSymbolExtensions
{
    /// <summary>
    /// Returns whether the type symbol represents a type.
    /// </summary>
    /// <param name="namedTypeSymbol">
    /// The <see cref="INamedTypeSymbol"/> to be evaluated.
    /// </param>
    /// <param name="ancestorTypeSymbol">
    /// The <see cref="ITypeSymbol"/> representing the ancestor type.
    /// </param>
    /// <returns>
    /// A <see cref="bool"/> representing whether the type symbol represents the specified type.
    /// </returns>
    public static bool InheritsFromOrImplements(this INamedTypeSymbol namedTypeSymbol, ITypeSymbol ancestorTypeSymbol)
    {
        return ancestorTypeSymbol.TypeKind switch
        {
            TypeKind.Interface => namedTypeSymbol.Implements(ancestorTypeSymbol),
            TypeKind.Class => namedTypeSymbol.InheritsFrom(ancestorTypeSymbol),
            _ => false
        };
    }

    /// <summary>
    /// Returns whether the type symbol represents a type.
    /// </summary>
    /// <param name="namedTypeSymbol">
    /// The <see cref="INamedTypeSymbol"/> to be evaluated.
    /// </param>
    /// <param name="name">
    /// A <see cref="string"/> representing the name of the type.
    /// </param>
    /// <param name="namespace">
    /// A <see cref="string"/> representing the namespace of the type.
    /// </param>
    /// <param name="arity">
    /// An <see cref="int"/> representing the arity of the type.
    /// </param>
    /// <returns>
    /// A <see cref="bool"/> representing whether the type symbol represents the specified type.
    /// </returns>
    public static bool IsTypeWithName(
        this INamedTypeSymbol namedTypeSymbol,
        string name,
        string? @namespace = null,
        int arity = 0)
    {
        if (@namespace is null)
        {
            return namedTypeSymbol.Name == name &&
                   namedTypeSymbol.Arity == arity;
        }

        return namedTypeSymbol.Name == name &&
               namedTypeSymbol.ContainingNamespace?.ToString() == @namespace &&
               namedTypeSymbol.Arity == arity;
    }
}