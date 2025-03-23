using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using OpenTrackerNext.Roslyn.ServiceCollection;
using Xunit;

namespace OpenTrackerNext.Roslyn.UnitTests.ServiceCollection;

[ExcludeFromCodeCoverage]
public sealed class ServiceCollectionAnalyzerTests
{
    // lang=cs
    private const string GeneratedDefinitions =
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
        
        namespace OpenTrackerNext.ServiceCollection
        {
            [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
            [global::System.AttributeUsage(global::System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
            internal sealed class ServiceCollectionAttribute : global::System.Attribute;
            
            [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
            [global::System.AttributeUsage(global::System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
            internal sealed class ServiceCollectionMemberAttribute<TCollection> : global::System.Attribute
                where TCollection : class;
        }
        
        """;
    
    [Fact]
    public async Task ShouldProduceError_WhenServiceCollectionIsNotPartial()
    {
        // lang=cs
        const string source =
            $$"""
            #nullable enable
            
            using OpenTrackerNext.Core.Utils;
            using OpenTrackerNext.ServiceCollection;
            
            {{GeneratedDefinitions}}
            
            namespace TestProject
            {
                public interface IService;
            
                [ServiceCollection]
                public class TestClass : IServiceCollection<IService>;
            }
            """;
        
        await AnalyzerTest<ServiceCollectionAnalyzer>
            .VerifyAnalyzerAsync(
                source,
                new DiagnosticResult(ServiceCollectionAnalyzer.ServiceCollectionMustBePartial)
                    .WithSpan(42, 18, 42, 27)
                    .WithArguments("TestClass"),
                new DiagnosticResult("CS0535", DiagnosticSeverity.Error)
                    .WithSpan(42, 30, 42, 58)
                    .WithArguments(
                        "TestProject.TestClass",
                        "OpenTrackerNext.Core.Utils.IServiceCollection<TestProject.IService>.All"));
    }
    
    [Fact]
    public async Task ShouldProduceError_WhenServiceCollectionIsStatic()
    {
        // lang=cs
        const string source =
            $$"""
            #nullable enable

            using OpenTrackerNext.Core.Utils;
            using OpenTrackerNext.ServiceCollection;
            
            {{GeneratedDefinitions}}

            namespace TestProject
            {
                public interface IService;
                
                [ServiceCollection]
                public static class TestClass;
            }
            """;
        
        await AnalyzerTest<ServiceCollectionAnalyzer>
            .VerifyAnalyzerAsync(
                source,
                new DiagnosticResult(ServiceCollectionAnalyzer.ServiceCollectionMustBePartial)
                    .WithSpan(42, 25, 42, 34)
                    .WithArguments("TestClass"),
                new DiagnosticResult(ServiceCollectionAnalyzer.ServiceCollectionMustNotBeStatic)
                    .WithSpan(42, 25, 42, 34)
                    .WithArguments("TestClass"),
                new DiagnosticResult(ServiceCollectionAnalyzer.ServiceCollectionMustImplementInterface)
                    .WithSpan(42, 25, 42, 34)
                    .WithArguments("TestClass"));
    }

    [Fact]
    public async Task ShouldProduceError_WhenServiceCollectionDoesNotImplementInterface()
    {
        // lang=cs
        const string source =
            $$"""
            #nullable enable

            using OpenTrackerNext.Core.Utils;
            using OpenTrackerNext.ServiceCollection;
            
            {{GeneratedDefinitions}}

            namespace TestProject
            {
                public interface IService;

                [ServiceCollection]
                public partial class TestClass;
            }
            """;
        
        await AnalyzerTest<ServiceCollectionAnalyzer>
            .VerifyAnalyzerAsync(
                source,
                new DiagnosticResult(ServiceCollectionAnalyzer.ServiceCollectionMustImplementInterface)
                    .WithSpan(42, 26, 42, 35)
                    .WithArguments("TestClass"));
    }
    
    [Fact]
    public async Task ShouldProduceError_WhenServiceCollectionMemberIsStatic()
    {
        // lang=cs
        const string source =
            $$"""
            #nullable enable

            using OpenTrackerNext.Core.Utils;
            using OpenTrackerNext.ServiceCollection;
            
            {{GeneratedDefinitions}}

            namespace TestProject
            {
                public interface IService;
                
                [ServiceCollection]
                public partial class TestClass : IServiceCollection<IService>;
                
                [ServiceCollectionMember<TestClass>]
                public static class TestClassMembers;
            }
            """;
        
        await AnalyzerTest<ServiceCollectionAnalyzer>
            .VerifyAnalyzerAsync(
                source,
                new DiagnosticResult("CS0535", DiagnosticSeverity.Error)
                    .WithSpan(42, 38, 42, 66)
                    .WithArguments(
                        "TestProject.TestClass",
                        "OpenTrackerNext.Core.Utils.IServiceCollection<TestProject.IService>.All"),
                new DiagnosticResult(ServiceCollectionAnalyzer.ServiceCollectionMemberMustNotBeStatic)
                    .WithSpan(45, 25, 45, 41)
                    .WithArguments("TestClassMembers"),
                new DiagnosticResult(ServiceCollectionAnalyzer.ServiceCollectionMemberMustBelongInServiceCollectionType)
                    .WithSpan(45, 25, 45, 41)
                    .WithArguments("TestClassMembers", "TestClass", "IService"));
    }
    
    [Fact]
    public async Task ShouldProduceError_WhenServiceCollectionMemberTypeArgumentIsInvalid()
    {
        // lang=cs
        const string source =
            $$"""
            #nullable enable

            using OpenTrackerNext.Core.Utils;
            using OpenTrackerNext.ServiceCollection;
            
            {{GeneratedDefinitions}}

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
        
        await AnalyzerTest<ServiceCollectionAnalyzer>
            .VerifyAnalyzerAsync(
                source,
                new DiagnosticResult("CS0535", DiagnosticSeverity.Error)
                    .WithSpan(42, 38, 42, 66)
                    .WithArguments(
                        "TestProject.TestClass",
                        "OpenTrackerNext.Core.Utils.IServiceCollection<TestProject.IService>.All"),
                new DiagnosticResult(ServiceCollectionAnalyzer.ServiceCollectionMemberMarkedWithNonServiceCollectionType)
                    .WithSpan(47, 18, 47, 33)
                    .WithArguments("TestClassMember", "TestClass2"));
    }
    
    [Fact]
    public async Task ShouldProduceError_WhenServiceCollectionMemberTypeArgumentIsNotInServiceCollection()
    {
        // lang=cs
        const string source =
            $$"""
            #nullable enable

            using OpenTrackerNext.Core.Utils;
            using OpenTrackerNext.ServiceCollection;
            
            {{GeneratedDefinitions}}

            namespace TestProject
            {
                public interface IService;
                
                [ServiceCollection]
                public partial class TestClass : IServiceCollection<IService>;
                
                [ServiceCollectionMember<TestClass>]
                public class TestClassMember;
            }
            """;
        
        await AnalyzerTest<ServiceCollectionAnalyzer>
            .VerifyAnalyzerAsync(
                source,
                new DiagnosticResult("CS0535", DiagnosticSeverity.Error)
                    .WithSpan(42, 38, 42, 66)
                    .WithArguments(
                        "TestProject.TestClass",
                        "OpenTrackerNext.Core.Utils.IServiceCollection<TestProject.IService>.All"),
                new DiagnosticResult(ServiceCollectionAnalyzer.ServiceCollectionMemberMustBelongInServiceCollectionType)
                    .WithSpan(45, 18, 45, 33)
                    .WithArguments("TestClassMember", "TestClass", "IService"));
    }

    [Fact]
    public async Task ShouldProduceError_WhenClassMarkedWithBothServiceCollectionAndServiceCollectionMember()
    {
        // lang=cs
        const string source =
            $$"""
            #nullable enable

            using OpenTrackerNext.Core.Utils;
            using OpenTrackerNext.ServiceCollection;
            
            {{GeneratedDefinitions}}

            namespace TestProject
            {
                public interface IService;

                [ServiceCollection]
                [ServiceCollectionMember<TestClass>]
                public partial class TestClass : IServiceCollection<IService>, IService;
            }
            """;
        
        await AnalyzerTest<ServiceCollectionAnalyzer>
            .VerifyAnalyzerAsync(
                source,
                new DiagnosticResult(
                        ServiceCollectionAnalyzer.TypeCannotHaveBothServiceCollectionAndServiceCollectionMemberMarkings)
                    .WithSpan(43, 26, 43, 35)
                    .WithArguments("TestClass"),
                new DiagnosticResult("CS0535", DiagnosticSeverity.Error)
                    .WithSpan(43, 38, 43, 66)
                    .WithArguments(
                        "TestProject.TestClass",
                        "OpenTrackerNext.Core.Utils.IServiceCollection<TestProject.IService>.All"));
    }
}