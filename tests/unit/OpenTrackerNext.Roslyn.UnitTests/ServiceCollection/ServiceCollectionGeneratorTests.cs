using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using OpenTrackerNext.Roslyn.ServiceCollection;
using Xunit;

namespace OpenTrackerNext.Roslyn.UnitTests.ServiceCollection;

[ExcludeFromCodeCoverage]
public sealed class ServiceCollectionGeneratorTests
{
    // lang=cs
    private const string InterfaceDefinition =
        """
        namespace OpenTrackerNext.Core.Utils
        {
            /// <summary>
            /// Represents a collection of services.
            /// </summary>
            /// <typeparam name="TService">
            /// The type of the services in the collection.
            /// </typeparam>
            public interface IServiceCollection<out TService>
                where TService : class
            {
                /// <summary>
                /// Gets a <see cref="global::System.Collections.Generic.IReadOnlyList{T}"/> of <typeparamref name="TService"/> representing all services in the collection.
                /// </summary>
                public global::System.Collections.Generic.IReadOnlyList<TService> All { get; }
            }
        }
        
        """;
    
    [Fact]
    public async Task ShouldNotGenerateCode_WhenServiceCollectionIsNotPartial()
    {
        // lang=cs
        const string source =
            $$"""
            #nullable enable
            
            using OpenTrackerNext.Core.Utils;
            using OpenTrackerNext.ServiceCollection;
            
            {{InterfaceDefinition}}
            
            namespace TestProject
            {
                public interface IService;
            
                [ServiceCollection]
                public class TestClass : IServiceCollection<IService>;
            }
            """;
        
        await GeneratorTest<ServiceCollectionGenerator>
            .VerifyGeneratorAsync(
                source,
                new DiagnosticResult("CS0535", DiagnosticSeverity.Error)
                    .WithSpan(30, 30, 30, 58)
                    .WithArguments(
                        "TestProject.TestClass",
                        "OpenTrackerNext.Core.Utils.IServiceCollection<TestProject.IService>.All"),
                (ServiceCollectionGenerator.ServiceCollectionAttributeHintName, ServiceCollectionGenerator.ServiceCollectionAttributeSource),
                (ServiceCollectionGenerator.ServiceCollectionMemberAttributeHintName, ServiceCollectionGenerator.ServiceCollectionMemberAttributeSource));
    }
    
    [Fact]
    public async Task ShouldNotGenerateCode_WhenServiceCollectionIsStatic()
    {
        // lang=cs
        const string source =
            $$"""
            #nullable enable

            using OpenTrackerNext.Core.Utils;
            using OpenTrackerNext.ServiceCollection;
            
            {{InterfaceDefinition}}

            namespace TestProject
            {
                public interface IService;

                [ServiceCollection]
                public static class TestClass;
            }
            """;
        
        await GeneratorTest<ServiceCollectionGenerator>
            .VerifyGeneratorAsync(
                source,
                (ServiceCollectionGenerator.ServiceCollectionAttributeHintName, ServiceCollectionGenerator.ServiceCollectionAttributeSource),
                (ServiceCollectionGenerator.ServiceCollectionMemberAttributeHintName, ServiceCollectionGenerator.ServiceCollectionMemberAttributeSource));
    }

    [Fact]
    public async Task ShouldNotGenerateCode_WhenServiceCollectionDoesNotImplementInterface()
    {
        // lang=cs
        const string source =
            $$"""
            #nullable enable

            using OpenTrackerNext.Core.Utils;
            using OpenTrackerNext.ServiceCollection;
            
            {{InterfaceDefinition}}

            namespace TestProject
            {
                public interface IService;

                [ServiceCollection]
                public partial class TestClass;
            }
            """;
        
        await GeneratorTest<ServiceCollectionGenerator>
            .VerifyGeneratorAsync(
                source,
                (ServiceCollectionGenerator.ServiceCollectionAttributeHintName, ServiceCollectionGenerator.ServiceCollectionAttributeSource),
                (ServiceCollectionGenerator.ServiceCollectionMemberAttributeHintName, ServiceCollectionGenerator.ServiceCollectionMemberAttributeSource));
    }
    
    [Fact]
    public async Task ShouldGenerateEmpty_WhenServiceCollectionMemberIsStatic()
    {
        // lang=cs
        const string source =
            $$"""
            #nullable enable

            using OpenTrackerNext.Core.Utils;
            using OpenTrackerNext.ServiceCollection;
            
            {{InterfaceDefinition}}

            namespace TestProject
            {
                public interface IService;
                
                [ServiceCollection]
                public partial class TestClass : IServiceCollection<IService>;
                
                [ServiceCollectionMember<TestClass>]
                public static class TestClassMembers;
            }
            """;
        
        // lang=cs
        const string expected =
            """
            // <auto-generated/>
            #nullable enable
            
            namespace TestProject;
            
            partial class TestClass
            {
                public global::System.Collections.Generic.IReadOnlyList<global::TestProject.IService> All { get; } = new global::System.Collections.Generic.List<global::TestProject.IService>
                {
                };
            }
            """;
        
        await GeneratorTest<ServiceCollectionGenerator>
            .VerifyGeneratorAsync(
                source,
                (ServiceCollectionGenerator.ServiceCollectionAttributeHintName, ServiceCollectionGenerator.ServiceCollectionAttributeSource),
                (ServiceCollectionGenerator.ServiceCollectionMemberAttributeHintName, ServiceCollectionGenerator.ServiceCollectionMemberAttributeSource),
                ("TestProject.TestClass.g.cs", expected));
    }
    
    [Fact]
    public async Task ShouldGenerateEmpty_WhenServiceCollectionMemberTypeArgumentIsInvalid()
    {
        // lang=cs
        const string source =
            $$"""
            #nullable enable

            using OpenTrackerNext.Core.Utils;
            using OpenTrackerNext.ServiceCollection;
            
            {{InterfaceDefinition}}

            namespace TestProject
            {
                public interface IService;

                [ServiceCollection]
                public partial class TestClass : IServiceCollection<IService>;
            
                public class TestClass2;

                [ServiceCollectionMember<TestClass2>]
                public class TestClassMember : IService;
            }
            """;
        
        // lang=cs
        const string expected =
            """
            // <auto-generated/>
            #nullable enable
            
            namespace TestProject;
            
            partial class TestClass
            {
                public global::System.Collections.Generic.IReadOnlyList<global::TestProject.IService> All { get; } = new global::System.Collections.Generic.List<global::TestProject.IService>
                {
                };
            }
            """;
        
        await GeneratorTest<ServiceCollectionGenerator>
            .VerifyGeneratorAsync(
                source,
                (ServiceCollectionGenerator.ServiceCollectionAttributeHintName, ServiceCollectionGenerator.ServiceCollectionAttributeSource),
                (ServiceCollectionGenerator.ServiceCollectionMemberAttributeHintName, ServiceCollectionGenerator.ServiceCollectionMemberAttributeSource),
                ("TestProject.TestClass.g.cs", expected));
    }
    
    [Fact]
    public async Task ShouldGenerateEmpty_WhenServiceCollectionMemberTypeArgumentIsNotInServiceCollection()
    {
        // lang=cs
        const string source =
            $$"""
            #nullable enable

            using OpenTrackerNext.Core.Utils;
            using OpenTrackerNext.ServiceCollection;
            
            {{InterfaceDefinition}}

            namespace TestProject
            {
                public interface IService;
                
                [ServiceCollection]
                public partial class TestClass : IServiceCollection<IService>;
                
                [ServiceCollectionMember<TestClass>]
                public class TestClassMember;
            }
            """;
        
        // lang=cs
        const string expected =
            """
            // <auto-generated/>
            #nullable enable
            
            namespace TestProject;
            
            partial class TestClass
            {
                public global::System.Collections.Generic.IReadOnlyList<global::TestProject.IService> All { get; } = new global::System.Collections.Generic.List<global::TestProject.IService>
                {
                };
            }
            """;
        
        await GeneratorTest<ServiceCollectionGenerator>
            .VerifyGeneratorAsync(
                source,
                (ServiceCollectionGenerator.ServiceCollectionAttributeHintName, ServiceCollectionGenerator.ServiceCollectionAttributeSource),
                (ServiceCollectionGenerator.ServiceCollectionMemberAttributeHintName, ServiceCollectionGenerator.ServiceCollectionMemberAttributeSource),
                ("TestProject.TestClass.g.cs", expected));
    }

    [Fact]
    public async Task ShouldGenerateExpected_WhenCodeIsCorrect()
    {
        // lang=cs
        const string source =
            $$"""
            #nullable enable

            using OpenTrackerNext.Core.Utils;
            using OpenTrackerNext.ServiceCollection;
            
            {{InterfaceDefinition}}

            namespace TestProject
            {
                public interface IService;
                
                [ServiceCollection]
                public partial class TestClass : IServiceCollection<IService>;
                
                [ServiceCollectionMember<TestClass>]
                public class TestClassMember : IService;
            }
            """;
        
        // lang=cs
        const string expected =
            """
            // <auto-generated/>
            #nullable enable

            namespace TestProject;

            partial class TestClass
            {
                public global::System.Collections.Generic.IReadOnlyList<global::TestProject.IService> All { get; } = new global::System.Collections.Generic.List<global::TestProject.IService>
                {
                    (global::TestProject.IService)global::Splat.Locator.Current.GetService(typeof(global::TestProject.TestClassMember))!,
                };
            }
            """;
        
        await GeneratorTest<ServiceCollectionGenerator>
            .VerifyGeneratorAsync(
                source,
                (ServiceCollectionGenerator.ServiceCollectionAttributeHintName, ServiceCollectionGenerator.ServiceCollectionAttributeSource),
                (ServiceCollectionGenerator.ServiceCollectionMemberAttributeHintName, ServiceCollectionGenerator.ServiceCollectionMemberAttributeSource),
                ("TestProject.TestClass.g.cs", expected));
    }
}