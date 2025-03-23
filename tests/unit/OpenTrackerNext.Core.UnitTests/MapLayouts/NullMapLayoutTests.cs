using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using OpenTrackerNext.Core.MapLayouts;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.MapLayouts;

[ExcludeFromCodeCoverage]
public sealed class NullMapLayoutTests
{
    [Fact]
    public void Name_ShouldReturnEmptyString()
    {
        NullMapLayout.Instance
            .Name
            .Should()
            .BeEmpty();
    }
    
    [Fact]
    public void Maps_ShouldReturnEmpty()
    {
        NullMapLayout.Instance
            .Maps
            .Should()
            .BeEmpty();
    }
}