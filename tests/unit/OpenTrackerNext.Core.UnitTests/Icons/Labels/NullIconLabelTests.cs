using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using OpenTrackerNext.Core.Icons.Labels;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Icons.Labels;

[ExcludeFromCodeCoverage]
public sealed class NullIconLabelTests : IDisposable
{
    public void Dispose()
    {
        NullIconLabel.Instance.Dispose();
    }
    
    [Fact]
    public void Text_ShouldReturnEmptyString()
    {
        NullIconLabel.Instance
            .Text
            .Should()
            .BeNull();
    }
    
    [Fact]
    public void Position_ShouldReturnBottomRight()
    {
        NullIconLabel.Instance
            .Position
            .Should()
            .Be(IconLabelPosition.BottomRight);
    }
}