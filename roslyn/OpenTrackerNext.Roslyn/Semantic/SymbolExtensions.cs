using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace OpenTrackerNext.Roslyn.Semantic;

/// <summary>
/// Extends <see cref="ISymbol"/> with additional methods.
/// </summary>
public static class SymbolExtensions
{
    /// <summary>
    /// Returns the fully qualified display string for the symbol.
    /// </summary>
    /// <param name="symbol">
    /// The <see cref="ISymbol"/> to be evaluated.
    /// </param>
    /// <returns>
    /// A <see cref="string"/> representing the fully qualified display string for the symbol.
    /// </returns>
    public static string ToFullyQualifiedString(this ISymbol symbol)
    {
        return symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
    }

    /// <summary>
    /// Returns whether the symbol has the specified attribute.
    /// </summary>
    /// <param name="symbol">
    /// The <see cref="ISymbol"/> to be evaluated.
    /// </param>
    /// <param name="attribute">
    /// An output of <see cref="AttributeData"/> representing the matching attribute.
    /// </param>
    /// <param name="attributeName">
    /// A <see cref="string"/> representing the name of the attribute.
    /// </param>
    /// <param name="attributeNamespace">
    /// A <see cref="string"/> representing the namespace of the attribute.
    /// </param>
    /// <returns>
    /// A <see cref="bool"/> representing whether the symbol has the specified attribute.
    /// </returns>
    public static bool HasAttribute(
        this ISymbol symbol,
        [NotNullWhen(true)] out AttributeData? attribute,
        string attributeName,
        string? attributeNamespace = null)
    {
        if (attributeNamespace is null)
        {
            attribute = symbol.GetAttributes()
                .FirstOrDefault(attribute => attribute.AttributeClass?.Name == attributeName);
            
            return attribute is not null;
        }
        
        attribute = symbol.GetAttributes()
            .FirstOrDefault(
                attribute =>
                    attribute.AttributeClass?.Name == attributeName &&
                    attribute.AttributeClass?.ContainingNamespace?.ToString() == attributeNamespace);

        return attribute is not null;
    }
    
    /// <summary>
    /// Returns whether the symbol is not a partial declaration.
    /// </summary>
    /// <param name="symbol">
    /// The <see cref="ISymbol"/> to be evaluated.
    /// </param>
    /// <returns>
    /// A <see cref="bool"/> representing whether the symbol is not a partial declaration.
    /// </returns>
    public static bool IsNotPartial(this ISymbol symbol)
    {
        foreach (var syntaxReference in symbol.DeclaringSyntaxReferences)
        {
            if (syntaxReference.GetSyntax() is TypeDeclarationSyntax typeDeclaration &&
                typeDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                return false;
            }
        }

        return true;
    }
    
    /// <summary>
    /// Returns the attributes with the specified name.
    /// </summary>
    /// <param name="symbol">
    /// The <see cref="ISymbol"/> to be evaluated.
    /// </param>
    /// <param name="attributeName">
    /// A <see cref="string"/> representing the name of the attribute.
    /// </param>
    /// <param name="attributeNamespace">
    /// A <see cref="string"/> representing the namespace of the attribute.
    /// </param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of <see cref="AttributeData"/> representing the attributes with the
    /// specified name.
    /// </returns>
    public static IEnumerable<AttributeData> GetAttributesWithName(
        this ISymbol symbol,
        string attributeName,
        string? attributeNamespace = null)
    {
        if (attributeNamespace is null)
        {
            return symbol
                .GetAttributes()
                .Where(attribute => attribute.AttributeClass?.Name == attributeName);
        }
        
        return symbol
            .GetAttributes()
            .Where(
                attribute =>
                    attribute.AttributeClass?.Name == attributeName &&
                    attribute.AttributeClass?.ContainingNamespace?.ToString() == attributeNamespace);
    }
}