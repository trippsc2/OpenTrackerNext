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
public sealed class ImageFileTests
{
    private readonly AvaloniaFixture _avaloniaFixture;
    private readonly string _fileName = Guid.NewGuid().ToLowercaseString();
    private readonly IStorageFile _file;

    private readonly ImageFile _subject;

    public ImageFileTests(AvaloniaFixture avaloniaFixture)
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
    public void FriendlyId_ShouldDefaultToEmpty()
    {
        _subject.FriendlyId
            .Should()
            .BeEmpty();
    }
    
    [Fact]
    public void FriendlyId_ShouldReturnSetValue()
    {
        const string value = "Test";
        
        _subject.FriendlyId = value;
        
        _subject.FriendlyId
            .Should()
            .Be(value);
    }

    [Fact]
    public void FriendlyId_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.FriendlyId = "Test";
        
        monitor.Should().RaisePropertyChangeFor(i => i.FriendlyId);
    }
    
    [Fact]
    public void Delete_ShouldCallDeleteOnFile()
    {
        _subject.Delete();
        
        _file.Received(1).Delete();
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
        var factory = Locator.Current.GetService<IImageFile.Factory>()!;

        var subject1 = factory(_file);
        var subject2 = factory(_file);

        subject1.Should().BeOfType<ImageFile>();
        subject2.Should().BeOfType<ImageFile>();
        
        subject1.Should().NotBeSameAs(subject2);
    }
}