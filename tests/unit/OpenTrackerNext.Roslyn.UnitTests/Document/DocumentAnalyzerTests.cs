using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using OpenTrackerNext.Roslyn.Document;
using Xunit;

namespace OpenTrackerNext.Roslyn.UnitTests.Document;

[ExcludeFromCodeCoverage]
public sealed class DocumentAnalyzerTests
{
    // lang=cs
    private const string DocumentAttributeSource =
        """
        namespace OpenTrackerNext.Document
        {
            [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
            [global::System.AttributeUsage(global::System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
            internal sealed class DocumentAttribute : global::System.Attribute;
        }
        
        """;
    
    [Fact]
    public async Task ShouldProduceError_WhenClassIsNotPartial()
    {
        // lang=cs
        const string source =
            $$"""
              using OpenTrackerNext.Document;
              
              {{DocumentAttributeSource}} 
              
              namespace TestProject
              {
                  [Document]
                  public class TestObject
                  {
                  }
              }
              """;

        await AnalyzerTest<DocumentAnalyzer>
            .VerifyAnalyzerAsync(
                source,
                new DiagnosticResult(DocumentAnalyzer.DocumentAttributeOnNonPartialType)
                    .WithSpan(14, 18, 14, 28)
                    .WithArguments("TestObject"));
    }
    
    [Fact]
    public async Task ShouldProduceError_WhenClassIsStatic()
    {
        // lang=cs
        const string source =
            $$"""
              using OpenTrackerNext.Document;
              
              {{DocumentAttributeSource}}
              
              namespace TestProject
              {
                  [Document]
                  public static partial class TestObject
                  {
                  }
              }
              """;

        await AnalyzerTest<DocumentAnalyzer>
            .VerifyAnalyzerAsync(
                source,
                new DiagnosticResult(DocumentAnalyzer.DocumentAttributeOnStaticType)
                    .WithSpan(14, 33, 14, 43)
                    .WithArguments("TestObject"));
    }
}