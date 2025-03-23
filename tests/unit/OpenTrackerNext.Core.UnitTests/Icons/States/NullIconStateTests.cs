using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using OpenTrackerNext.Core.Icons.Labels;
using OpenTrackerNext.Core.Icons.States;
using OpenTrackerNext.Core.Images;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Icons.States;

[ExcludeFromCodeCoverage]
public sealed class NullIconStateTests : IDisposable
{
    public void Dispose()
    {
        NullIconState.Instance.Dispose();
    }
    
    [Fact]
    public void Label_ShouldReturnNullIconLabel()
    {
        NullIconState.Instance
            .Label
            .Should()
            .Be(NullIconLabel.Instance);
    }
    
    [Fact]
    public void Image_ShouldReturnNullImageFile()
    {
        NullIconState.Instance
            .Image
            .Should()
            .Be(NullImageFile.Instance);
    }
}