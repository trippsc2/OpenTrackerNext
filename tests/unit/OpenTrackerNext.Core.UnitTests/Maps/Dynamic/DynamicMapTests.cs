using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using FluentAssertions;
using NSubstitute;
using OpenTrackerNext.Core.Images;
using OpenTrackerNext.Core.Maps.Dynamic;
using OpenTrackerNext.Core.Requirements;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Maps.Dynamic;

[ExcludeFromCodeCoverage]
public sealed class DynamicMapTests : IDisposable
{
    private readonly CompositeDisposable _disposables = new();
    private readonly IRequirement _requirement = Substitute.For<IRequirement>();
    private readonly IReadOnlyImageFile _metImage = Substitute.For<IReadOnlyImageFile>();
    private readonly IReadOnlyImageFile _unmetImage = Substitute.For<IReadOnlyImageFile>();

    private readonly DynamicMap _subject;

    public DynamicMapTests()
    {
        _subject = new DynamicMap(_requirement, _metImage, _unmetImage);
        
        _subject.DisposeWith(_disposables);
    }
    
    public void Dispose()
    {
        _disposables.Dispose();
    }

    [Fact]
    public void Image_ShouldReturnExpected_WhenRequirementIsMet()
    {
        _requirement.IsMet.Returns(true);

        _requirement.PropertyChanged += Raise
            .Event<PropertyChangedEventHandler>(
                _requirement,
                new PropertyChangedEventArgs(nameof(IRequirement.IsMet)));

        _subject.Image
            .Should()
            .BeSameAs(_metImage);
    }
    
    [Fact]
    public void Image_ShouldReturnExpected_WhenRequirementIsNotMet()
    {
        _requirement.IsMet.Returns(false);

        _requirement.PropertyChanged += Raise
            .Event<PropertyChangedEventHandler>(
                _requirement,
                new PropertyChangedEventArgs(nameof(IRequirement.IsMet)));

        _subject.Image
            .Should()
            .BeSameAs(_unmetImage);
    }
}