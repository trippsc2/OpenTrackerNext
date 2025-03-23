using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia;
using FluentAssertions;
using OpenTrackerNext.Core.Images;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Images;

[ExcludeFromCodeCoverage]
public sealed class NullImageFileTests
{
    [Fact]
    public void Id_ShouldReturnExpected()
    {
        NullImageFile.Instance
            .Id
            .Should()
            .Be(ImageId.Empty);
    }
    
    [Fact]
    public void FriendlyId_ShouldReturnExpected()
    {
        NullImageFile.Instance
            .FriendlyId
            .Should()
            .Be(NullImageFile.FriendlyIdConstant);
    }

    [Fact]
    public void FriendlyName_ShouldThrowNotSupportedException_WhenSet()
    {
        NullImageFile.Instance
            .Invoking(x => x.FriendlyId = "test")
            .Should()
            .Throw<NotSupportedException>();
    }
    
    [Fact]
    public void Delete_ShouldThrowNotSupportedException()
    {
        NullImageFile.Instance
            .Invoking(i => i.Delete())
            .Should()
            .Throw<NotSupportedException>();
    }

    // [Fact]
    // public void GetBitmap_ShouldReturnEmptyBitmap()
    // {
    //     NullImageFile.Instance
    //         .GetBitmap()
    //         .Size
    //         .Should()
    //         .BeEquivalentTo(new Size(1, 1));
    // }
}