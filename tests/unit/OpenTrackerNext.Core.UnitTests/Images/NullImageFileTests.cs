using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Avalonia;
using FluentAssertions;
using OpenTrackerNext.Core.Images;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Images;

[ExcludeFromCodeCoverage]
public sealed class NullImageFileTests
{
    private readonly AvaloniaFixture _avaloniaFixture;

    public NullImageFileTests(AvaloniaFixture avaloniaFixture)
    {
        _avaloniaFixture = avaloniaFixture;
    }
    
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

    [Fact]
    public async Task GetBitmap_ShouldReturnEmptyBitmap()
    {
        await _avaloniaFixture.Session.Dispatch(
            () =>
            {
                NullImageFile.Instance
                    .GetBitmap()
                    .Size
                    .Should()
                    .BeEquivalentTo(new Size(1, 1));
            },
            TestContext.Current.CancellationToken);
    }
}