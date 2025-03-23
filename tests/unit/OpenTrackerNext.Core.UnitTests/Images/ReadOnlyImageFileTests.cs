using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using FluentAssertions;
using NSubstitute;
using OpenTrackerNext.Core.Ids;
using OpenTrackerNext.Core.Images;
using OpenTrackerNext.Core.Storage;
using Splat;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Images;

[ExcludeFromCodeCoverage]
public sealed class ReadOnlyImageFileTests
{
    private readonly AvaloniaFixture _avaloniaFixture;
    private readonly string _fileName = Guid.NewGuid().ToLowercaseString();
    private readonly IStorageFile _file;

    private readonly ReadOnlyImageFile _subject;

    public ReadOnlyImageFileTests(AvaloniaFixture avaloniaFixture)
    {
        _avaloniaFixture = avaloniaFixture;
        _file = Substitute.For<IStorageFile>();
        _file.Name.Returns(_fileName);
        
        _subject = new ImageFile(_file);
    }
    
    [Fact]
    public void Id_ShouldReturnExpected()
    {
        _subject.Id
            .Should()
            .Be(ImageId.Parse(_fileName));
    }

    [Fact]
    public async Task GetBitmap_ShouldReturnExpectedBitmap()
    {
        var stream = new MemoryStream();
        
        _file.OpenRead().Returns(stream);
        
        await _avaloniaFixture.Session.Dispatch(
            () =>
            {
                var bitmap = _subject.GetBitmap();
        
                bitmap.Size
                    .Should()
                    .BeEquivalentTo(new Size(1, 1));
            },
            TestContext.Current.CancellationToken);
    }

    [Fact]
    public void SplatResolve_ShouldResolveInterfaceFactory()
    {
        var factory = Locator.Current.GetService<IReadOnlyImageFile.Factory>()!;

        var subject1 = factory(_file);
        var subject2 = factory(_file);

        subject1.Should().BeOfType<ReadOnlyImageFile>();
        subject2.Should().BeOfType<ReadOnlyImageFile>();
        
        subject1.Should().NotBeSameAs(subject2);
    }

}