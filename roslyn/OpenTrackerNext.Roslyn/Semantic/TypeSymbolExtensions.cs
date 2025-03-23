using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace OpenTrackerNext.Roslyn.Semantic;

/// <summary>
/// Extends <see cref="ITypeSymbol"/> with additional methods.
/// </summary>
public static class TypeSymbolExtensions
{
    /// <summary>
    /// Returns whether the type symbol implements the specified interface.
    /// </summary>
    /// <param name="typeSymbol">
    /// The <see cref="ITypeSymbol"/> to be evaluated.
    /// </param>
    /// <param name="interfaceSymbol">
    /// The <see cref="INamedTypeSymbol"/> representing the interface.
    /// If the interface is not implemented, this will be null.
    /// </param>
    /// <param name="interfaceName">
    /// A <see cref="string"/> representing the name of the interface.
    /// </param>
    /// <param name="interfaceNamespace">
    /// A <see cref="string"/> representing the namespace of the interface.
    /// </param>
    /// <param name="interfaceArity">
    /// An <see cref="int"/> representing the arity of the interface.
    /// </param>
    /// <returns>
    /// A <see cref="bool"/> representing whether the type symbol implements the specified interface.
    /// </returns>
    public static bool ImplementsInterfaceNamed(
        this ITypeSymbol typeSymbol,
        [NotNullWhen(true)] out INamedTypeSymbol? interfaceSymbol,
        string interfaceName,
        string? interfaceNamespace = null,
        int interfaceArity = 0)
    {
        if (interfaceNamespace is null)
        {
            interfaceSymbol = typeSymbol.AllInterfaces
                .FirstOrDefault(
                    @interface =>
                        @interface.Name == interfaceName &&
                        @interface.Arity == interfaceArity);
            
            return interfaceSymbol is not null;
        }
        
        interfaceSymbol = typeSymbol.AllInterfaces
            .FirstOrDefault(
                @interface =>
                    @interface.Name == interfaceName &&
                    @interface.ContainingNamespace?.ToString() == interfaceNamespace &&
                    @interface.Arity == interfaceArity);
        
        return interfaceSymbol is not null;
    }
        
    /// <summary>
    /// Returns whether the type symbol implements the interface.
    /// </summary>
    /// <param name="typeSymbol">
    /// The <see cref="ITypeSymbol"/> to be evaluated.
    /// </param>
    /// <param name="interfaceTypeSymbol">
    /// The <see cref="ISymbol"/> representing the interface type.
    /// </param>
    /// <returns>
    /// A <see cref="bool"/> representing whether the type symbol implements the specified interface.
    /// </returns>
    public static bool Implements(this ITypeSymbol typeSymbol, ISymbol interfaceTypeSymbol)
    {
        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach (var interfaceSymbol in typeSymbol.AllInterfaces)
        {
            if (interfaceSymbol.Equals(interfaceTypeSymbol, SymbolEqualityComparer.Default))
            {
                return true;
            }
        }
        
        return false;
    }

    /// <summary>
    /// Returns whether the type symbol inherits from the type.
    /// </summary>
    /// <param name="typeSymbol">
    /// The <see cref="ITypeSymbol"/> to be evaluated.
    /// </param>
    /// <param name="ancestorTypeSymbol">
    /// The <see cref="ISymbol"/> representing the ancestor type.
    /// </param>
    /// <returns>
    /// A <see cref="bool"/> representing whether the type symbol inherits from the specified type.
    /// </returns>
    public static bool InheritsFrom(this ITypeSymbol typeSymbol, ISymbol ancestorTypeSymbol)
    {
        var currentBaseTypeSymbol = typeSymbol.BaseType;

        while (currentBaseTypeSymbol is not null)
        {
            if (currentBaseTypeSymbol.Equals(ancestorTypeSymbol, SymbolEqualityComparer.Default))
            {
                return true;
            }

            currentBaseTypeSymbol = currentBaseTypeSymbol.BaseType;
        }

        return false;
    }

    /// <summary>
    /// Returns whether the type symbol inherits from the type.
    /// </summary>
    /// <param name="typeSymbol">
    /// The <see cref="ITypeSymbol"/> to be evaluated.
    /// </param>
    /// <param name="ancestorSymbol">
    /// The <see cref="INamedTypeSymbol"/> representing the ancestor.
    /// If the ancestor is not implemented, this will be null.
    /// </param>
    /// <param name="ancestorName">
    /// A <see cref="string"/> representing the name of the ancestor.
    /// </param>
    /// <param name="ancestorNamespace">
    /// A <see cref="string"/> representing the namespace of the ancestor.
    /// </param>
    /// <param name="ancestorArity">
    /// An <see cref="int"/> representing the arity of the ancestor.
    /// </param>
    /// <returns>
    /// A <see cref="bool"/> representing whether the type symbol inherits from the specified type.
    /// </returns>
    public static bool InheritsFromTypeNamed(
        this ITypeSymbol typeSymbol,
        [NotNullWhen(true)] out INamedTypeSymbol? ancestorSymbol,
        string ancestorName,
        string? ancestorNamespace = null,
        int ancestorArity = 0)
    {
        var currentBaseTypeSymbol = typeSymbol.BaseType;

        while (currentBaseTypeSymbol is not null)
        {
            if (ancestorNamespace is null)
            {
                if (currentBaseTypeSymbol.Name == ancestorName && currentBaseTypeSymbol.Arity == ancestorArity)
                {
                    ancestorSymbol = currentBaseTypeSymbol;
                    return true;
                }
            }

            if (currentBaseTypeSymbol.Name == ancestorName &&
                currentBaseTypeSymbol.ContainingNamespace?.ToString() == ancestorNamespace &&
                currentBaseTypeSymbol.Arity == ancestorArity)
            {
                ancestorSymbol = currentBaseTypeSymbol;
                return true;
            }
            
            currentBaseTypeSymbol = currentBaseTypeSymbol.BaseType;
        }

        ancestorSymbol = null;
        return false;
    }
}