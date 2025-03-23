using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using OpenTrackerNext.Editor.ViewModels.Documents;
using ReactiveUI;
using Splat;
using Xunit;

namespace OpenTrackerNext.Editor.UnitTests.Views.Documents;

[ExcludeFromCodeCoverage]
public sealed class DocumentLayoutViewTests
{
    private readonly AvaloniaFixture _avaloniaFixture;
    
    public DocumentLayoutViewTests(AvaloniaFixture avaloniaFixture)
    {
        _avaloniaFixture = avaloniaFixture;
    }
    
    [Fact]
    public async Task SplatResolve_ShouldResolveToTransientInstance()
    {
        await _avaloniaFixture.Session.Dispatch(
            () =>
            {
                var subject1 = Locator.Current.GetService<IViewFor<DocumentLayoutViewModel>>()!;
                var subject2 = Locator.Current.GetService<IViewFor<DocumentLayoutViewModel>>()!;

                subject1.Should().NotBeSameAs(subject2);
            },
            TestContext.Current.CancellationToken);
    }
}