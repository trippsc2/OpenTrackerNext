using Avalonia;
using Avalonia.Headless;
using Avalonia.ReactiveUI;

namespace OpenTrackerNext.Editor.UnitTests;

public class TestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions())
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
    }
}