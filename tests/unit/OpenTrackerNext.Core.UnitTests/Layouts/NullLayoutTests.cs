using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia;
using FluentAssertions;
using OpenTrackerNext.Core.Layouts;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Layouts;

[ExcludeFromCodeCoverage]
public sealed class NullLayoutTests : IDisposable
{
    public void Dispose()
    {
        NullLayout.Instance.Dispose();
    }
    
    [Fact]
    public void Margin_ShouldReturnDefault()
    {
        NullLayout.Instance
            .Margin
            .Should()
            .Be(new Thickness());
    }
}