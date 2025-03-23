using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using OpenTrackerNext.Core.Entities;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Entities;

[ExcludeFromCodeCoverage]
public sealed class NullEntityTests
{
    [Fact]
    public void Minimum_ShouldReturnExpected()
    {
        NullEntity.Instance
            .Minimum
            .Should()
            .Be(0);
    }
    
    [Fact]
    public void Starting_ShouldReturnExpected()
    {
        NullEntity.Instance
            .Starting
            .Should()
            .Be(0);
    }
    
    [Fact]
    public void Maximum_ShouldReturnExpected()
    {
        NullEntity.Instance
            .Maximum
            .Should()
            .Be(0);
    }
    
    [Fact]
    public void Current_ShouldReturnExpected()
    {
        NullEntity.Instance
            .Current
            .Should()
            .Be(0);
    }
    
    [Fact]
    public void Current_ShouldThrowNotSupportedException_WhenSet()
    {
        NullEntity.Instance
            .Invoking(e => e.Current = 1)
            .Should()
            .Throw<NotSupportedException>();
    }
    
    [Fact]
    public void Reset_ShouldThrowNotSupportedException()
    {
        NullEntity.Instance.Reset();
        
        NullEntity.Instance
            .Current
            .Should()
            .Be(0);
    }
}