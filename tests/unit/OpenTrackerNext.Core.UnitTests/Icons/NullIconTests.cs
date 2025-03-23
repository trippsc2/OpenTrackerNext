using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using OpenTrackerNext.Core.Icons;
using OpenTrackerNext.Core.Icons.States;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Icons;

[ExcludeFromCodeCoverage]
public sealed class NullIconTests : IDisposable
{
    public void Dispose()
    {
        NullIcon.Instance.Dispose();
    }
    
    [Fact]
    public void CurrentState_ShouldReturnNullIconState()
    {
        NullIcon.Instance
            .CurrentState
            .Should()
            .BeSameAs(NullIconState.Instance);
    }
    
    [Fact]
    public void OnLeftClick_ShouldDoNothing()
    {
        NullIcon.Instance.OnLeftClick();
        
        NullIcon.Instance
            .CurrentState
            .Should()
            .BeSameAs(NullIconState.Instance);
    }
    
    [Fact]
    public void OnRightClick_ShouldDoNothing()
    {
        NullIcon.Instance.OnRightClick();
        
        NullIcon.Instance
            .CurrentState
            .Should()
            .BeSameAs(NullIconState.Instance);
    }
}