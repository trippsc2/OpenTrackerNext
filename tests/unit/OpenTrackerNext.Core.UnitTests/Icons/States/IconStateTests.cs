using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NSubstitute;
using OpenTrackerNext.Core.Icons.Labels;
using OpenTrackerNext.Core.Icons.States;
using OpenTrackerNext.Core.Images;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Icons.States;

[ExcludeFromCodeCoverage]
public sealed class IconStateTests
{
    private readonly IIconLabel _label = Substitute.For<IIconLabel>();
    private readonly IReadOnlyImageFile _image = Substitute.For<IReadOnlyImageFile>();

    private readonly IconState _subject;

    public IconStateTests()
    {
        _subject = new IconState
        {
            Label = _label,
            Image = _image
        };
    }
    
    [Fact]
    public void Label_ShouldEqualValueFromConstructor()
    {
        _subject.Label
            .Should()
            .Be(_label);
    }
    
    [Fact]
    public void Image_ShouldEqualValueFromConstructor()
    {
        _subject.Image
            .Should()
            .Be(_image);
    }
    
    [Fact]
    public void Dispose_ShouldCallDisposeOnLabel()
    {
        _subject.Dispose();
        
        _label.Received(1).Dispose();
    }
}