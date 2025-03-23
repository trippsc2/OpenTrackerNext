using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Headless;
using OpenTrackerNext.Core.UnitTests;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.MaterialDesign;
using Xunit;

[assembly: AssemblyFixture(typeof(AvaloniaFixture))]

namespace OpenTrackerNext.Core.UnitTests;

[ExcludeFromCodeCoverage]
public sealed class AvaloniaFixture : IDisposable
{
    public AvaloniaFixture()
    {
        IconProvider.Current.Register<MaterialDesignIconProvider>();

        Session = HeadlessUnitTestSession.StartNew(typeof(App));
    }

    public HeadlessUnitTestSession Session { get; }

    public void Dispose()
    {
        Session.Dispose();
    }
}