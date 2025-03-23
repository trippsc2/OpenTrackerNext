using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using OpenTrackerNext.Core.Layouts;
using OpenTrackerNext.Core.UIPanes;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.UIPanes;

[ExcludeFromCodeCoverage]
public sealed class NullUIPaneTests
{
    [Fact]
    public void Title_ShouldReturnEmpty()
    {
        NullUIPane.Instance
            .Title
            .Should()
            .Be(string.Empty);
    }
    
    [Fact]
    public void Body_ShouldReturnNullLayout()
    {
        NullUIPane.Instance
            .Body
            .Should()
            .BeSameAs(NullLayout.Instance);
    }
}