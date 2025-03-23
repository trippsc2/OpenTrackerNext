using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using OpenTrackerNext.Roslyn.Factories;
using Xunit;

namespace OpenTrackerNext.Roslyn.UnitTests.Factories;

[ExcludeFromCodeCoverage]
public sealed class FactoryAnalyzerTests
{
    // lang=cs
    private const string AttributeInterfaceSource =
        """
        namespace OpenTrackerNext.Factories
        {
            /// <summary>
            /// Marks a class as an abstract factory.
            /// </summary>
            [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
            [global::System.AttributeUsage(global::System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
            internal sealed class AbstractFactoryAttribute : global::System.Attribute;

            /// <summary>
            /// Marks a class as a factory belonging to the specified abstract factory.
            /// </summary>
            [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
            [global::System.AttributeUsage(global::System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
            internal sealed class FactoryAttribute : global::System.Attribute;
        }
        
        namespace OpenTrackerNext.Core.Factories
        {
            /// <summary>
            /// Represents a factory.
            /// </summary>
            /// <typeparam name="TInput">
            /// The type of the input.
            /// </typeparam>
            /// <typeparam name="TOutput">
            /// The type of the output.
            /// </typeparam>
            public interface IFactory<in TInput, out TOutput>
                where TInput : notnull
                where TOutput : notnull
            {
                /// <summary>
                /// Creates a new <typeparamref name="TOutput"/> object.
                /// </summary>
                /// <param name="input">
                /// A <see cref="TInput"/> representing the input.
                /// </param>
                /// <returns>
                /// A new <typeparamref name="TOutput"/> object.
                /// </returns>
                TOutput Create(TInput input);
            }
        
            /// <summary>
            /// Represents the specific creation logic for <see cref="TOutput"/> objects created from <see cref="TInput"/> objects.
            /// </summary>
            /// <typeparam name="TBaseInput">
            /// The type of the base input.
            /// </typeparam>
            /// <typeparam name="TOutput">
            /// The type of the output.
            /// </typeparam>
            /// <typeparam name="TInput">
            /// The type of the input.
            /// </typeparam>
            public interface ISpecificFactory<in TBaseInput, out TOutput, in TInput> : IFactory<TBaseInput, TOutput>
                where TBaseInput : notnull
                where TOutput : notnull
                where TInput : TBaseInput
            {
                /// <summary>
                /// Returns a new <see cref="TOutput"/> object created from the specified <see cref="TInput"/> object.
                /// </summary>
                /// <param name="input">
                /// A <see cref="TInput"/> representing the input object.
                /// </param>
                /// <returns>
                /// A new <see cref="TOutput"/> object.
                /// </returns>
                TOutput Create(TInput input);
            }
        }
        """;
    
    // lang=cs
    private const string BaseInputDefinition =
        """
            public interface IBaseInput
            {
            }
        """;
    
    // lang=cs
    private const string Input1Definition =
        """
            public class Input1 : IBaseInput
            {
            }
        """;
    
    // lang=cs
    private const string Input2Definition =
        """
            public class Input2 : IBaseInput
            {
            }
        """;
    
    // lang=cs
    private const string OutputDefinition =
        """
            public interface IOutput
            {
            }
        """;
    
    // lang=cs
    private const string Output1Definition =
        """
            public class Output1 : IOutput
            {
            }
        """;
    
    // lang=cs
    private const string Output2Definition =
        """
            public class Output2 : IOutput
            {
            }
        """;

    [Fact]
    public async Task ShouldGenerateError_WhenSpecificFactoryIsStatic()
    {
        // lang=cs
        const string source =
            $$"""
              using OpenTrackerNext.Core.Factories;
              using OpenTrackerNext.Factories;
              
              {{AttributeInterfaceSource}}

              namespace TestProject
              {
              {{BaseInputDefinition}}

              {{Input1Definition}}

              {{Input2Definition}}

              {{OutputDefinition}}

              {{Output1Definition}}

              {{Output2Definition}}

                  [Factory]
                  public static partial class SpecificFactory1
                  {
                  }

                  [Factory]
                  public partial class SpecificFactory2 : ISpecificFactory<IBaseInput, IOutput, Input2>
                  {
                      public IOutput Create(Input2 input)
                      {
                          return new Output2();
                      }
                  }

                  [AbstractFactory]
                  public partial class AbstractFactory : IFactory<IBaseInput, IOutput>
                  {
                  }
              }   
              """;

        await AnalyzerTest<FactoryAnalyzer>
            .VerifyAnalyzerAsync(
                source,
                new DiagnosticResult(FactoryAnalyzer.SpecificFactoryIsStatic)
                    .WithSpan(105, 33, 105, 49)
                    .WithArguments("SpecificFactory1"),
                new DiagnosticResult(FactoryAnalyzer.SpecificFactoryDoesNotImplementInterface)
                    .WithSpan(105, 33, 105, 49)
                    .WithArguments("SpecificFactory1"),
                new DiagnosticResult("CS0535", DiagnosticSeverity.Error)
                    .WithSpan(110, 45, 110, 90)
                    .WithArguments(
                        "TestProject.SpecificFactory2",
                        "OpenTrackerNext.Core.Factories.IFactory<TestProject.IBaseInput, TestProject.IOutput>.Create(TestProject.IBaseInput)"),
                new DiagnosticResult("CS0535", DiagnosticSeverity.Error)
                    .WithSpan(119, 44, 119, 73)
                    .WithArguments(
                        "TestProject.AbstractFactory",
                        "OpenTrackerNext.Core.Factories.IFactory<TestProject.IBaseInput, TestProject.IOutput>.Create(TestProject.IBaseInput)"));
    }

    [Fact]
    public async Task ShouldGenerateError_WhenSpecificFactoryIsNotPartial()
    {
        // lang=cs
        const string source =
            $$"""
              using OpenTrackerNext.Core.Factories;
              using OpenTrackerNext.Factories;
              
              {{AttributeInterfaceSource}}
              
              namespace TestProject
              {
              {{BaseInputDefinition}}

              {{Input1Definition}}

              {{Input2Definition}}

              {{OutputDefinition}}

              {{Output1Definition}}

              {{Output2Definition}}

                  [Factory]
                  public class SpecificFactory1 : ISpecificFactory<IBaseInput, IOutput, Input1>
                  {
                      public IOutput Create(Input1 input)
                      {
                          return new Output1();
                      }
                  }
    
                  [Factory]
                  public partial class SpecificFactory2 : ISpecificFactory<IBaseInput, IOutput, Input2>
                  {
                      public IOutput Create(Input2 input)
                      {
                          return new Output2();
                      }
                  }

                  [AbstractFactory]
                  public partial class AbstractFactory : IFactory<IBaseInput, IOutput>
                  {
                  }
              }
              """;

        await AnalyzerTest<FactoryAnalyzer>
            .VerifyAnalyzerAsync(
                source,
                new DiagnosticResult(FactoryAnalyzer.SpecificFactoryIsNotPartial)
                    .WithSpan(105, 18, 105, 34)
                    .WithArguments("SpecificFactory1"),
                new DiagnosticResult("CS0535", DiagnosticSeverity.Error)
                    .WithSpan(105, 37, 105, 82)
                    .WithArguments(
                        "TestProject.SpecificFactory1",
                        "OpenTrackerNext.Core.Factories.IFactory<TestProject.IBaseInput, TestProject.IOutput>.Create(TestProject.IBaseInput)"),
                new DiagnosticResult("CS0535", DiagnosticSeverity.Error)
                    .WithSpan(114, 45, 114, 90)
                    .WithArguments(
                        "TestProject.SpecificFactory2",
                        "OpenTrackerNext.Core.Factories.IFactory<TestProject.IBaseInput, TestProject.IOutput>.Create(TestProject.IBaseInput)"),
                new DiagnosticResult("CS0535", DiagnosticSeverity.Error)
                    .WithSpan(123, 44, 123, 73)
                    .WithArguments(
                        "TestProject.AbstractFactory",
                        "OpenTrackerNext.Core.Factories.IFactory<TestProject.IBaseInput, TestProject.IOutput>.Create(TestProject.IBaseInput)"));
    }

    [Fact]
    public async Task ShouldGenerateError_WhenSpecificFactoryDoesNotImplementInterface()
    {
        // lang=cs
        const string source =
            $$"""
              using OpenTrackerNext.Core.Factories;
              using OpenTrackerNext.Factories;
              
              {{AttributeInterfaceSource}}
              
              namespace TestProject
              {
              {{BaseInputDefinition}}

              {{Input1Definition}}

              {{Input2Definition}}

              {{OutputDefinition}}

              {{Output1Definition}}

              {{Output2Definition}}

                  [Factory]
                  public partial class SpecificFactory1
                  {
                  }

                  [Factory]
                  public partial class SpecificFactory2 : ISpecificFactory<IBaseInput, IOutput, Input2>
                  {
                      public IOutput Create(Input2 input)
                      {
                          return new Output2();
                      }
                  }

                  [AbstractFactory]
                  public partial class AbstractFactory : IFactory<IBaseInput, IOutput>
                  {
                  }
              }
              """;

        await AnalyzerTest<FactoryAnalyzer>
            .VerifyAnalyzerAsync(
                source,
                new DiagnosticResult(FactoryAnalyzer.SpecificFactoryDoesNotImplementInterface)
                    .WithSpan(105, 26, 105, 42)
                    .WithArguments("SpecificFactory1"),
                new DiagnosticResult("CS0535", DiagnosticSeverity.Error)
                    .WithSpan(110, 45, 110, 90)
                    .WithArguments(
                        "TestProject.SpecificFactory2",
                        "OpenTrackerNext.Core.Factories.IFactory<TestProject.IBaseInput, TestProject.IOutput>.Create(TestProject.IBaseInput)"),
                new DiagnosticResult("CS0535", DiagnosticSeverity.Error)
                    .WithSpan(119, 44, 119, 73)
                    .WithArguments(
                        "TestProject.AbstractFactory",
                        "OpenTrackerNext.Core.Factories.IFactory<TestProject.IBaseInput, TestProject.IOutput>.Create(TestProject.IBaseInput)"));
    }
    
    [Fact]
    public async Task ShouldGenerateNothing_WhenAbstractFactoryIsStatic()
    {
        // lang=cs
        const string source =
            $$"""
              using OpenTrackerNext.Core.Factories;
              using OpenTrackerNext.Factories;
              
              {{AttributeInterfaceSource}}
              
              namespace TestProject
              {
              {{BaseInputDefinition}}

              {{Input1Definition}}

              {{Input2Definition}}

              {{OutputDefinition}}

              {{Output1Definition}}

              {{Output2Definition}}

                  [Factory]
                  public partial class SpecificFactory1 : ISpecificFactory<IBaseInput, IOutput, Input1>
                  {
                      public IOutput Create(Input1 input)
                      {
                          return new Output1();
                      }
                  }

                  [Factory]
                  public partial class SpecificFactory2 : ISpecificFactory<IBaseInput, IOutput, Input2>
                  {
                      public IOutput Create(Input2 input)
                      {
                          return new Output2();
                      }
                  }

                  [AbstractFactory]
                  public static partial class AbstractFactory
                  {
                  }
              }
              """;

        await AnalyzerTest<FactoryAnalyzer>
            .VerifyAnalyzerAsync(
                source,
                new DiagnosticResult("CS0535", DiagnosticSeverity.Error)
                    .WithSpan(105, 45, 105, 90)
                    .WithArguments(
                        "TestProject.SpecificFactory1",
                        "OpenTrackerNext.Core.Factories.IFactory<TestProject.IBaseInput, TestProject.IOutput>.Create(TestProject.IBaseInput)"),
                new DiagnosticResult("CS0535", DiagnosticSeverity.Error)
                    .WithSpan(114, 45, 114, 90)
                    .WithArguments(
                        "TestProject.SpecificFactory2",
                        "OpenTrackerNext.Core.Factories.IFactory<TestProject.IBaseInput, TestProject.IOutput>.Create(TestProject.IBaseInput)"),
                new DiagnosticResult(FactoryAnalyzer.AbstractFactoryIsStatic)
                    .WithSpan(123, 33, 123, 48)
                    .WithArguments("AbstractFactory"),
                new DiagnosticResult(FactoryAnalyzer.AbstractFactoryDoesNotImplementInterface)
                    .WithSpan(123, 33, 123, 48)
                    .WithArguments("AbstractFactory"));
    }
    
    [Fact]
    public async Task ShouldGenerateError_WhenAbstractFactoryIsNotPartial()
    {
        // lang=cs
        const string source =
            $$"""
              using OpenTrackerNext.Core.Factories;
              using OpenTrackerNext.Factories;
              
              {{AttributeInterfaceSource}}
              
              namespace TestProject
              {
              {{BaseInputDefinition}}

              {{Input1Definition}}

              {{Input2Definition}}

              {{OutputDefinition}}

              {{Output1Definition}}

              {{Output2Definition}}

                  [Factory]
                  public partial class SpecificFactory1 : ISpecificFactory<IBaseInput, IOutput, Input1>
                  {
                      public IOutput Create(Input1 input)
                      {
                          return new Output1();
                      }
                  }

                  [Factory]
                  public partial class SpecificFactory2 : ISpecificFactory<IBaseInput, IOutput, Input2>
                  {
                      public IOutput Create(Input2 input)
                      {
                          return new Output2();
                      }
                  }

                  [AbstractFactory]
                  public class AbstractFactory : IFactory<IBaseInput, IOutput>
                  {
                  }
              }
              """;

        await AnalyzerTest<FactoryAnalyzer>
            .VerifyAnalyzerAsync(
                source,
                new DiagnosticResult("CS0535", DiagnosticSeverity.Error)
                    .WithSpan(105, 45, 105, 90)
                    .WithArguments(
                        "TestProject.SpecificFactory1",
                        "OpenTrackerNext.Core.Factories.IFactory<TestProject.IBaseInput, TestProject.IOutput>.Create(TestProject.IBaseInput)"),
                new DiagnosticResult("CS0535", DiagnosticSeverity.Error)
                    .WithSpan(114, 45, 114, 90)
                    .WithArguments(
                        "TestProject.SpecificFactory2",
                        "OpenTrackerNext.Core.Factories.IFactory<TestProject.IBaseInput, TestProject.IOutput>.Create(TestProject.IBaseInput)"),
                new DiagnosticResult(FactoryAnalyzer.AbstractFactoryIsNotPartial)
                    .WithSpan(123, 18, 123, 33)
                    .WithArguments("AbstractFactory"),
                new DiagnosticResult("CS0535", DiagnosticSeverity.Error)
                    .WithSpan(123, 36, 123, 65)
                    .WithArguments(
                        "TestProject.AbstractFactory",
                        "OpenTrackerNext.Core.Factories.IFactory<TestProject.IBaseInput, TestProject.IOutput>.Create(TestProject.IBaseInput)"));
    }
    
    [Fact]
    public async Task ShouldGenerateError_WhenAbstractFactoryDoesNotImplementInterface()
    {
        // lang=cs
        const string source =
            $$"""
              using OpenTrackerNext.Core.Factories;
              using OpenTrackerNext.Factories;
              
              {{AttributeInterfaceSource}}
              
              namespace TestProject
              {
              {{BaseInputDefinition}}

              {{Input1Definition}}

              {{Input2Definition}}

              {{OutputDefinition}}

              {{Output1Definition}}

              {{Output2Definition}}

                  [Factory]
                  public partial class SpecificFactory1 : ISpecificFactory<IBaseInput, IOutput, Input1>
                  {
                      public IOutput Create(Input1 input)
                      {
                          return new Output1();
                      }
                  }

                  [Factory]
                  public partial class SpecificFactory2 : ISpecificFactory<IBaseInput, IOutput, Input2>
                  {
                      public IOutput Create(Input2 input)
                      {
                          return new Output2();
                      }
                  }

                  [AbstractFactory]
                  public partial class AbstractFactory
                  {
                  }
              }
              """;

        await AnalyzerTest<FactoryAnalyzer>
            .VerifyAnalyzerAsync(
                source,
                new DiagnosticResult("CS0535", DiagnosticSeverity.Error)
                    .WithSpan(105, 45, 105, 90)
                    .WithArguments(
                        "TestProject.SpecificFactory1",
                        "OpenTrackerNext.Core.Factories.IFactory<TestProject.IBaseInput, TestProject.IOutput>.Create(TestProject.IBaseInput)"),
                new DiagnosticResult("CS0535", DiagnosticSeverity.Error)
                    .WithSpan(114, 45, 114, 90)
                    .WithArguments(
                        "TestProject.SpecificFactory2",
                        "OpenTrackerNext.Core.Factories.IFactory<TestProject.IBaseInput, TestProject.IOutput>.Create(TestProject.IBaseInput)"),
                new DiagnosticResult(FactoryAnalyzer.AbstractFactoryDoesNotImplementInterface)
                    .WithSpan(123, 26, 123, 41)
                    .WithArguments("AbstractFactory"));
    }

        
    [Fact]
    public async Task ShouldGenerateError_WhenAbstractFactoryIsMarkedWithFactoryAttribute()
    {
        // lang=cs
        const string source =
            $$"""
              using OpenTrackerNext.Core.Factories;
              using OpenTrackerNext.Factories;
              
              {{AttributeInterfaceSource}}
              
              namespace TestProject
              {
              {{BaseInputDefinition}}

              {{Input1Definition}}

              {{Input2Definition}}

              {{OutputDefinition}}

              {{Output1Definition}}

              {{Output2Definition}}

                  [AbstractFactory]
                  [Factory]
                  public partial class AbstractFactory;
              }
              """;

        await AnalyzerTest<FactoryAnalyzer>
            .VerifyAnalyzerAsync(
                source,
                new DiagnosticResult(FactoryAnalyzer.AbstractFactoryDoesNotImplementInterface)
                    .WithSpan(106, 26, 106, 41)
                    .WithArguments("AbstractFactory"),
                new DiagnosticResult(FactoryAnalyzer.AbstractFactoryHasSpecificFactoryAttribute)
                    .WithSpan(106, 26, 106, 41)
                    .WithArguments("AbstractFactory"));
    }

    [Fact]
    public async Task ShouldGenerateNothing_WhenAbstractFactoryWithMultipleSpecificFactories()
    {
        // lang=cs
        const string source =
            $$"""
              using OpenTrackerNext.Core.Factories;
              using OpenTrackerNext.Factories;
              
              {{AttributeInterfaceSource}}
              
              namespace TestProject
              {
              {{BaseInputDefinition}}

              {{Input1Definition}}

              {{Input2Definition}}

              {{OutputDefinition}}

              {{Output1Definition}}

              {{Output2Definition}}

                  [Factory]
                  public partial class SpecificFactory1 : ISpecificFactory<IBaseInput, IOutput, Input1>
                  {
                      public IOutput Create(Input1 input)
                      {
                          return new Output1();
                      }
                  }

                  [Factory]
                  public partial class SpecificFactory2 : ISpecificFactory<IBaseInput, IOutput, Input2>
                  {
                      public IOutput Create(Input2 input)
                      {
                          return new Output2();
                      }
                  }

                  [AbstractFactory]
                  public partial class AbstractFactory : IFactory<IBaseInput, IOutput>;
              }
              """;

        await AnalyzerTest<FactoryAnalyzer>
            .VerifyAnalyzerAsync(
                source,
                new DiagnosticResult("CS0535", DiagnosticSeverity.Error)
                    .WithSpan(105, 45, 105, 90)
                    .WithArguments(
                        "TestProject.SpecificFactory1",
                        "OpenTrackerNext.Core.Factories.IFactory<TestProject.IBaseInput, TestProject.IOutput>.Create(TestProject.IBaseInput)"),
                new DiagnosticResult("CS0535", DiagnosticSeverity.Error)
                    .WithSpan(114, 45, 114, 90)
                    .WithArguments(
                        "TestProject.SpecificFactory2",
                        "OpenTrackerNext.Core.Factories.IFactory<TestProject.IBaseInput, TestProject.IOutput>.Create(TestProject.IBaseInput)"),
                new DiagnosticResult("CS0535", DiagnosticSeverity.Error)
                    .WithSpan(123, 44, 123, 73)
                    .WithArguments(
                        "TestProject.AbstractFactory",
                        "OpenTrackerNext.Core.Factories.IFactory<TestProject.IBaseInput, TestProject.IOutput>.Create(TestProject.IBaseInput)"));
    }}