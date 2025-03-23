using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using OpenTrackerNext.Roslyn.Semantic;

namespace OpenTrackerNext.Roslyn.Factories;

/// <summary>
/// Analyzes the abstract factory and specific factory classes for errors.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class FactoryAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for a class marked with [Factory] that is static.
    /// </summary>
    public static DiagnosticDescriptor SpecificFactoryIsStatic { get; } = new(
        "OTNF0001",
        "Specific factory class cannot be static",
        "The marked specific factory class '{0}' is static.",
        "OpenTrackerNext.Generators.Factories",
        DiagnosticSeverity.Error,
        true);

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for a class marked with [Factory] that is static.
    /// </summary>
    public static DiagnosticDescriptor SpecificFactoryIsNotPartial { get; } = new(
        "OTNF0002",
        "Specific factory class must be partial",
        "The marked specific factory class '{0}' is not partial.",
        "OpenTrackerNext.Generators.Factories",
        DiagnosticSeverity.Error,
        true);

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for a class marked with [Factory] that does not implement the
    /// ISpecificFactory interface.
    /// </summary>
    public static DiagnosticDescriptor SpecificFactoryDoesNotImplementInterface { get; } = new(
        "OTNF0003",
        "Specific factory class does not implement factory interface",
        "The marked specific factory class '{0}' does not implement the ISpecificFactory<TBaseInput, TOutput, TInput> interface.",
        "OpenTrackerNext.Generators.Factories",
        DiagnosticSeverity.Error,
        true);

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for a class marked with [AbstractFactory] that is static.
    /// </summary>
    public static DiagnosticDescriptor AbstractFactoryIsStatic { get; } = new(
        "OTNF0004",
        "Abstract factory class cannot be static",
        "The marked abstract factory class '{0}' is static.",
        "OpenTrackerNext.Generators.Factories",
        DiagnosticSeverity.Error,
        true);
    
    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for a class marked with [AbstractFactory] that is not partial.
    /// </summary>
    public static DiagnosticDescriptor AbstractFactoryIsNotPartial { get; } = new(
        "OTNF0005",
        "Abstract factory class must be partial",
        "The marked abstract factory class '{0}' is not partial.",
        "OpenTrackerNext.Generators.Factories",
        DiagnosticSeverity.Error,
        true);

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for a class marked with [AbstractFactory] that does not implement the
    /// IFactory interface.
    /// </summary>
    public static DiagnosticDescriptor AbstractFactoryDoesNotImplementInterface { get; } = new(
        "OTNF0006",
        "Abstract factory class does not implement factory interface",
        "The marked abstract factory class '{0}' does not implement the IFactory<TInput, TOutput> interface.",
        "OpenTrackerNext.Generators.Factories",
        DiagnosticSeverity.Error,
        true);

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for a class marked with [AbstractFactory] that has no specific
    /// factories associated with it.
    /// </summary>
    public static DiagnosticDescriptor AbstractFactoryHasSpecificFactoryAttribute { get; } = new(
        "OTNF0007",
        "Abstract factory class cannot have specific factory attribute",
        "The marked abstract factory class '{0}' has the [Factory] marker attribute.",
        "OpenTrackerNext.Generators.Factories",
        DiagnosticSeverity.Error,
        true);

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        [
            SpecificFactoryIsStatic,
            SpecificFactoryIsNotPartial,
            SpecificFactoryDoesNotImplementInterface,
            AbstractFactoryIsStatic,
            AbstractFactoryIsNotPartial,
            AbstractFactoryDoesNotImplementInterface,
            AbstractFactoryHasSpecificFactoryAttribute
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
        if (context.Symbol is not ITypeSymbol namedTypeSymbol)
        {
            return;
        }
        
        if (namedTypeSymbol
            .HasAttribute(
                out _,
                "AbstractFactoryAttribute",
                "OpenTrackerNext.Factories"))
        {
            AnalyzeAbstractFactory(context, namedTypeSymbol);
            return;
        }

        if (namedTypeSymbol
            .HasAttribute(
                out _,
                "FactoryAttribute",
                "OpenTrackerNext.Factories"))
        {
            AnalyzeSpecificFactory(context, namedTypeSymbol);
        }
    }

    private static void AnalyzeAbstractFactory(SymbolAnalysisContext context, ITypeSymbol typeSymbol)
    {
        if (typeSymbol.IsStatic)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    AbstractFactoryIsStatic,
                    context.Symbol.Locations[0],
                    context.Symbol.Name));
        }
        
        if (typeSymbol.IsNotPartial())
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    AbstractFactoryIsNotPartial,
                    context.Symbol.Locations[0],
                    context.Symbol.Name));
        }

        if (!typeSymbol
                .ImplementsInterfaceNamed(
                    out _,
                    "IFactory",
                    "OpenTrackerNext.Core.Factories",
                    2))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    AbstractFactoryDoesNotImplementInterface,
                    context.Symbol.Locations[0],
                    context.Symbol.Name));
        }
        
        if (typeSymbol
            .HasAttribute(
                out _,
                "FactoryAttribute",
                "OpenTrackerNext.Factories"))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    AbstractFactoryHasSpecificFactoryAttribute,
                    context.Symbol.Locations[0],
                    context.Symbol.Name));
        }
    }

    private static void AnalyzeSpecificFactory(SymbolAnalysisContext context, ITypeSymbol typeSymbol)
    {
        if (typeSymbol.IsStatic)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    SpecificFactoryIsStatic,
                    context.Symbol.Locations[0],
                    context.Symbol.Name));
        }

        if (typeSymbol.IsNotPartial())
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    SpecificFactoryIsNotPartial,
                    context.Symbol.Locations[0],
                    context.Symbol.Name));
        }

        if (!typeSymbol
                .ImplementsInterfaceNamed(
                    out _,
                    "ISpecificFactory",
                    "OpenTrackerNext.Core.Factories",
                    3))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    SpecificFactoryDoesNotImplementInterface,
                    context.Symbol.Locations[0],
                    context.Symbol.Name));
        }
    }
}