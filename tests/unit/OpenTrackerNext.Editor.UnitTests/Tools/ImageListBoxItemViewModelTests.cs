using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using FluentAssertions;
using NSubstitute;
using OpenTrackerNext.Core.Images;
using OpenTrackerNext.Editor.Images;
using OpenTrackerNext.Editor.ViewModels.Tools;
using ReactiveUI;
using Splat;
using Xunit;

namespace OpenTrackerNext.Editor.UnitTests.Tools;

[ExcludeFromCodeCoverage]
public sealed class ImageListBoxItemViewModelTests
{
    private readonly CompositeDisposable _disposables = new();
    private readonly IImageService _imageService = Substitute.For<IImageService>();
    private readonly IImageFile _imageFile = Substitute.For<IImageFile>();

    private readonly ImageListBoxItemViewModel _subject;

    public ImageListBoxItemViewModelTests()
    {
        _subject = new ImageListBoxItemViewModel(_imageService, _imageFile);

        _subject.Activator
            .Activate()
            .DisposeWith(_disposables);
    }

    [Fact]
    public void ContextMenuItems_ShouldReturnExpected()
    {
        _subject.ContextMenuItems
            .Should()
            .HaveCount(3)
            .And.Contain(
                x =>
                    x.Icon == "mdi-rename" &&
                    x.Header == "Rename..." &&
                    x.Command == _subject.RenameCommand)
            .And.Contain(x => x.Header == "-")
            .And.Contain(
                x =>
                    x.Icon == "mdi-delete" &&
                    x.Header == "Delete..." &&
                    x.Command == _subject.DeleteCommand);
    }
    
    [Theory]
    [InlineData("Test")]
    [InlineData("Test2")]
    public void Text_ShouldReturnExpected(string expected)
    {
        _imageFile.FriendlyId.Returns(expected);

        _imageFile.PropertyChanged += Raise
            .Event<PropertyChangedEventHandler>(
                _imageFile,
                new PropertyChangedEventArgs(nameof(IImageFile.FriendlyId)));
        
        _subject.Text
            .Should()
            .Be(expected);
    }

    [Fact]
    public void DoubleTapCommand_ShouldDoNothing()
    {
        _subject.DoubleTapCommand
            .Execute()
            .Subscribe()
            .DisposeWith(_disposables);

        _imageService
            .DidNotReceive()
            .DeleteFileAsync(Arg.Any<IImageFile>());

        _imageService
            .DidNotReceive()
            .RenameFileAsync(Arg.Any<IImageFile>());

        _imageService
            .DidNotReceive()
            .AddFilesAsync();
    }
    
    [Fact]
    public void RenameCommand_ShouldCallRenameOnImageService()
    {
        _subject.RenameCommand
            .Execute()
            .Subscribe()
            .DisposeWith(_disposables);

        _imageService
            .Received(1)
            .RenameFileAsync(Arg.Is(_imageFile));
    }

    [Fact]
    public void DeleteCommand_ShouldCallDeleteOnImageService()
    {
        _subject.DeleteCommand
            .Execute()
            .Subscribe()
            .DisposeWith(_disposables);

        _imageService
            .Received(1)
            .DeleteFileAsync(Arg.Is(_imageFile));
    }
    
    [Fact]
    public void GetViewType_ShouldReturnExpected()
    {
        _subject.GetViewType()
            .Should()
            .Be(typeof(IViewFor<IListBoxItemViewModel>));
    }

    [Fact]
    public void SplatResolve_ShouldResolveToFactory()
    {
        var factory = Locator.Current.GetService<ImageListBoxItemViewModel.Factory>()!;

        var file = Substitute.For<IImageFile>();
        
        var subject1 = factory(file);
        var subject2 = factory(file);

        subject1.Should().NotBeSameAs(subject2);
    }
}