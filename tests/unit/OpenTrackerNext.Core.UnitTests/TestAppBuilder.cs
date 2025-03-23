using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Headless;
using Avalonia.ReactiveUI;
using OpenTrackerNext.Core.UnitTests;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.MaterialDesign;

[assembly: AvaloniaTestApplication(typeof(TestAppBuilder))]

namespace OpenTrackerNext.Core.UnitTests;

[ExcludeFromCodeCoverage]
public sealed class TestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp()
    {
        IconProvider.Current.Register<MaterialDesignIconProvider>();
        
        return AppBuilder
            .Configure<Application>()
            .UseReactiveUI()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions());
    }
}