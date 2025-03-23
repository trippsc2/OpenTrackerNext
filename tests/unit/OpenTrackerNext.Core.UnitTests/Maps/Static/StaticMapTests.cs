using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using FluentAssertions;
using NSubstitute;
using OpenTrackerNext.Core.Images;
using OpenTrackerNext.Core.Maps.Static;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Maps.Static;

[ExcludeFromCodeCoverage]
public sealed class StaticMapTests : IDisposable
{
    private readonly CompositeDisposable _disposables = new();
    
    public void Dispose()
    {
        _disposables.Dispose();
    }
    
    [Fact]
    public void Image_ShouldBeSetAtInitialization()
    {
        var image = Substitute.For<IReadOnlyImageFile>();
        
        var subject = new StaticMap
            {
                Image = image
            }
            .DisposeWith(_disposables);
        
        subject.Image
            .Should()
            .Be(image);
    }
}