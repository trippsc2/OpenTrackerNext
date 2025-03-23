using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using OpenTrackerNext.Editor.ViewModels.Tools;
using ReactiveUI;
using Splat;
using Xunit;

namespace OpenTrackerNext.Editor.UnitTests.Views.Tools;

[ExcludeFromCodeCoverage]
public sealed class ToolLayoutViewTests
{
    private readonly AvaloniaFixture _avaloniaFixture;
    
    public ToolLayoutViewTests(AvaloniaFixture avaloniaFixture)
    {
        _avaloniaFixture = avaloniaFixture;
    }
    
    [Fact]
    public async Task SplatResolve_ShouldResolveToTransientInstance()
    {
        await _avaloniaFixture.Session.Dispatch(
            () =>
            {
                var subject1 = Locator.Current.GetService<IViewFor<ToolLayoutViewModel>>()!;
                var subject2 = Locator.Current.GetService<IViewFor<ToolLayoutViewModel>>()!;

                subject1.Should().NotBeSameAs(subject2);
            },
            TestContext.Current.CancellationToken);
    }
}