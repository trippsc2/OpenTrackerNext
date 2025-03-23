using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using OpenTrackerNext.Roslyn.Factories;
using Xunit;

namespace OpenTrackerNext.Roslyn.UnitTests.Factories;

[ExcludeFromCodeCoverage]
public sealed class FactoryGeneratorTests
{
    // lang=cs
    private const string InterfaceDefinitions =
        """
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
    private const string BaseInputDefinition = "    public interface IBaseInput;";
    
    // lang=cs
    private const string Input1Definition = "    public class Input1 : IBaseInput;";
    
    // lang=cs
    private const string Input2Definition = "    public class Input2 : IBaseInput;";
    
    // lang=cs
    private const string OutputDefinition = "    public interface IOutput;";
    
    // lang=cs
    private const string Output1Definition = "    public class Output1 : IOutput;";
    
    // lang=cs
    private const string Output2Definition = "    public class Output2 : IOutput;";
    
    [Fact]
    public async Task ShouldGenerateNothing_WhenNoMatchingClass()
    {
        // lang=cs
        const string source =
            $$"""
            {{InterfaceDefinitions}}
            
            namespace TestProject
            {
            {{BaseInputDefinition}}
            
            {{Input1Definition}}
            
            {{Input2Definition}}
            
            {{OutputDefinition}}
            
            {{Output1Definition}}
            
            {{Output2Definition}}
            }
            """;
        
        await GeneratorTest<FactoryGenerator>
            .VerifyGeneratorAsync(
                source, 
                (FactoryGenerator.AbstractFactoryAttributeHintName, FactoryGenerator.AbstractFactoryAttributeSource),
                (FactoryGenerator.FactoryAttributeHintName, FactoryGenerator.FactoryAttributeSource));
    }

    [Fact]
    public async Task ShouldGenerateNothing_WhenSpecificFactoryIsStatic()
    {
        // lang=cs
        const string source =
            $$"""
              using OpenTrackerNext.Core.Factories;
              using OpenTrackerNext.Factories;

              {{InterfaceDefinitions}}
              
              namespace TestProject
              {
              {{BaseInputDefinition}}

              {{Input1Definition}}

              {{Input2Definition}}

              {{OutputDefinition}}

              {{Output1Definition}}

              {{Output2Definition}}

                  [Factory]
                  public static partial class SpecificFactory1;

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

        // lang=cs
        const string expectedAbstractFactory =
            """
            #nullable enable
            // <auto-generated/>
            
            namespace TestProject;
            
            partial class AbstractFactory
            {
                private readonly global::System.Collections.Immutable.ImmutableDictionary<global::System.Type, global::OpenTrackerNext.Core.Factories.IFactory<global::TestProject.IBaseInput, global::TestProject.IOutput>> _factories;
                
                public AbstractFactory()
                {
                    _factories = GetFactories();
                }
                
                public global::TestProject.IOutput Create(global::TestProject.IBaseInput input)
                {
                    return _factories[input.GetType()].Create(input);
                }
                
                private global::System.Collections.Immutable.ImmutableDictionary<global::System.Type, global::OpenTrackerNext.Core.Factories.IFactory<global::TestProject.IBaseInput, global::TestProject.IOutput>> GetFactories()
                {
                    var builder = global::System.Collections.Immutable.ImmutableDictionary.CreateBuilder<global::System.Type, global::OpenTrackerNext.Core.Factories.IFactory<global::TestProject.IBaseInput, global::TestProject.IOutput>>();
                    
                    builder.Add(typeof(global::TestProject.Input2), (global::OpenTrackerNext.Core.Factories.IFactory<global::TestProject.IBaseInput, global::TestProject.IOutput>)global::Splat.Locator.Current.GetService(typeof(global::TestProject.SpecificFactory2))!);
                
                    return builder.ToImmutable();
                }
            }
            """;
        
        // lang=cs
        const string expectedSpecificFactory2 =
            """
            #nullable enable
            // <auto-generated/>
            
            namespace TestProject;
            
            partial class SpecificFactory2
            {
                global::TestProject.IOutput global::OpenTrackerNext.Core.Factories.IFactory<global::TestProject.IBaseInput, global::TestProject.IOutput>.Create(global::TestProject.IBaseInput input)
                {
                    if (input is not global::TestProject.Input2 specificInput)
                    {
                        throw new global::System.ArgumentException("Input is not of the correct type.");
                    }
                    
                    return Create(specificInput);
                }
            }
            """;

        await GeneratorTest<FactoryGenerator>
            .VerifyGeneratorAsync(
                source,
                (FactoryGenerator.AbstractFactoryAttributeHintName, FactoryGenerator.AbstractFactoryAttributeSource),
                (FactoryGenerator.FactoryAttributeHintName, FactoryGenerator.FactoryAttributeSource),
                ("TestProject.SpecificFactory2.g.cs", expectedSpecificFactory2),
                ("TestProject.AbstractFactory.g.cs", expectedAbstractFactory));
    }

    [Fact]
    public async Task ShouldGenerateNothing_WhenSpecificFactoryIsNotPartial()
    {
        // lang=cs
        const string source =
            $$"""
              using OpenTrackerNext.Core.Factories;
              using OpenTrackerNext.Factories;
              
              {{InterfaceDefinitions}}
              
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
                  public partial class AbstractFactory : IFactory<IBaseInput, IOutput>;
              }
              """;

        // lang=cs
        const string expectedAbstractFactory =
            """
            #nullable enable
            // <auto-generated/>
            
            namespace TestProject;
            
            partial class AbstractFactory
            {
                private readonly global::System.Collections.Immutable.ImmutableDictionary<global::System.Type, global::OpenTrackerNext.Core.Factories.IFactory<global::TestProject.IBaseInput, global::TestProject.IOutput>> _factories;
                
                public AbstractFactory()
                {
                    _factories = GetFactories();
                }
                
                public global::TestProject.IOutput Create(global::TestProject.IBaseInput input)
                {
                    return _factories[input.GetType()].Create(input);
                }
                
                private global::System.Collections.Immutable.ImmutableDictionary<global::System.Type, global::OpenTrackerNext.Core.Factories.IFactory<global::TestProject.IBaseInput, global::TestProject.IOutput>> GetFactories()
                {
                    var builder = global::System.Collections.Immutable.ImmutableDictionary.CreateBuilder<global::System.Type, global::OpenTrackerNext.Core.Factories.IFactory<global::TestProject.IBaseInput, global::TestProject.IOutput>>();
                    
                    builder.Add(typeof(global::TestProject.Input2), (global::OpenTrackerNext.Core.Factories.IFactory<global::TestProject.IBaseInput, global::TestProject.IOutput>)global::Splat.Locator.Current.GetService(typeof(global::TestProject.SpecificFactory2))!);
                
                    return builder.ToImmutable();
                }
            }
            """;
        
        // lang=cs
        const string expectedSpecificFactory2 =
            """
            #nullable enable
            // <auto-generated/>
            
            namespace TestProject;
            
            partial class SpecificFactory2
            {
                global::TestProject.IOutput global::OpenTrackerNext.Core.Factories.IFactory<global::TestProject.IBaseInput, global::TestProject.IOutput>.Create(global::TestProject.IBaseInput input)
                {
                    if (input is not global::TestProject.Input2 specificInput)
                    {
                        throw new global::System.ArgumentException("Input is not of the correct type.");
                    }
                    
                    return Create(specificInput);
                }
            }
            """;

        await GeneratorTest<FactoryGenerator>
            .VerifyGeneratorAsync(
                source,
                new DiagnosticResult("CS0535", DiagnosticSeverity.Error)
                    .WithSpan(76, 37, 76, 82)
                    .WithArguments(
                        "TestProject.SpecificFactory1",
                        "OpenTrackerNext.Core.Factories.IFactory<TestProject.IBaseInput, TestProject.IOutput>.Create(TestProject.IBaseInput)"),
                (FactoryGenerator.AbstractFactoryAttributeHintName, FactoryGenerator.AbstractFactoryAttributeSource),
                (FactoryGenerator.FactoryAttributeHintName, FactoryGenerator.FactoryAttributeSource),
                ("TestProject.SpecificFactory2.g.cs", expectedSpecificFactory2),
                ("TestProject.AbstractFactory.g.cs", expectedAbstractFactory));
    }

    [Fact]
    public async Task ShouldGenerateNothing_WhenSpecificFactoryDoesNotImplementInterface()
    {
        // lang=cs
        const string source =
            $$"""
              using OpenTrackerNext.Core.Factories;
              using OpenTrackerNext.Factories;
              
              {{InterfaceDefinitions}}
              
              namespace TestProject
              {
              {{BaseInputDefinition}}

              {{Input1Definition}}

              {{Input2Definition}}

              {{OutputDefinition}}

              {{Output1Definition}}

              {{Output2Definition}}

                  [Factory]
                  public partial class SpecificFactory1;

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

        // lang=cs
        const string expectedAbstractFactory =
            """
            #nullable enable
            // <auto-generated/>
            
            namespace TestProject;
            
            partial class AbstractFactory
            {
                private readonly global::System.Collections.Immutable.ImmutableDictionary<global::System.Type, global::OpenTrackerNext.Core.Factories.IFactory<global::TestProject.IBaseInput, global::TestProject.IOutput>> _factories;
                
                public AbstractFactory()
                {
                    _factories = GetFactories();
                }
                
                public global::TestProject.IOutput Create(global::TestProject.IBaseInput input)
                {
                    return _factories[input.GetType()].Create(input);
                }
                
                private global::System.Collections.Immutable.ImmutableDictionary<global::System.Type, global::OpenTrackerNext.Core.Factories.IFactory<global::TestProject.IBaseInput, global::TestProject.IOutput>> GetFactories()
                {
                    var builder = global::System.Collections.Immutable.ImmutableDictionary.CreateBuilder<global::System.Type, global::OpenTrackerNext.Core.Factories.IFactory<global::TestProject.IBaseInput, global::TestProject.IOutput>>();
                    
                    builder.Add(typeof(global::TestProject.Input2), (global::OpenTrackerNext.Core.Factories.IFactory<global::TestProject.IBaseInput, global::TestProject.IOutput>)global::Splat.Locator.Current.GetService(typeof(global::TestProject.SpecificFactory2))!);
                
                    return builder.ToImmutable();
                }
            }
            """;
        
        // lang=cs
        const string expectedSpecificFactory2 =
            """
            #nullable enable
            // <auto-generated/>
            
            namespace TestProject;
            
            partial class SpecificFactory2
            {
                global::TestProject.IOutput global::OpenTrackerNext.Core.Factories.IFactory<global::TestProject.IBaseInput, global::TestProject.IOutput>.Create(global::TestProject.IBaseInput input)
                {
                    if (input is not global::TestProject.Input2 specificInput)
                    {
                        throw new global::System.ArgumentException("Input is not of the correct type.");
                    }
                    
                    return Create(specificInput);
                }
            }
            """;

        await GeneratorTest<FactoryGenerator>
            .VerifyGeneratorAsync(
                source,
                (FactoryGenerator.AbstractFactoryAttributeHintName, FactoryGenerator.AbstractFactoryAttributeSource),
                (FactoryGenerator.FactoryAttributeHintName, FactoryGenerator.FactoryAttributeSource),
                ("TestProject.SpecificFactory2.g.cs", expectedSpecificFactory2),
                ("TestProject.AbstractFactory.g.cs", expectedAbstractFactory));
    }
    
    [Fact]
    public async Task ShouldGenerateNothing_WhenAbstractFactoryIsStatic()
    {
        // lang=cs
        const string source =
            $$"""
              using OpenTrackerNext.Core.Factories;
              using OpenTrackerNext.Factories;
              
              {{InterfaceDefinitions}}
              
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
                  public static class AbstractFactory;
              }
              """;
        
        // lang=cs
        const string expectedSpecificFactory1 =
            """
            #nullable enable
            // <auto-generated/>
            
            namespace TestProject;
            
            partial class SpecificFactory1
            {
                global::TestProject.IOutput global::OpenTrackerNext.Core.Factories.IFactory<global::TestProject.IBaseInput, global::TestProject.IOutput>.Create(global::TestProject.IBaseInput input)
                {
                    if (input is not global::TestProject.Input1 specificInput)
                    {
                        throw new global::System.ArgumentException("Input is not of the correct type.");
                    }
                    
                    return Create(specificInput);
                }
            }
            """;

        // lang=cs
        const string expectedSpecificFactory2 =
            """
            #nullable enable
            // <auto-generated/>
            
            namespace TestProject;
            
            partial class SpecificFactory2
            {
                global::TestProject.IOutput global::OpenTrackerNext.Core.Factories.IFactory<global::TestProject.IBaseInput, global::TestProject.IOutput>.Create(global::TestProject.IBaseInput input)
                {
                    if (input is not global::TestProject.Input2 specificInput)
                    {
                        throw new global::System.ArgumentException("Input is not of the correct type.");
                    }
                    
                    return Create(specificInput);
                }
            }
            """;

        await GeneratorTest<FactoryGenerator>
            .VerifyGeneratorAsync(
                source,
                (FactoryGenerator.AbstractFactoryAttributeHintName, FactoryGenerator.AbstractFactoryAttributeSource),
                (FactoryGenerator.FactoryAttributeHintName, FactoryGenerator.FactoryAttributeSource),
                ("TestProject.SpecificFactory1.g.cs", expectedSpecificFactory1),
                ("TestProject.SpecificFactory2.g.cs", expectedSpecificFactory2));
    }
    
    [Fact]
    public async Task ShouldGenerateNothing_WhenAbstractFactoryIsNotPartial()
    {
        // lang=cs
        const string source =
            $$"""
              using OpenTrackerNext.Core.Factories;
              using OpenTrackerNext.Factories;
              
              {{InterfaceDefinitions}}
              
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
                  public class AbstractFactory : IFactory<IBaseInput, IOutput>;
              }
              """;
        
        // lang=cs
        const string expectedSpecificFactory1 =
            """
            #nullable enable
            // <auto-generated/>
            
            namespace TestProject;
            
            partial class SpecificFactory1
            {
                global::TestProject.IOutput global::OpenTrackerNext.Core.Factories.IFactory<global::TestProject.IBaseInput, global::TestProject.IOutput>.Create(global::TestProject.IBaseInput input)
                {
                    if (input is not global::TestProject.Input1 specificInput)
                    {
                        throw new global::System.ArgumentException("Input is not of the correct type.");
                    }
                    
                    return Create(specificInput);
                }
            }
            """;
        
        // lang=cs
        const string expectedSpecificFactory2 =
            """
            #nullable enable
            // <auto-generated/>
            
            namespace TestProject;
            
            partial class SpecificFactory2
            {
                global::TestProject.IOutput global::OpenTrackerNext.Core.Factories.IFactory<global::TestProject.IBaseInput, global::TestProject.IOutput>.Create(global::TestProject.IBaseInput input)
                {
                    if (input is not global::TestProject.Input2 specificInput)
                    {
                        throw new global::System.ArgumentException("Input is not of the correct type.");
                    }
                    
                    return Create(specificInput);
                }
            }
            """;
            

        await GeneratorTest<FactoryGenerator>
            .VerifyGeneratorAsync(
                source,
                new DiagnosticResult("CS0535", DiagnosticSeverity.Error)
                    .WithSpan(94, 36, 94, 65)
                    .WithArguments(
                        "TestProject.AbstractFactory",
                        "OpenTrackerNext.Core.Factories.IFactory<TestProject.IBaseInput, TestProject.IOutput>.Create(TestProject.IBaseInput)"),
                (FactoryGenerator.AbstractFactoryAttributeHintName, FactoryGenerator.AbstractFactoryAttributeSource),
                (FactoryGenerator.FactoryAttributeHintName, FactoryGenerator.FactoryAttributeSource),
                ("TestProject.SpecificFactory1.g.cs", expectedSpecificFactory1),
                ("TestProject.SpecificFactory2.g.cs", expectedSpecificFactory2));
    }
    
    [Fact]
    public async Task ShouldGenerateNothing_WhenAbstractFactoryDoesNotImplementInterface()
    {
        // lang=cs
        const string source =
            $$"""
              using OpenTrackerNext.Core.Factories;
              using OpenTrackerNext.Factories;
              
              {{InterfaceDefinitions}}
              
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
                  public partial class AbstractFactory;
              }
              """;
        
        // lang=cs
        const string expectedSpecificFactory1 =
            """
            #nullable enable
            // <auto-generated/>
            
            namespace TestProject;
            
            partial class SpecificFactory1
            {
                global::TestProject.IOutput global::OpenTrackerNext.Core.Factories.IFactory<global::TestProject.IBaseInput, global::TestProject.IOutput>.Create(global::TestProject.IBaseInput input)
                {
                    if (input is not global::TestProject.Input1 specificInput)
                    {
                        throw new global::System.ArgumentException("Input is not of the correct type.");
                    }
                    
                    return Create(specificInput);
                }
            }
            """;
        
        // lang=cs
        const string expectedSpecificFactory2 =
            """
            #nullable enable
            // <auto-generated/>
            
            namespace TestProject;
            
            partial class SpecificFactory2
            {
                global::TestProject.IOutput global::OpenTrackerNext.Core.Factories.IFactory<global::TestProject.IBaseInput, global::TestProject.IOutput>.Create(global::TestProject.IBaseInput input)
                {
                    if (input is not global::TestProject.Input2 specificInput)
                    {
                        throw new global::System.ArgumentException("Input is not of the correct type.");
                    }
                    
                    return Create(specificInput);
                }
            }
            """;

        await GeneratorTest<FactoryGenerator>
            .VerifyGeneratorAsync(
                source,
                (FactoryGenerator.AbstractFactoryAttributeHintName, FactoryGenerator.AbstractFactoryAttributeSource),
                (FactoryGenerator.FactoryAttributeHintName, FactoryGenerator.FactoryAttributeSource),
                ("TestProject.SpecificFactory1.g.cs", expectedSpecificFactory1),
                ("TestProject.SpecificFactory2.g.cs", expectedSpecificFactory2));
    }

    [Fact]
    public async Task ShouldGenerateNothing_WhenAbstractFactoryHasNoSpecificFactories()
    {
        // lang=cs
        const string source =
            $$"""
              using OpenTrackerNext.Core.Factories;
              using OpenTrackerNext.Factories;
              
              {{InterfaceDefinitions}}
              
              namespace TestProject
              {
              {{BaseInputDefinition}}

              {{Input1Definition}}

              {{Input2Definition}}

              {{OutputDefinition}}

              {{Output1Definition}}

              {{Output2Definition}}

                  [AbstractFactory]
                  public partial class AbstractFactory : IFactory<IBaseInput, IOutput>;
              }
              """;
        
        // lang=cs
        const string expected =
            """
            #nullable enable
            // <auto-generated/>
            
            namespace TestProject;
            
            partial class AbstractFactory
            {
                private readonly global::System.Collections.Immutable.ImmutableDictionary<global::System.Type, global::OpenTrackerNext.Core.Factories.IFactory<global::TestProject.IBaseInput, global::TestProject.IOutput>> _factories;
                
                public AbstractFactory()
                {
                    _factories = GetFactories();
                }
                
                public global::TestProject.IOutput Create(global::TestProject.IBaseInput input)
                {
                    return _factories[input.GetType()].Create(input);
                }
                
                private global::System.Collections.Immutable.ImmutableDictionary<global::System.Type, global::OpenTrackerNext.Core.Factories.IFactory<global::TestProject.IBaseInput, global::TestProject.IOutput>> GetFactories()
                {
                    var builder = global::System.Collections.Immutable.ImmutableDictionary.CreateBuilder<global::System.Type, global::OpenTrackerNext.Core.Factories.IFactory<global::TestProject.IBaseInput, global::TestProject.IOutput>>();
                    
                
                    return builder.ToImmutable();
                }
            }
            """;
        
        await GeneratorTest<FactoryGenerator>
            .VerifyGeneratorAsync(
                source,
                (FactoryGenerator.AbstractFactoryAttributeHintName, FactoryGenerator.AbstractFactoryAttributeSource),
                (FactoryGenerator.FactoryAttributeHintName, FactoryGenerator.FactoryAttributeSource),
                ("TestProject.AbstractFactory.g.cs", expected));
    }

    [Fact]
    public async Task ShouldGenerateExpected_WhenAbstractFactoryWithMultipleSpecificFactories()
    {
        // lang=cs
        const string source =
            $$"""
              using OpenTrackerNext.Core.Factories;
              using OpenTrackerNext.Factories;
              
              {{InterfaceDefinitions}}
              
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

        // lang=cs
        const string expectedAbstractFactory =
            """
            #nullable enable
            // <auto-generated/>
            
            namespace TestProject;
            
            partial class AbstractFactory
            {
                private readonly global::System.Collections.Immutable.ImmutableDictionary<global::System.Type, global::OpenTrackerNext.Core.Factories.IFactory<global::TestProject.IBaseInput, global::TestProject.IOutput>> _factories;
                
                public AbstractFactory()
                {
                    _factories = GetFactories();
                }
                
                public global::TestProject.IOutput Create(global::TestProject.IBaseInput input)
                {
                    return _factories[input.GetType()].Create(input);
                }
                
                private global::System.Collections.Immutable.ImmutableDictionary<global::System.Type, global::OpenTrackerNext.Core.Factories.IFactory<global::TestProject.IBaseInput, global::TestProject.IOutput>> GetFactories()
                {
                    var builder = global::System.Collections.Immutable.ImmutableDictionary.CreateBuilder<global::System.Type, global::OpenTrackerNext.Core.Factories.IFactory<global::TestProject.IBaseInput, global::TestProject.IOutput>>();
                    
                    builder.Add(typeof(global::TestProject.Input1), (global::OpenTrackerNext.Core.Factories.IFactory<global::TestProject.IBaseInput, global::TestProject.IOutput>)global::Splat.Locator.Current.GetService(typeof(global::TestProject.SpecificFactory1))!);
                    builder.Add(typeof(global::TestProject.Input2), (global::OpenTrackerNext.Core.Factories.IFactory<global::TestProject.IBaseInput, global::TestProject.IOutput>)global::Splat.Locator.Current.GetService(typeof(global::TestProject.SpecificFactory2))!);
                
                    return builder.ToImmutable();
                }
            }
            """;
        
        // lang=cs
        const string expectedSpecificFactory1 =
            """
            #nullable enable
            // <auto-generated/>
            
            namespace TestProject;
            
            partial class SpecificFactory1
            {
                global::TestProject.IOutput global::OpenTrackerNext.Core.Factories.IFactory<global::TestProject.IBaseInput, global::TestProject.IOutput>.Create(global::TestProject.IBaseInput input)
                {
                    if (input is not global::TestProject.Input1 specificInput)
                    {
                        throw new global::System.ArgumentException("Input is not of the correct type.");
                    }
                    
                    return Create(specificInput);
                }
            }
            """;
        
        // lang=cs
        const string expectedSpecificFactory2 =
            """
            #nullable enable
            // <auto-generated/>
            
            namespace TestProject;
            
            partial class SpecificFactory2
            {
                global::TestProject.IOutput global::OpenTrackerNext.Core.Factories.IFactory<global::TestProject.IBaseInput, global::TestProject.IOutput>.Create(global::TestProject.IBaseInput input)
                {
                    if (input is not global::TestProject.Input2 specificInput)
                    {
                        throw new global::System.ArgumentException("Input is not of the correct type.");
                    }
                    
                    return Create(specificInput);
                }
            }
            """;

        await GeneratorTest<FactoryGenerator>
            .VerifyGeneratorAsync(
                source,
                (FactoryGenerator.AbstractFactoryAttributeHintName, FactoryGenerator.AbstractFactoryAttributeSource),
                (FactoryGenerator.FactoryAttributeHintName, FactoryGenerator.FactoryAttributeSource),
                ("TestProject.SpecificFactory1.g.cs", expectedSpecificFactory1),
                ("TestProject.SpecificFactory2.g.cs", expectedSpecificFactory2),
                ("TestProject.AbstractFactory.g.cs", expectedAbstractFactory));
    }
}