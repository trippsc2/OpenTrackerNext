using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using OpenTrackerNext.Editor.ViewModels.Tools;
using ReactiveUI;
using Splat;
using Xunit;

namespace OpenTrackerNext.Editor.UnitTests.Views.Tools;

[ExcludeFromCodeCoverage]
public sealed class ToolViewTests
{
    private readonly AvaloniaFixture _avaloniaFixture;
    
    public ToolViewTests(AvaloniaFixture avaloniaFixture)
    {
        _avaloniaFixture = avaloniaFixture;
    }
    
    [Fact]
    public async Task SplatResolve_ShouldResolveToTransientInstance()
    {
        await _avaloniaFixture.Session.Dispatch(
            () =>
            {
                var subject1 = Locator.Current.GetService<IViewFor<ToolViewModel>>()!;
                var subject2 = Locator.Current.GetService<IViewFor<ToolViewModel>>()!;

                subject1.Should().NotBeSameAs(subject2);
            },
            TestContext.Current.CancellationToken);
    }
}