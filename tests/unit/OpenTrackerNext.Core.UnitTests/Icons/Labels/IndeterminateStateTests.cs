using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using OpenTrackerNext.Core.Icons.Labels;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Icons.Labels;

[ExcludeFromCodeCoverage]
public sealed class IndeterminateStateTests
{
    [Theory]
    [InlineData(nameof(IndeterminateState.None))]
    [InlineData(nameof(IndeterminateState.Minimum))]
    [InlineData(nameof(IndeterminateState.Maximum))]
    public void DisplayName_ShouldReturnExpected(string expected)
    {
        var subject = IndeterminateState.FromName(expected);
        
        subject.DisplayName
            .Should()
            .Be(expected);
    }
}