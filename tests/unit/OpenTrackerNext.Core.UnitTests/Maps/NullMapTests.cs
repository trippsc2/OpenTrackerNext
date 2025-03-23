using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using OpenTrackerNext.Core.Images;
using OpenTrackerNext.Core.Maps;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Maps;

[ExcludeFromCodeCoverage]
public sealed class NullMapTests : IDisposable
{
    public void Dispose()
    {
        NullMap.Instance.Dispose();
    }
    
    [Fact]
    public void Image_ShouldReturnNullImageFile()
    {
        NullMap.Instance
            .Image
            .Should()
            .Be(NullImageFile.Instance);
    }
}