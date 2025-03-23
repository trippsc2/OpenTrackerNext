using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using FluentAssertions;
using NSubstitute;
using OpenTrackerNext.Core.Icons.States;
using OpenTrackerNext.Core.Icons.Static;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Icons.Static;

[ExcludeFromCodeCoverage]
public sealed class StaticIconTests : IDisposable
{
    private readonly CompositeDisposable _disposables = new();

    public void Dispose()
    {
        _disposables.Dispose();
    }

    [Fact]
    public void CurrentState_ShouldInitializeFromConstructor()
    {
        var iconState = Substitute.For<IIconState>();
        
        var subject = new StaticIcon { CurrentState = iconState };

        subject.DisposeWith(_disposables);

        subject.CurrentState
            .Should()
            .Be(iconState);
    }

    [Fact]
    public void OnLeftClick_ShouldDoNothing()
    {
        var iconState = Substitute.For<IIconState>();
        
        var subject = new StaticIcon { CurrentState = iconState };
        
        subject.DisposeWith(_disposables);

        subject.OnLeftClick();
        
        subject.CurrentState
            .Should()
            .Be(iconState);
    }
    
    [Fact]
    public void OnRightClick_ShouldDoNothing()
    {
        var iconState = Substitute.For<IIconState>();
        
        var subject = new StaticIcon { CurrentState = iconState };
        
        subject.DisposeWith(_disposables);

        subject.OnRightClick();
        
        subject.CurrentState
            .Should()
            .Be(iconState);
    }
}