using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using OpenTrackerNext.Roslyn.Semantic;

namespace OpenTrackerNext.Roslyn.ServiceCollection;

/// <summary>
/// The analyzer for the <see cref="ServiceCollectionGenerator"/>.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ServiceCollectionAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for a service collection that is not partial.
    /// </summary>
    public static DiagnosticDescriptor ServiceCollectionMustBePartial { get; } = new(
        "OTSC0001",
        "Service collection must be partial",
        "Service collection '{0}' must be partial",
        "OpenTrackerNext.Generators.ServiceCollection",
        DiagnosticSeverity.Error,
        true);
    
    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for a service collection that is static.
    /// </summary>
    public static DiagnosticDescriptor ServiceCollectionMustNotBeStatic { get; } = new(
        "OTSC0002",
        "Service collection must not be static",
        "Service collection '{0}' must not be static",
        "OpenTrackerNext.Generators.ServiceCollection",
        DiagnosticSeverity.Error,
        true);
    
    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for a service collection that does not implement the
    /// IServiceCollection{TService} interface.
    /// </summary>
    public static DiagnosticDescriptor ServiceCollectionMustImplementInterface { get; } = new(
        "OTSC0003",
        "Service collection must implement interface",
        "Service collection '{0}' must implement the IServiceCollection<TService> interface",
        "OpenTrackerNext.Generators.ServiceCollection",
        DiagnosticSeverity.Error,
        true);
    
    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for a service collection member that is static.
    /// </summary>
    public static DiagnosticDescriptor ServiceCollectionMemberMustNotBeStatic { get; } = new(
        "OTSC0004",
        "Service collection member must not be static",
        "Service collection member '{0}' must not be static",
        "OpenTrackerNext.Generators.ServiceCollection",
        DiagnosticSeverity.Error,
        true);

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for a service collection member that is marked with a non-service
    /// service collection type.
    /// </summary>
    public static DiagnosticDescriptor ServiceCollectionMemberMarkedWithNonServiceCollectionType { get; } = new(
        "OTSC0005",
        "Service collection member marked with non-service collection type",
        "Service collection member '{0}' is marked with a non-service collection type '{1}'",
        "OpenTrackerNext.Generators.ServiceCollection",
        DiagnosticSeverity.Error,
        true);
    
    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for a service collection member that does not belong in the service
    /// collection type.
    /// </summary>
    public static DiagnosticDescriptor ServiceCollectionMemberMustBelongInServiceCollectionType { get; } = new(
        "OTSC0006",
        "Service collection member must belong in service collection type",
        "Service collection member '{0}' must belong in service collection type '{1}' which accepts only '{2}' objects.",
        "OpenTrackerNext.Generators.ServiceCollection",
        DiagnosticSeverity.Error,
        true);
    
    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for a service collection member that is marked as both a service
    /// collection and a service collection member.
    /// </summary>
    public static DiagnosticDescriptor TypeCannotHaveBothServiceCollectionAndServiceCollectionMemberMarkings { get; } =
        new(
            "OTSC0007",
            "Type cannot have both service collection and service collection member markings",
            "Type '{0}' cannot have both service collection and service collection member markings",
            "OpenTrackerNext.Generators.ServiceCollection",
            DiagnosticSeverity.Error,
            true);
    
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        [
            ServiceCollectionMustBePartial,
            ServiceCollectionMustNotBeStatic,
            ServiceCollectionMustImplementInterface,
            ServiceCollectionMemberMustNotBeStatic,
            ServiceCollectionMemberMarkedWithNonServiceCollectionType,
            ServiceCollectionMemberMustBelongInServiceCollectionType,
            TypeCannotHaveBothServiceCollectionAndServiceCollectionMemberMarkings
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
        if (context.Symbol is not INamedTypeSymbol namedTypeSymbol)
        {
            return;
        }

        if (namedTypeSymbol
                .HasAttribute(
                    out _,
                    "ServiceCollectionAttribute",
                    "OpenTrackerNext.ServiceCollection"))
        {
            AnalyzeServiceCollection(context, namedTypeSymbol);
            return;
        }
        
        if (namedTypeSymbol
            .HasAttribute(
                out var attributeData,
                "ServiceCollectionMemberAttribute",
                "OpenTrackerNext.ServiceCollection"))
        {
            AnalyzeServiceCollectionMember(context, namedTypeSymbol, attributeData);
        }
    }

    private static void AnalyzeServiceCollection(SymbolAnalysisContext context, ITypeSymbol typeSymbol)
    {
        if (typeSymbol
            .HasAttribute(
                out _,
                "ServiceCollectionMemberAttribute",
                "OpenTrackerNext.ServiceCollection"))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    TypeCannotHaveBothServiceCollectionAndServiceCollectionMemberMarkings,
                    typeSymbol.Locations[0],
                    typeSymbol.Name));
        }
        
        if (typeSymbol.IsNotPartial())
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    ServiceCollectionMustBePartial,
                    typeSymbol.Locations[0],
                    typeSymbol.Name));
        }
        
        if (typeSymbol.IsStatic)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    ServiceCollectionMustNotBeStatic,
                    typeSymbol.Locations[0],
                    typeSymbol.Name));
        }
        
        if (!typeSymbol.ImplementsInterfaceNamed(
                out _,
                "IServiceCollection",
                "OpenTrackerNext.Core.Utils",
                1))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    ServiceCollectionMustImplementInterface,
                    typeSymbol.Locations[0],
                    typeSymbol.Name));
        }
    }

    private static void AnalyzeServiceCollectionMember(
        SymbolAnalysisContext context,
        INamedTypeSymbol namedTypeSymbol,
        AttributeData attributeData)
    {
        if (namedTypeSymbol
            .HasAttribute(
                out _,
                "ServiceCollectionAttribute",
                "OpenTrackerNext.ServiceCollection"))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    TypeCannotHaveBothServiceCollectionAndServiceCollectionMemberMarkings,
                    namedTypeSymbol.Locations[0],
                    namedTypeSymbol.Name));
        }
        
        if (namedTypeSymbol.IsStatic)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    ServiceCollectionMemberMustNotBeStatic,
                    namedTypeSymbol.Locations[0],
                    namedTypeSymbol.Name));
        }
        
        if (attributeData.AttributeClass?.TypeArguments[0] is not INamedTypeSymbol serviceCollectionTypeSymbol)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    ServiceCollectionMemberMarkedWithNonServiceCollectionType,
                    namedTypeSymbol.Locations[0],
                    namedTypeSymbol.Name,
                    null));
            return;
        }
        
        if (!serviceCollectionTypeSymbol.ImplementsInterfaceNamed(
                out var serviceCollectionInterfaceSymbol,
                "IServiceCollection",
                "OpenTrackerNext.Core.Utils",
                1))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    ServiceCollectionMemberMarkedWithNonServiceCollectionType,
                    namedTypeSymbol.Locations[0],
                    namedTypeSymbol.Name,
                    serviceCollectionTypeSymbol.Name));
            return;
        }

        var serviceCollectionMemberTypeSymbol = serviceCollectionInterfaceSymbol.TypeArguments[0];
        
        if (!SymbolEqualityComparer.Default.Equals(serviceCollectionMemberTypeSymbol, namedTypeSymbol) &&
            !namedTypeSymbol.InheritsFromOrImplements(serviceCollectionMemberTypeSymbol))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    ServiceCollectionMemberMustBelongInServiceCollectionType,
                    namedTypeSymbol.Locations[0],
                    namedTypeSymbol.Name,
                    serviceCollectionTypeSymbol.Name,
                    serviceCollectionInterfaceSymbol.TypeArguments[0].Name));
        }
    }
}