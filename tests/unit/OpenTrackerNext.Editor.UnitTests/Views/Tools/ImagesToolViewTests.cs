using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using OpenTrackerNext.Editor.ViewModels.Tools;
using OpenTrackerNext.Editor.Views.Tools;
using ReactiveUI;
using Splat;
using Xunit;

namespace OpenTrackerNext.Editor.UnitTests.Views.Tools;

[ExcludeFromCodeCoverage]
public sealed class ImagesToolViewTests
{
    private readonly AvaloniaFixture _avaloniaFixture;
    
    public ImagesToolViewTests(AvaloniaFixture avaloniaFixture)
    {
        _avaloniaFixture = avaloniaFixture;
    }
    
    [Fact]
    public async Task SplatResolve_ShouldResolveToTransientInstance()
    {
        await _avaloniaFixture.Session.Dispatch(
            () =>
            {
                var subject1 = Locator.Current.GetService<IViewFor<ImagesToolViewModel>>()!;
                var subject2 = Locator.Current.GetService<IViewFor<ImagesToolViewModel>>()!;

                subject1.Should().BeOfType<ImagesToolView>();
                subject2.Should().BeOfType<ImagesToolView>();

                subject1.Should().NotBeSameAs(subject2);
            },
            TestContext.Current.CancellationToken); 
    }
}