using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using OpenTrackerNext.Editor.ViewModels.Tools;
using ReactiveUI;
using Splat;
using Xunit;

namespace OpenTrackerNext.Editor.UnitTests.Views.Tools;

[ExcludeFromCodeCoverage]
public sealed class ToolBarViewTests
{
    private readonly AvaloniaFixture _avaloniaFixture;
    
    public ToolBarViewTests(AvaloniaFixture avaloniaFixture)
    {
        _avaloniaFixture = avaloniaFixture;
    }
    
    [Fact]
    public async Task SplatResolve_ShouldResolveToTransientInstance()
    {
        await _avaloniaFixture.Session.Dispatch(
            () =>
            {
                var subject1 = Locator.Current.GetService<IViewFor<ToolBarViewModel>>()!;
                var subject2 = Locator.Current.GetService<IViewFor<ToolBarViewModel>>()!;

                subject1.Should().NotBeSameAs(subject2);
            },
            TestContext.Current.CancellationToken);
    }
}