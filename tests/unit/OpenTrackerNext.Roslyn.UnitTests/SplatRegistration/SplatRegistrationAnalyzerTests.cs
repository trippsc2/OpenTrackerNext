using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using OpenTrackerNext.Roslyn.SplatRegistration;
using Xunit;

namespace OpenTrackerNext.Roslyn.UnitTests.SplatRegistration;

[ExcludeFromCodeCoverage]
public sealed class SplatRegistrationAnalyzerTests
{
    // lang=cs
    private const string AttributeSourceCode =
        """
        #nullable enable
        
        using Avalonia.ReactiveUI;
        using OpenTrackerNext.SplatRegistration;
        using ReactiveUI;
        
        namespace OpenTrackerNext.SplatRegistration
        {
            /// <summary>
            /// Marks a non-generic concrete class to be registered with Splat.
            /// </summary>
            [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
            [global::System.AttributeUsage(global::System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
            internal sealed class SplatAttribute : global::System.Attribute
            {
                /// <summary>
                /// Gets a <see cref="global::System.Type"/> representing the concrete type to be registered.
                /// </summary>
                public global::System.Type? RegisterAsType { get; init; }
            }
            
            /// <summary>
            /// Marks a constructor to be used during Splat registration.
            /// </summary>
            [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
            [global::System.AttributeUsage(global::System.AttributeTargets.Constructor, Inherited = false, AllowMultiple = false)]
            internal sealed class SplatConstructorAttribute : global::System.Attribute;
            
            /// <summary>
            /// Marks a generic concrete class to be registered with Splat.
            /// </summary>
            [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
            [global::System.AttributeUsage(global::System.AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
            internal sealed class SplatGenericAttribute : global::System.Attribute
            {
                /// <summary>
                /// Gets a <see cref="global::System.Type"/> representing the concrete type to be registered.
                /// </summary>
                public required global::System.Type ConcreteType { get; init; }
                
                /// <summary>
                /// Gets a <see cref="global::System.Type"/> representing the type to be registered as.
                /// </summary>
                public global::System.Type? RegisterAsType { get; init; }
            }
            
            /// <summary>
            /// Marks a ReactiveUI view class to be ignored by the source generator.
            /// </summary>
            [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
            [global::System.AttributeUsage(global::System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
            internal sealed class SplatIgnoreReactiveViewAttribute : global::System.Attribute;
            
            /// <summary>
            /// Marks a concrete class to be registered with Splat as a single instance.
            /// </summary>
            [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
            [global::System.AttributeUsage(global::System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
            internal sealed class SplatSingleInstanceAttribute : global::System.Attribute;
        }
        
        
        """;
    
    [Fact]
    public async Task ShouldProduceError_WhenNoConstructorIsFoundOnViewForType()
    {
        // lang=cs
        const string source =
            """
            namespace TestProject
            {
                public class TestViewModel : ReactiveObject
                {
                }
            
                public class TestView : ReactiveUserControl<TestViewModel>
                {
                    public TestView(int i)
                    {
                    }
                }
            }
            """;
        
        await AnalyzerTest<SplatRegistrationAnalyzer>
            .VerifyAnalyzerAsync(
                AttributeSourceCode + source,
             new DiagnosticResult(SplatRegistrationAnalyzer.InvalidReactiveViewConstructor)
                 .WithSpan(68, 18, 68, 26)
                 .WithArguments("TestView"));
    }
    
    [Fact]
    public async Task ShouldNotProduceError_WhenNoConstructorIsFoundOnReactiveViewTypeButMarkedToIgnore()
    {
        // lang=cs
        const string source =
            """
            namespace TestProject
            {
                public class TestViewModel : ReactiveObject;
                
                [SplatIgnoreReactiveView]
                public class TestView : ReactiveUserControl<TestViewModel>
                {
                    public TestView(int i)
                    {
                    }
                }
            }
            """;
        
        await AnalyzerTest<SplatRegistrationAnalyzer>.VerifyAnalyzerAsync(AttributeSourceCode + source);
    }
    
    [Fact]
    public async Task ShouldNotProduceError_WhenParameterlessConstructorExistsOnReactiveView()
    {
        // lang=cs
        const string source =
            """
            namespace TestProject
            {
                public class TestViewModel : ReactiveObject;
                
                public class TestView : ReactiveUserControl<TestViewModel>
                {
                    public TestView()
                    {
                    }
                }
            }
            """;
        
        await AnalyzerTest<SplatRegistrationAnalyzer>.VerifyAnalyzerAsync(AttributeSourceCode + source);
    }
    
    [Fact]
    public async Task ShouldNotProduceError_WhenNoConstructorsExistOnReactiveView()
    {
        // lang=cs
        const string source =
            """
            namespace TestProject
            {
                public class TestViewModel : ReactiveObject;

                public class TestView : ReactiveUserControl<TestViewModel>
                {
                }
            }
            """;
        
        await AnalyzerTest<SplatRegistrationAnalyzer>.VerifyAnalyzerAsync(AttributeSourceCode + source);
    }
    
    [Fact]
    public async Task ShouldProduceWarning_WhenReactiveViewIsMarkedWithSplatAttribute()
    {
        // lang=cs
        const string source =
            """
            namespace TestProject
            {
                public class TestViewModel : ReactiveObject;
                
                [Splat]
                public class TestView : ReactiveUserControl<TestViewModel>
                {
                    public TestView()
                    {
                    }
                }
            }
            """;
        
        await AnalyzerTest<SplatRegistrationAnalyzer>
            .VerifyAnalyzerAsync(
                AttributeSourceCode + source,
                new DiagnosticResult(SplatRegistrationAnalyzer.SplatAttributeOnReactiveViewType)
                    .WithSpan(67, 18, 67, 26)
                    .WithArguments("TestView"));
    }
    
    [Fact]
    public async Task ShouldProduceWarning_WhenReactiveViewIsMarkedWithSplatGenericAttribute()
    {
        // lang=cs
        const string source =
            """
            namespace TestProject
            {
                public class TestViewModel : ReactiveObject;
                
                [SplatGeneric(ConcreteType = typeof(TestView))]
                public class TestView : ReactiveUserControl<TestViewModel>
                {
                    public TestView()
                    {
                    }
                }
            }
            """;
        
        await AnalyzerTest<SplatRegistrationAnalyzer>
            .VerifyAnalyzerAsync(
                AttributeSourceCode + source,
                new DiagnosticResult(SplatRegistrationAnalyzer.SplatGenericAttributeOnReactiveViewType)
                    .WithSpan(67, 18, 67, 26)
                    .WithArguments("TestView"));
    }
    
    [Fact]
    public async Task ShouldProduceWarning_WhenNonReactiveViewIsMarkedWithSplatIgnoreReactiveViewAttribute()
    {
        // lang=cs
        const string source =
            """
            namespace TestProject
            {
                [SplatIgnoreReactiveView]
                public class TestViewModel : ReactiveObject;
                
                public class TestView : ReactiveUserControl<TestViewModel>
                {
                    public TestView()
                    {
                    }
                }
            }
            """;
        
        await AnalyzerTest<SplatRegistrationAnalyzer>
            .VerifyAnalyzerAsync(
                AttributeSourceCode + source,
                new DiagnosticResult(SplatRegistrationAnalyzer.SplatIgnoreReactiveViewAttributeOnNonReactiveView)
                    .WithSpan(65, 18, 65, 31)
                    .WithArguments("TestViewModel"));
    }
    
    [Fact]
    public async Task ShouldProduceError_WhenMarkedTypeHasMultipleConstructorsNoneMarked()
    {
        // lang=cs
        const string source =
            """
            namespace TestProject
            {
                [Splat]
                public class TestObject
                {
                    public TestObject()
                    {
                    }
                    
                    public TestObject(int i)
                    {
                    }
                }
            }
            """;
        
        await AnalyzerTest<SplatRegistrationAnalyzer>
            .VerifyAnalyzerAsync(
                AttributeSourceCode + source,
                new DiagnosticResult(SplatRegistrationAnalyzer.MissingConstructorMarking)
                    .WithSpan(65, 18, 65, 28)
                    .WithArguments("TestObject"));
    }
    
    [Fact]
    public async Task ShouldNotProduceError_WhenMarkedTypeHasMultipleConstructorsOneIsMarked()
    {
        // lang=cs
        const string source =
            """
            namespace TestProject
            {
                [Splat]
                public class TestObject
                {
                    public TestObject()
                    {
                    }
                    
                    [SplatConstructor]
                    public TestObject(int i)
                    {
                    }
                }
            }
            """;
        
        await AnalyzerTest<SplatRegistrationAnalyzer>.VerifyAnalyzerAsync(AttributeSourceCode + source);
    }
    
    [Fact]
    public async Task ShouldNotProduceError_WhenMarkedTypeHasNoConstructors()
    {
        // lang=cs
        const string source =
            """
            namespace TestProject
            {
                [Splat]
                public class TestObject;
            }
            """;
        
        await AnalyzerTest<SplatRegistrationAnalyzer>.VerifyAnalyzerAsync(AttributeSourceCode + source);
    }
    
    [Fact]
    public async Task ShouldNotProduceError_WhenMarkedTypeHasOneConstructor()
    {
        // lang=cs
        const string source =
            """
            namespace TestProject
            {
                [Splat]
                public class TestObject
                {
                    public TestObject(int i)
                    {
                    }
                }
            }
            """;
        
        await AnalyzerTest<SplatRegistrationAnalyzer>.VerifyAnalyzerAsync(AttributeSourceCode + source);
    }

    [Fact]
    public async Task ShouldProduceError_WhenSplatAttributeMarkedTypeIsAbstract()
    {
        // lang=cs
        const string source =
            """
            namespace TestProject
            {
                [Splat]
                public abstract class TestObject
                {
                    public TestObject(int i)
                    {
                    }
                }
            }
            """;
        
        await AnalyzerTest<SplatRegistrationAnalyzer>
            .VerifyAnalyzerAsync(
                AttributeSourceCode + source,
                new DiagnosticResult(SplatRegistrationAnalyzer.SplatAttributeOnAbstractType)
                    .WithSpan(65, 27, 65, 37)
                    .WithArguments("TestObject"));
    }

    [Fact]
    public async Task ShouldProduceError_WhenSplatAttributeMarkedTypeIsGeneric()
    {
        // lang=cs
        const string source =
            """
            namespace TestProject
            {
                [Splat]
                public class TestObject<T>
                {
                    public TestObject(int i)
                    {
                    }
                }
            }
            """;
        
        await AnalyzerTest<SplatRegistrationAnalyzer>
            .VerifyAnalyzerAsync(
                AttributeSourceCode + source,
                new DiagnosticResult(SplatRegistrationAnalyzer.SplatAttributeOnGenericType)
                    .WithSpan(65, 18, 65, 28)
                    .WithArguments("TestObject"));
    }

    [Fact]
    public async Task ShouldProduceError_WhenSplatGenericAttributeMarkedTypeIsAbstract()
    {
        // lang=cs
        const string source =
            """
            namespace TestProject
            {
                [SplatGeneric(ConcreteType = typeof(TestObject<int>))]
                public abstract class TestObject<T>
                {
                    public TestObject(int i)
                    {
                    }
                }
            }
            """;
        
        await AnalyzerTest<SplatRegistrationAnalyzer>
            .VerifyAnalyzerAsync(
                AttributeSourceCode + source,
                new DiagnosticResult(SplatRegistrationAnalyzer.SplatGenericAttributeOnAbstractType)
                    .WithSpan(65, 27, 65, 37)
                    .WithArguments("TestObject"));
    }

    [Fact]
    public async Task ShouldProduceError_WhenSplatGenericAttributeMarkedTypeIsNotGeneric()
    {
        // lang=cs
        const string source =
            """
            namespace TestProject
            {
                [SplatGeneric(ConcreteType = typeof(TestObject))]
                public class TestObject
                {
                    public TestObject(int i)
                    {
                    }
                }
            }
            """;
        
        await AnalyzerTest<SplatRegistrationAnalyzer>
            .VerifyAnalyzerAsync(
                AttributeSourceCode + source,
                new DiagnosticResult(SplatRegistrationAnalyzer.SplatGenericAttributeOnNonGenericType)
                    .WithSpan(65, 18, 65, 28)
                    .WithArguments("TestObject"));
    }

    [Fact]
    public async Task ShouldProduceError_WhenSplatAttributeRegisterAsTypeIsUnboundGeneric()
    {
        // lang=cs
        const string source =
            """
            namespace TestProject
            {
                public class TestObject<T>
                {
                    public TestObject(int i)
                    {
                    }
                }

                [Splat(RegisterAsType = typeof(TestObject<>))]
                public class TestObject
                {
                    public TestObject(int i)
                    {
                    }
                }
            }
            """;
        
        await AnalyzerTest<SplatRegistrationAnalyzer>
            .VerifyAnalyzerAsync(
                AttributeSourceCode + source,
                new DiagnosticResult(SplatRegistrationAnalyzer.RegisterAsTypeIsUnboundGeneric)
                    .WithSpan(72, 18, 72, 28)
                    .WithArguments("TestObject", "TestObject"));
    }

    [Fact]
    public async Task ShouldProduceError_WhenSplatAttributeRegisterAsTypeIsDelegateReturningUnboundGeneric()
    {
        // lang=cs
        const string source =
            """
            namespace TestProject
            {
                public class TestObject<T>
                {
                    public delegate TestObject<T> Factory();
                    
                    public TestObject(int i)
                    {
                    }
                }
                
                [Splat(RegisterAsType = typeof(TestObject<>.Factory))]
                public class TestObject
                {
                    public TestObject(int i)
                    {
                    }
                }
            }
            """;
        
        await AnalyzerTest<SplatRegistrationAnalyzer>
            .VerifyAnalyzerAsync(
                AttributeSourceCode + source,
                new DiagnosticResult(SplatRegistrationAnalyzer.RegisterAsTypeIsDelegateWithInvalidReturnType)
                    .WithSpan(74, 18, 74, 28)
                    .WithArguments("Factory"));
    }

    [Fact]
    public async Task ShouldProduceError_WhenSplatGenericAttributeRegisterAsTypeIsUnboundGeneric()
    {
        // lang=cs
        const string source =
            """
            namespace TestProject
            {
                [SplatGeneric(ConcreteType = typeof(TestObject<int>), RegisterAsType = typeof(TestObject<>))]
                public class TestObject<T>
                {
                    public TestObject(int i)
                    {
                    }
                }
            }
            """;
        
        await AnalyzerTest<SplatRegistrationAnalyzer>
            .VerifyAnalyzerAsync(
                AttributeSourceCode + source,
                new DiagnosticResult(SplatRegistrationAnalyzer.RegisterAsTypeIsUnboundGeneric)
                    .WithSpan(65, 18, 65, 28)
                    .WithArguments("TestObject", "TestObject"));
    }

    [Fact]
    public async Task ShouldProduceError_WhenSplatGenericAttributeRegisterAsTypeIsDelegateReturningUnboundGeneric()
    {
        // lang=cs
        const string source =
            """
            namespace TestProject
            {
                [SplatGeneric(ConcreteType = typeof(TestObject<int>), RegisterAsType = typeof(TestObject<>.Factory))]
                public class TestObject<T>
                {
                    public delegate TestObject<T> Factory();
                    
                    public TestObject(int i)
                    {
                    }
                }
            }
            """;
        
        await AnalyzerTest<SplatRegistrationAnalyzer>
            .VerifyAnalyzerAsync(
                AttributeSourceCode + source,
                new DiagnosticResult(SplatRegistrationAnalyzer.RegisterAsTypeIsDelegateWithInvalidReturnType)
                    .WithSpan(65, 18, 65, 28)
                    .WithArguments("Factory"));
    }

    [Fact]
    public async Task ShouldProduceWarning_WhenSplatAttributeRegisterAsTypeIsConcreteType()
    {
        // lang=cs
        const string source =
            """
            namespace TestProject
            {
                [Splat(RegisterAsType = typeof(TestObject))]
                public class TestObject
                {
                    public TestObject(int i)
                    {
                    }
                }
            }
            """;
        
        await AnalyzerTest<SplatRegistrationAnalyzer>
            .VerifyAnalyzerAsync(
                AttributeSourceCode + source,
                new DiagnosticResult(SplatRegistrationAnalyzer.RegisterAsTypeIsSameAsConcreteType)
                    .WithSpan(65, 18, 65, 28)
                    .WithArguments("TestObject", "TestObject"));
    }

    [Fact]
    public async Task ShouldProduceWarning_WhenSplatAttributeRegisterAsTypeIsDelegateReturningConcreteType()
    {
        // lang=cs
        const string source =
            """
            namespace TestProject
            {
                [Splat(RegisterAsType = typeof(TestObject))]
                public class TestObject
                {
                    public delegate TestObject Factory(int i);
                    
                    public TestObject(int i)
                    {
                    }
                }
            }
            """;
        
        await AnalyzerTest<SplatRegistrationAnalyzer>
            .VerifyAnalyzerAsync(
                AttributeSourceCode + source,
                new DiagnosticResult(SplatRegistrationAnalyzer.RegisterAsTypeIsSameAsConcreteType)
                    .WithSpan(65, 18, 65, 28)
                    .WithArguments("TestObject", "TestObject"));
    }

    [Fact]
    public async Task ShouldProduceWarning_WhenSplatGenericAttributeRegisterAsTypeIsConcreteType()
    {
        // lang=cs
        const string source =
            """
            namespace TestProject
            {
                [SplatGeneric(ConcreteType = typeof(TestObject<int>), RegisterAsType = typeof(TestObject<int>))]
                public class TestObject<T>
                {
                    public TestObject(int i)
                    {
                    }
                }
            }
            """;
        
        await AnalyzerTest<SplatRegistrationAnalyzer>
            .VerifyAnalyzerAsync(
                AttributeSourceCode + source,
                new DiagnosticResult(SplatRegistrationAnalyzer.RegisterAsTypeIsSameAsConcreteType)
                    .WithSpan(65, 18, 65, 28)
                    .WithArguments("TestObject", "TestObject"));
    }

    [Fact]
    public async Task ShouldNotProduceWarning_WhenSplatGenericAttributeRegisterAsTypeIsDelegateReturningConcreteType()
    {
        // lang=cs
        const string source =
            """
            namespace TestProject
            {
                [SplatGeneric(ConcreteType = typeof(TestObject<int>), RegisterAsType = typeof(TestObject<int>.Factory))]
                public class TestObject<T>
                {
                    public delegate TestObject<T> Factory(int i);
                    
                    public TestObject(int i)
                    {
                    }
                }
            }
            """;
        
        await AnalyzerTest<SplatRegistrationAnalyzer>.VerifyAnalyzerAsync(AttributeSourceCode + source);
    }

    [Fact]
    public async Task ShouldProduceError_WhenSplatAttributeRegisterAsTypeIsNotValid()
    {
        // lang=cs
        const string source =
            """
            namespace TestProject
            {
                public interface ITestObject;
                
                [Splat(RegisterAsType = typeof(ITestObject))]
                public class TestObject
                {
                    public TestObject(int i)
                    {
                    }
                }
            }
            """;
        
        await AnalyzerTest<SplatRegistrationAnalyzer>
            .VerifyAnalyzerAsync(
                AttributeSourceCode + source,
                new DiagnosticResult(SplatRegistrationAnalyzer.ConcreteTypeCannotBeAssignedToRegisterAsType)
                    .WithSpan(67, 18, 67, 28)
                    .WithArguments("TestObject", "ITestObject"));
    }

    [Fact]
    public async Task ShouldProduceWarning_WhenSplatSingleInstanceAttributeWithoutOtherAttribute()
    {
        // lang=cs
        const string source =
            """
            namespace TestProject
            {
                [SplatSingleInstance]
                public class TestObject
                {
                    public TestObject(int i)
                    {
                    }
                }
            }
            """;
        
        await AnalyzerTest<SplatRegistrationAnalyzer>
            .VerifyAnalyzerAsync(
                AttributeSourceCode + source,
                new DiagnosticResult(SplatRegistrationAnalyzer.SplatSingleInstanceAttributeOnUnmarkedType)
                    .WithSpan(65, 18, 65, 28)
                    .WithArguments("TestObject", "TestObject"));
    }
    
    [Fact]
    public async Task ShouldProduceError_WhenSplatGenericConcreteTypeIsNotSameAsMarkedType()
    {
        // lang=cs
        const string source =
            """
            namespace TestProject
            {
                public class OtherObject<T>;
                
                [SplatGeneric(ConcreteType = typeof(OtherObject<int>))]
                public class TestObject<T>
                {
                    public TestObject(int i)
                    {
                    }
                }
            }
            """;
        
        await AnalyzerTest<SplatRegistrationAnalyzer>
            .VerifyAnalyzerAsync(
                AttributeSourceCode + source,
                new DiagnosticResult(SplatRegistrationAnalyzer.SplatGenericConcreteTypeIsNotValid)
                    .WithSpan(67, 18, 67, 28)
                    .WithArguments("TestObject", "OtherObject"));
    }
}