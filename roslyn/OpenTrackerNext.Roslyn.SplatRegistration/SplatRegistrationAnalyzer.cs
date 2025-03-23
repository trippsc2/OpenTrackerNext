using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using OpenTrackerNext.Roslyn.Semantic;

namespace OpenTrackerNext.Roslyn.SplatRegistration;

/// <summary>
/// Analyzes the source code to ensure it is appropriate for <see cref="SplatRegistrationGenerator"/>.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SplatRegistrationAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for a ReactiveUI view to be registered without a valid constructor.
    /// </summary>
    public static DiagnosticDescriptor InvalidReactiveViewConstructor { get; } = new(
        "OTNS0001",
        "View must have public parameterless constructor",
        "The ReactiveUI view '{0}' does not have a valid constructor. It must have a public parameterless constructor. Alternatively, you can add the [SplatIgnoreReactiveView] attribute to not register the class.",
        "OpenTrackerNext.Generators.Splat",
        DiagnosticSeverity.Error,
        true);

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for a class marked with [Splat] that is a ReactiveUI view type.
    /// </summary>
    public static DiagnosticDescriptor SplatAttributeOnReactiveViewType { get; } = new(
        "OTNS0002",
        "SplatAttribute is ignored on ReactiveUI view type",
        "The class '{0}' is marked with [Splat] but is a ReactiveUI view type. The [Splat] attribute will be ignored.",
        "OpenTrackerNext.Generators.Splat",
        DiagnosticSeverity.Warning,
        true);

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for a class marked with [Splat] that is a ReactiveUI view type.
    /// </summary>
    public static DiagnosticDescriptor SplatGenericAttributeOnReactiveViewType { get; } = new(
        "OTNS0003",
        "SplatGenericAttribute is ignored on ReactiveUI view type",
        "The class '{0}' is marked with [SplatGeneric] but is a ReactiveUI view type. The [SplatGeneric] attribute will be ignored.",
        "OpenTrackerNext.Generators.Splat",
        DiagnosticSeverity.Warning,
        true);

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for a class marked with [SplatIgnoreReactiveView] that is a
    /// non-ReactiveUI view type.
    /// </summary>
    public static DiagnosticDescriptor SplatIgnoreReactiveViewAttributeOnNonReactiveView { get; } = new(
        "OTNS0004",
        "SplatAttribute cannot be used on a non-ReactiveUI view type",
        "The class '{0}' is marked with [SplatIgnoreReactiveView] but is not a ReactiveUI view type.",
        "OpenTrackerNext.Generators.Splat",
        DiagnosticSeverity.Warning,
        true);

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for a class marked with [Splat] with multiple constructors and none
    /// marked with [SplatConstructor].
    /// </summary>
    public static DiagnosticDescriptor MissingConstructorMarking { get; } = new(
        "OTNS0005",
        "Constructor must be marked with [SplatConstructor]",
        "A constructor of the class '{0}' is missing the [SplatConstructor] attribute. This is required for the constructor to be used for dependency injection if the type has multiple constructors.",
        "OpenTrackerNext.Generators.Splat",
        DiagnosticSeverity.Error,
        true);

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for a class marked with [Splat] that is a generic type.
    /// </summary>
    public static DiagnosticDescriptor SplatAttributeOnAbstractType { get; } = new(
        "OTNS0006",
        "SplatAttribute cannot be used on a abstract type",
        "The class '{0}' is marked with [Splat] but is an abstract type.",
        "OpenTrackerNext.Generators.Splat",
        DiagnosticSeverity.Error,
        true);

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for a class marked with [Splat] that is a generic type.
    /// </summary>
    public static DiagnosticDescriptor SplatAttributeOnGenericType { get; } = new(
        "OTNS0007",
        "SplatAttribute cannot be used on a generic type",
        "The class '{0}' is marked with [Splat] but is a generic type. Use the [SplatGeneric] attribute instead.",
        "OpenTrackerNext.Generators.Splat",
        DiagnosticSeverity.Error,
        true);

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for a class marked with [SplatGeneric] that is a generic type.
    /// </summary>
    public static DiagnosticDescriptor SplatGenericAttributeOnAbstractType { get; } = new(
        "OTNS0008",
        "SplatGenericAttribute cannot be used on a abstract type",
        "The class '{0}' is marked with [SplatGeneric] but is an abstract type.",
        "OpenTrackerNext.Generators.Splat",
        DiagnosticSeverity.Error,
        true);

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for a class marked with [SplatGeneric] that is a generic type.
    /// </summary>
    public static DiagnosticDescriptor SplatGenericAttributeOnNonGenericType { get; } = new(
        "OTNS0009",
        "SplatGenericAttribute cannot be used on a generic type",
        "The class '{0}' is marked with [SplatGeneric] but is a non-generic type. Use the [Splat] attribute instead.",
        "OpenTrackerNext.Generators.Splat",
        DiagnosticSeverity.Error,
        true);

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for a class marked with [Splat] with a RegisterAsType that is not
    /// valid.
    /// </summary>
    public static DiagnosticDescriptor RegisterAsTypeIsUnboundGeneric { get; } = new(
        "OTNS0010",
        "RegisterAsType cannot be an unbound generic type",
        "The RegisterAsType '{1}' cannot be an unbound generic type. Please provide type arguments.",
        "OpenTrackerNext.Generators.Splat",
        DiagnosticSeverity.Error,
        true);

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for a class marked with [Splat] with a RegisterAsType that is not
    /// valid.
    /// </summary>
    public static DiagnosticDescriptor ConcreteTypeCannotBeAssignedToRegisterAsType { get; } = new(
        "OTNS0011",
        "RegisterAsType cannot be assigned to concrete type",
        "The RegisterAsType '{0}' cannot be assigned to class '{1}'.",
        "OpenTrackerNext.Generators.Splat",
        DiagnosticSeverity.Error,
        true);
    
    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for a class marked with [SplatSingleInstance] but not [Splat] or [SplatGeneric].
    /// </summary>
    public static DiagnosticDescriptor SplatSingleInstanceAttributeOnUnmarkedType { get; } = new(
        "OTNS0012",
        "Class marked with [SplatSingleInstance] should also be marked with [Splat] or [SplatGeneric]",
        "The class '{0}' is marked with [SplatSingleInstance] without being marked with [Splat] or [SplatGeneric].",
        "OpenTrackerNext.Generators.Splat",
        DiagnosticSeverity.Warning,
        true);
    
    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for a class marked with [SplatGeneric] with a ConcreteType that is not
    /// the same generic as the declared type.
    /// </summary>
    public static DiagnosticDescriptor SplatGenericConcreteTypeIsNotValid { get; } = new(
        "OTNS0013",
        "Concrete type must be the same generic as the declared type",
        "The class '{0}' is marked with [SplatGeneric] and ConcreteType '{1}' is not derived from the declared generic type.",
        "OpenTrackerNext.Generators.Splat",
        DiagnosticSeverity.Error,
        true);

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for a class marked with [Splat] or [SplatGeneric] with a
    /// RegisterAsType that is a delegate with an invalid return type.
    /// </summary>
    public static DiagnosticDescriptor RegisterAsTypeIsDelegateWithInvalidReturnType { get; } = new(
        "OTNS0014",
        "RegisterAsType delegate has invalid return type",
        "The delegate '{0}' is assigned to RegisterAsType. This delegate does not have a valid return type.",
        "OpenTrackerNext.Generators.Splat",
        DiagnosticSeverity.Error,
        true);

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> for a class marked with [Splat] or [SplatGeneric] with a
    /// RegisterAsType that is the same as the concrete type.
    /// </summary>
    public static DiagnosticDescriptor RegisterAsTypeIsSameAsConcreteType { get; } = new(
        "OTNS0015",
        "RegisterAsType is the same as ConcreteType",
        "The RegisterAsType '{0}' is the same as the ConcreteType '{1}'. This is the default behavior and does not need to be specified.",
        "OpenTrackerNext.Generators.Splat",
        DiagnosticSeverity.Warning,
        true);

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        [
            InvalidReactiveViewConstructor,
            SplatAttributeOnReactiveViewType,
            SplatGenericAttributeOnReactiveViewType,
            SplatIgnoreReactiveViewAttributeOnNonReactiveView,
            MissingConstructorMarking,
            SplatAttributeOnAbstractType,
            SplatAttributeOnGenericType,
            SplatGenericAttributeOnAbstractType,
            SplatGenericAttributeOnNonGenericType,
            RegisterAsTypeIsUnboundGeneric,
            ConcreteTypeCannotBeAssignedToRegisterAsType,
            SplatSingleInstanceAttributeOnUnmarkedType,
            SplatGenericConcreteTypeIsNotValid,
            RegisterAsTypeIsDelegateWithInvalidReturnType,
            RegisterAsTypeIsSameAsConcreteType
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
        var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;
        
        if (namedTypeSymbol.ImplementsInterfaceNamed(
                out _,
                "IViewFor",
                "ReactiveUI"))
        {
            AnalyzeReactiveViewType(context, namedTypeSymbol);
            return;
        }
        
        if (namedTypeSymbol.HasAttribute(
                out var splatAttribute,
                "SplatAttribute",
                "OpenTrackerNext.SplatRegistration"))
        {
            AnalyzeSplatMarkedType(context, namedTypeSymbol, splatAttribute);
        }

        if (namedTypeSymbol.HasAttribute(
                out var splatGenericAttribute,
                "SplatGenericAttribute",
                "OpenTrackerNext.SplatRegistration"))
        {
            AnalyzeSplatGenericMarkedType(context, namedTypeSymbol, splatGenericAttribute);
        }

        if (namedTypeSymbol.HasAttribute(
                out _,
                "SplatIgnoreReactiveViewAttribute",
                "OpenTrackerNext.SplatRegistration"))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    SplatIgnoreReactiveViewAttributeOnNonReactiveView,
                    namedTypeSymbol.Locations[0],
                    namedTypeSymbol.Name));
        }

        if (namedTypeSymbol.HasAttribute(
                out _,
                "SplatSingleInstanceAttribute",
                "OpenTrackerNext.SplatRegistration") &&
            splatAttribute is null &&
            splatGenericAttribute is null)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    SplatSingleInstanceAttributeOnUnmarkedType,
                    namedTypeSymbol.Locations[0],
                    namedTypeSymbol.Name));
        }
    }
    
    private static void AnalyzeReactiveViewType(
        SymbolAnalysisContext context,
        INamedTypeSymbol namedTypeSymbol)
    {
        if (namedTypeSymbol.HasAttribute(
                out _,
                "SplatIgnoreReactiveViewAttribute",
                "OpenTrackerNext.SplatRegistration"))
        {
            return;
        }

        if (namedTypeSymbol.HasAttribute(
                out _,
                "SplatAttribute",
                "OpenTrackerNext.SplatRegistration"))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    SplatAttributeOnReactiveViewType,
                    namedTypeSymbol.Locations[0],
                    namedTypeSymbol.Name));
        }
        
        if (namedTypeSymbol.HasAttribute(
                out _,
                "SplatGenericAttribute",
                "OpenTrackerNext.SplatRegistration"))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    SplatGenericAttributeOnReactiveViewType,
                    namedTypeSymbol.Locations[0],
                    namedTypeSymbol.Name));
        }

        if (namedTypeSymbol.Constructors.Length == 0)
        {
            return;
        }
        
        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach (var constructor in namedTypeSymbol.Constructors)
        {
            if (constructor.Parameters.Length == 0)
            {
                return;
            }
        }

        context.ReportDiagnostic(
            Diagnostic.Create(
                InvalidReactiveViewConstructor,
                namedTypeSymbol.Locations[0],
                namedTypeSymbol.Name));
    }
    
    private static void AnalyzeSplatMarkedType(
        SymbolAnalysisContext context,
        INamedTypeSymbol namedTypeSymbol,
        AttributeData splatAttribute)
    {
        AnalyzeMarkedTypeConstructors(context, namedTypeSymbol);
        AnalyzeNonGenericConcreteType(context, namedTypeSymbol);
        
        INamedTypeSymbol? registerAsTypeSymbol = null;
        
        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach (var namedArgument in splatAttribute.NamedArguments)
        {
            if (namedArgument.Key != "RegisterAsType")
            {
                continue;
            }
            
            registerAsTypeSymbol = namedArgument.Value.Value as INamedTypeSymbol;
            break;
        }

        if (registerAsTypeSymbol is null)
        {
            return;
        }

        var isDelegate = false;
        if (registerAsTypeSymbol.TypeKind == TypeKind.Delegate)
        {
            isDelegate = true;
            var delegateSymbol = registerAsTypeSymbol;
            registerAsTypeSymbol = delegateSymbol.DelegateInvokeMethod?.ReturnType as INamedTypeSymbol;
            
            if (registerAsTypeSymbol is null)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        RegisterAsTypeIsDelegateWithInvalidReturnType,
                        namedTypeSymbol.Locations[0],
                        delegateSymbol.Name));
                return;
            }
        }
        
        AnalyzeRegisterAsType(context, namedTypeSymbol, registerAsTypeSymbol, isDelegate);
    }

    private static void AnalyzeSplatGenericMarkedType(
        SymbolAnalysisContext context,
        INamedTypeSymbol namedTypeSymbol,
        AttributeData splatGenericAttribute)
    {
        AnalyzeMarkedTypeConstructors(context, namedTypeSymbol);

        if (!namedTypeSymbol.IsGenericType)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    SplatGenericAttributeOnNonGenericType,
                    namedTypeSymbol.Locations[0],
                    namedTypeSymbol.Name));
            return;
        }
        
        INamedTypeSymbol? concreteTypeSymbol = null;
        INamedTypeSymbol? registerAsTypeSymbol = null;
        foreach (var namedArgument in splatGenericAttribute.NamedArguments)
        {
            switch (namedArgument.Key)
            {
                case "ConcreteType":
                    concreteTypeSymbol = namedArgument.Value.Value as INamedTypeSymbol;
                    break;
                case "RegisterAsType":
                    registerAsTypeSymbol = namedArgument.Value.Value as INamedTypeSymbol;
                    break;
            }
        }

        if (concreteTypeSymbol is null)
        {
            return;
        }
        
        var unboundNamedTypeSymbol = namedTypeSymbol.ConstructUnboundGenericType();
        var unboundConcreteTypeSymbol = concreteTypeSymbol.ConstructUnboundGenericType();
        
        if (!SymbolEqualityComparer.Default
                .Equals(unboundNamedTypeSymbol, unboundConcreteTypeSymbol))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    SplatGenericConcreteTypeIsNotValid,
                    namedTypeSymbol.Locations[0],
                    namedTypeSymbol.Name,
                    concreteTypeSymbol.Name));
        }
        
        AnalyzeGenericConcreteType(context, concreteTypeSymbol);

        if (registerAsTypeSymbol is null)
        {
            return;
        }

        var isDelegate = false;
        if (registerAsTypeSymbol.TypeKind == TypeKind.Delegate)
        {
            isDelegate = true;
            var delegateSymbol = registerAsTypeSymbol;
            registerAsTypeSymbol = delegateSymbol.DelegateInvokeMethod?.ReturnType as INamedTypeSymbol;
            
            if (registerAsTypeSymbol is null)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        RegisterAsTypeIsDelegateWithInvalidReturnType,
                        namedTypeSymbol.Locations[0],
                        delegateSymbol.Name));
                return;
            }

            if (SymbolEqualityComparer.Default.Equals(namedTypeSymbol, registerAsTypeSymbol))
            {
                return;
            }
        }
        
        AnalyzeRegisterAsType(context, concreteTypeSymbol, registerAsTypeSymbol, isDelegate);
    }
    
    private static void AnalyzeMarkedTypeConstructors(SymbolAnalysisContext context, INamedTypeSymbol namedTypeSymbol)
    {
        switch (namedTypeSymbol.Constructors.Length)
        {
            case 0:
            case 1:
                return;
        }

        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach (var constructorSymbol in namedTypeSymbol.Constructors)
        {
            if (constructorSymbol.HasAttribute(
                    out _,
                    "SplatConstructorAttribute",
                    "OpenTrackerNext.SplatRegistration"))
            {
                return;
            }
        }
        
        context.ReportDiagnostic(
            Diagnostic.Create(
                MissingConstructorMarking,
                namedTypeSymbol.Locations[0],
                namedTypeSymbol.Name));
    }
    
    private static void AnalyzeNonGenericConcreteType(SymbolAnalysisContext context, INamedTypeSymbol namedTypeSymbol)
    {
        if (namedTypeSymbol.IsGenericType)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    SplatAttributeOnGenericType,
                    namedTypeSymbol.Locations[0],
                    namedTypeSymbol.Name));
        }

        if (namedTypeSymbol.IsAbstract)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    SplatAttributeOnAbstractType,
                    namedTypeSymbol.Locations[0],
                    namedTypeSymbol.Name));
        }
    }

    private static void AnalyzeGenericConcreteType(SymbolAnalysisContext context, INamedTypeSymbol namedTypeSymbol)
    {
        if (!namedTypeSymbol.IsGenericType)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    SplatGenericAttributeOnNonGenericType,
                    namedTypeSymbol.Locations[0],
                    namedTypeSymbol.Name));
        }

        if (namedTypeSymbol.IsAbstract)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    SplatGenericAttributeOnAbstractType,
                    namedTypeSymbol.Locations[0],
                    namedTypeSymbol.Name));
        }
    }

    private static void AnalyzeRegisterAsType(
        SymbolAnalysisContext context,
        INamedTypeSymbol concreteTypeSymbol,
        INamedTypeSymbol registerAsTypeSymbol,
        bool isDelegate)
    {
        if (registerAsTypeSymbol.IsUnboundGenericType)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    RegisterAsTypeIsUnboundGeneric,
                    concreteTypeSymbol.Locations[0],
                    concreteTypeSymbol.Name,
                    registerAsTypeSymbol.Name));
            return;
        }

        if (SymbolEqualityComparer.Default.Equals(concreteTypeSymbol, registerAsTypeSymbol))
        {
            if (!isDelegate)
            {
                context.ReportDiagnostic(
                Diagnostic.Create(
                    RegisterAsTypeIsSameAsConcreteType,
                    concreteTypeSymbol.Locations[0],
                    registerAsTypeSymbol.Name,
                    concreteTypeSymbol.Name));
            }
            
            return;
        }

        if (concreteTypeSymbol.InheritsFromOrImplements(registerAsTypeSymbol))
        {
            return;
        }
        
        context.ReportDiagnostic(
            Diagnostic.Create(
                ConcreteTypeCannotBeAssignedToRegisterAsType,
                concreteTypeSymbol.Locations[0],
                concreteTypeSymbol.Name,
                registerAsTypeSymbol.Name));
    }
}