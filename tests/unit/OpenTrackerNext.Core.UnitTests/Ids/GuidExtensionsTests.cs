using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using OpenTrackerNext.Core.Ids;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Ids;

[ExcludeFromCodeCoverage]
public sealed class GuidExtensionsTests
{
    [Fact]
    public void ToLowercaseString_ShouldReturnExpected()
    {
        var subject = Guid.NewGuid();
        var expected = subject.ToString().ToLowerInvariant();

        subject.ToLowercaseString()
            .Should()
            .Be(expected);
    }
}