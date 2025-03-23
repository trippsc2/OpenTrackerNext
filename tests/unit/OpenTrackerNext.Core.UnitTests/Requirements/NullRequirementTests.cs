using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using OpenTrackerNext.Core.Requirements;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Requirements;

[ExcludeFromCodeCoverage]
public sealed class NullRequirementTests : IDisposable
{
    public void Dispose()
    {
        NullRequirement.Instance.Dispose();
    }

    [Fact]
    public void IsMet_ShouldReturnTrue()
    {
        NullRequirement.Instance
            .IsMet
            .Should()
            .BeTrue();
    }
}