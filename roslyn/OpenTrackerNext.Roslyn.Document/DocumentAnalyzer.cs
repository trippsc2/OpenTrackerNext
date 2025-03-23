using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using OpenTrackerNext.Roslyn.Semantic;
using OpenTrackerNext.Roslyn.Syntactic;

namespace OpenTrackerNext.Roslyn.Document;

/// <summary>
/// Analyzes the source code to ensure it is appropriate for <see cref="DocumentGenerator"/>.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DocumentAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for a class marked with [MakeEqual] that is a non-partial type.
    /// </summary>
    public static DiagnosticDescriptor DocumentAttributeOnNonPartialType { get; } = new(
        "OTNM0001",
        "DocumentAttribute cannot be used on a non-partial type",
        "The class '{0}' is marked with [MakeEqual] but is a non-partial type.",
        "OpenTrackerNext.Generators.MakeEqual",
        DiagnosticSeverity.Error,
        true);
    
    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for a class marked with [MakeEqual] that is a static type.
    /// </summary>
    public static DiagnosticDescriptor DocumentAttributeOnStaticType { get; } = new(
        "OTNM0002",
        "DocumentAttribute cannot be used on a static type",
        "The class '{0}' is marked with [MakeEqual] but is a static type.",
        "OpenTrackerNext.Generators.MakeEqual",
        DiagnosticSeverity.Error,
        true);
    
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
    [
        DocumentAttributeOnNonPartialType,
        DocumentAttributeOnStaticType
    ];

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        
        context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
    }

    private static void AnalyzeNamedType(SymbolAnalysisContext context)
    {
        if (!context.Symbol
                .HasAttribute(
                    out _,
                    "DocumentAttribute",
                    "OpenTrackerNext.Document"))
        {
            return;
        }
        
        foreach (var syntaxReference in context.Symbol.DeclaringSyntaxReferences)
        {
            if (syntaxReference.GetSyntax() is not ClassDeclarationSyntax classDeclaration)
            {
                continue;
            }
            
            AnalyzeClassDeclaration(context, classDeclaration);
        }
    }

    private static void AnalyzeClassDeclaration(SymbolAnalysisContext context, TypeDeclarationSyntax classDeclaration)
    {
        if (!classDeclaration.IsPartial())
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    DocumentAttributeOnNonPartialType,
                    classDeclaration.Identifier.GetLocation(),
                    classDeclaration.Identifier.Text));
        }
        
        if (!classDeclaration.IsNotStatic())
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    DocumentAttributeOnStaticType,
                    classDeclaration.Identifier.GetLocation(),
                    classDeclaration.Identifier.Text));
        }
    }
}