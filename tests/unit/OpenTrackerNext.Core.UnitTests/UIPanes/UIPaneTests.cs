using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NSubstitute;
using OpenTrackerNext.Core.Layouts;
using OpenTrackerNext.Core.UIPanes;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.UIPanes;

[ExcludeFromCodeCoverage]
public sealed class UIPaneTests
{
    [Theory]
    [InlineData("Test")]
    [InlineData("Test2")]
    public void Title_ShouldReturnExpectedValue(string expected)
    {
        var subject = new UIPane
        {
            Title = expected,
            Body = NullLayout.Instance
        };

        subject.Title
            .Should()
            .Be(expected);
    }
    
    [Fact]
    public void Body_ShouldReturnExpectedValue()
    {
        var expected = Substitute.For<ILayout>();
        
        var subject = new UIPane
        {
            Title = string.Empty,
            Body = expected
        };

        subject.Body
            .Should()
            .BeSameAs(expected);
    }
}