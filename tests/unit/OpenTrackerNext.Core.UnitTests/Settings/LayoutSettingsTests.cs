using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using OpenTrackerNext.Core.Settings;
using Splat;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Settings;

[ExcludeFromCodeCoverage]
public sealed class LayoutSettingsTests
{
    [Fact]
    public void SplatResolve_ShouldResolveToSingleInstance()
    {
        var subject1 = Locator.Current.GetService<LayoutSettings>()!;
        var subject2 = Locator.Current.GetService<LayoutSettings>()!;

        subject1
            .Should()
            .BeSameAs(subject2);
    }
    
    [Fact]
    public void SplatResolve_ShouldResolveToLazySingleInstance()
    {
        var lazy = Locator.Current.GetService<Lazy<LayoutSettings>>()!;
        var subject = Locator.Current.GetService<LayoutSettings>()!;

        lazy.Value
            .Should()
            .BeSameAs(subject);
    }
}