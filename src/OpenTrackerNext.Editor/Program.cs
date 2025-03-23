using System;
using Avalonia;
using Avalonia.ReactiveUI;
using OpenTrackerNext.Core.Generated;
using OpenTrackerNext.Editor.Generated;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.MaterialDesign;
using Splat;

namespace OpenTrackerNext.Editor;

/// <summary>
/// Entrypoint class.
/// </summary>
/// <remarks>
/// Must remain non-static and public for the visual designer to work.
/// </remarks>
// ReSharper disable once ClassNeverInstantiated.Global
public sealed class Program
{
    /// <summary>
    /// Entrypoint method.
    /// </summary>
    /// <param name="args">
    /// The command line arguments.
    /// </param>
    /// <remarks>
    /// Initialization code. Don't use any Avalonia, third-party APIs or any SynchronizationContext-reliant code before
    /// AppMain is called: things aren't initialized yet and stuff might break.
    /// </remarks>
    [STAThread]
    public static void Main(string[] args)
    {
        IconProvider.Current.Register<MaterialDesignIconProvider>();

        Locator.CurrentMutable
            .RegisterTypesFromOpenTrackerNextCore()
            .RegisterTypesFromOpenTrackerNextEditor();
        
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    /// <summary>
    /// Avalonia configuration method.
    /// </summary>
    /// <returns>
    /// The <see cref="AppBuilder"/> instance.
    /// </returns>
    /// <remarks>
    /// Avalonia configuration, don't remove; also used by visual designer.
    /// Must remain public for the visual designer to work.
    /// </remarks>
    // ReSharper disable once MemberCanBePrivate.Global
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
    }
}