using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace OpenTrackerNext.Roslyn.Syntactic;

/// <summary>
/// Extends <see cref="TypeDeclarationSyntax"/> with additional methods.
/// </summary>
public static class TypeDeclarationSyntaxExtensions
{
    /// <summary>
    /// Returns whether the type declaration is partial.
    /// </summary>
    /// <param name="typeDeclaration">
    /// The <see cref="TypeDeclarationSyntax"/> to be evaluated.
    /// </param>
    /// <returns>
    /// A <see cref="bool"/> representing whether the type declaration is partial.
    /// </returns>
    public static bool IsPartial(this TypeDeclarationSyntax typeDeclaration)
    {
        return typeDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword);
    }
    
    /// <summary>
    /// Returns whether the type declaration is not partial.
    /// </summary>
    /// <param name="typeDeclaration">
    ///     The <see cref="TypeDeclarationSyntax"/> to be evaluated.
    /// </param>
    /// <returns>
    ///     A <see cref="bool"/> representing whether the type declaration is not partial.
    /// </returns>
    public static bool IsNotPartial(this TypeDeclarationSyntax typeDeclaration)
    {
        return !typeDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword);
    }
    
    /// <summary>
    /// Returns whether the type declaration is static.
    /// </summary>
    /// <param name="typeDeclaration">
    ///     The <see cref="TypeDeclarationSyntax"/> to be evaluated.
    /// </param>
    /// <returns>
    ///     A <see cref="bool"/> representing whether the type declaration is static.
    /// </returns>
    public static bool IsStatic(this TypeDeclarationSyntax typeDeclaration)
    {
        return typeDeclaration.Modifiers.Any(SyntaxKind.StaticKeyword);
    }

    /// <summary>
    /// Returns whether the type declaration is not static.
    /// </summary>
    /// <param name="typeDeclaration">
    /// The <see cref="TypeDeclarationSyntax"/> to be evaluated.
    /// </param>
    /// <returns>
    /// A <see cref="bool"/> representing whether the type declaration is not static.
    /// </returns>
    public static bool IsNotStatic(this TypeDeclarationSyntax typeDeclaration)
    {
        return !typeDeclaration.Modifiers.Any(SyntaxKind.StaticKeyword);
    }

    /// <summary>
    /// Returns whether the type declaration is generic.
    /// </summary>
    /// <param name="typeDeclaration">
    /// The <see cref="TypeDeclarationSyntax"/> to be evaluated.
    /// </param>
    /// <returns>
    /// A <see cref="bool"/> representing whether the type declaration is generic.
    /// </returns>
    public static bool IsGeneric(this TypeDeclarationSyntax typeDeclaration)
    {
        return typeDeclaration.Arity > 0;
    }

    /// <summary>
    /// Returns whether the type declaration is not generic.
    /// </summary>
    /// <param name="typeDeclaration">
    /// The <see cref="TypeDeclarationSyntax"/> to be evaluated.
    /// </param>
    /// <returns>
    /// A <see cref="bool"/> representing whether the type declaration is not generic.
    /// </returns>
    public static bool IsNotGeneric(this TypeDeclarationSyntax typeDeclaration)
    {
        return typeDeclaration.Arity == 0;
    }
    
    /// <summary>
    /// Returns whether the type declaration is not abstract.
    /// </summary>
    /// <param name="typeDeclaration">
    /// The <see cref="TypeDeclarationSyntax"/> to be evaluated.
    /// </param>
    /// <returns>
    /// A <see cref="bool"/> representing whether the type declaration is not abstract.
    /// </returns>
    public static bool IsNotAbstract(this TypeDeclarationSyntax typeDeclaration)
    {
        return !typeDeclaration.Modifiers.Any(SyntaxKind.AbstractKeyword);
    }
}