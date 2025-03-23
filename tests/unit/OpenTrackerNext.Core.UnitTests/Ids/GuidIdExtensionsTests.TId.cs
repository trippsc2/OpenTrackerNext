using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using OpenTrackerNext.Core.Ids;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Ids;

[ExcludeFromCodeCoverage]
public abstract class GuidIdExtensionsTests<TId>
    where TId : struct, IGuidId<TId>
{
    [Fact]
    public void ToLowercaseString_ShouldReturnExpected()
    {
        var subject = TId.New();
        var expected = subject.Value
            .ToString()
            .ToLowerInvariant();

        subject.ToLowercaseString()
            .Should()
            .Be(expected);
    }
}