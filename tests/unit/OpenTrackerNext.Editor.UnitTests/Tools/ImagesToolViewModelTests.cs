using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using FluentAssertions;
using NSubstitute;
using OpenTrackerNext.Core.Images;
using OpenTrackerNext.Core.Results;
using OpenTrackerNext.Core.Storage;
using OpenTrackerNext.Core.Storage.Memory;
using OpenTrackerNext.Editor.Images;
using OpenTrackerNext.Editor.ViewModels.Tools;
using ReactiveUI;
using Splat;
using Xunit;
using IAvaloniaStorageFile = Avalonia.Platform.Storage.IStorageFile;

namespace OpenTrackerNext.Editor.UnitTests.Tools;

[ExcludeFromCodeCoverage]
public sealed class ImagesToolViewModelTests : IAsyncLifetime
{
    private readonly CompositeDisposable _disposables = new();
    private readonly ImageService _imageService = new(ImageFileFactory);
    private readonly MemoryStorageFolder _packFolder = new(Path.GetTempPath());
    
    private readonly ImagesToolViewModel _subject;

    public ImagesToolViewModelTests()
    {
        _subject = new ImagesToolViewModel(_imageService, ListBoxItemFactory);
        
        _subject.Activator
            .Activate()
            .DisposeWith(_disposables);

        _subject.ListBoxItems
            .ToObservableChangeSet()
            .Transform(x => x.Activator.Activate())
            .DisposeMany()
            .Subscribe()
            .DisposeWith(_disposables);
    }
    
    public async ValueTask InitializeAsync()
    {
        await _imageService.NewPackAsync(_packFolder);
    }

    public ValueTask DisposeAsync()
    { 
        _disposables.Dispose();
        return ValueTask.CompletedTask;
    }

    private static ImageFile ImageFileFactory(IStorageFile file)
    {
        return new ImageFile(file);
    }

    private ImageListBoxItemViewModel ListBoxItemFactory(IImageFile imageFile)
    {
        return new ImageListBoxItemViewModel(_imageService, imageFile);
    }
    
    private async Task<IImageFile> CreateImageFileAsync()
    {
        var disposable = _imageService
            .AddImagesDialog
            .RegisterHandler(
                context =>
                {
                    var file = Substitute.For<IAvaloniaStorageFile>();
                    file.Name.Returns(Guid.NewGuid().ToString());
                    file.OpenReadAsync().Returns(new MemoryStream());
                    
                    context.SetOutput(new List<IAvaloniaStorageFile> { file });
                });
        
        var files = await _imageService.AddFilesAsync();
        
        disposable.Dispose();

        return files[0];
    }

    [Fact]
    public async Task ListBoxItems_ShouldReturnExpected()
    {
        var imageFile1 = await CreateImageFileAsync();
        var imageFile2 = await CreateImageFileAsync();
        
        _subject.ListBoxItems
            .Should()
            .HaveCount(2)
            .And.Contain(x => x.Text == imageFile1.FriendlyId)
            .And.Contain(x => x.Text == imageFile2.FriendlyId);
    }
    
    [Fact]
    public async Task SelectedListBoxItem_ShouldRaisePropertyChanged()
    {
        await CreateImageFileAsync();

        using var monitor = _subject.Monitor();
        
        _subject.SelectedListBoxItem = _subject.ListBoxItems[0];
        
        monitor.Should().RaisePropertyChangeFor(x => x.SelectedListBoxItem);
    }

    [Fact]
    public void AddCommand_ShouldCallAddImage()
    {
        var addImagesHandled = false;
        
        _imageService
            .AddImagesDialog
            .RegisterHandler(
                context =>
                {
                    addImagesHandled = true;
                    context.SetOutput(new List<IAvaloniaStorageFile>());
                })
            .DisposeWith(_disposables);
        
        _subject.AddCommand
            .Execute()
            .Subscribe()
            .DisposeWith(_disposables);
        
        addImagesHandled.Should().BeTrue();
    }

    [Fact]
    public void RenameCommand_ShouldDoNothing_WhenSelectedListBoxItemIsNull()
    {
        var renameHandled = false;
        _imageService
            .RenameImageDialog
            .RegisterHandler(
                context =>
                {
                    renameHandled = true;
                    context.SetOutput(OperationResult.Success);
                })
            .DisposeWith(_disposables);
        
        _subject.RenameCommand
            .Execute()
            .Subscribe()
            .DisposeWith(_disposables);
        
        renameHandled.Should().BeFalse();
    }
    
    [Fact]
    public async Task RenameCommand_ShouldCallRenameImage()
    {
        await CreateImageFileAsync();
        
        _subject.SelectedListBoxItem = _subject.ListBoxItems[0];
        
        var renameHandled = false;
        
        _imageService
            .RenameImageDialog
            .RegisterHandler(
                context =>
                {
                    renameHandled = true;
                    context.SetOutput(OperationResult.Success);
                })
            .DisposeWith(_disposables);
        
        _subject.RenameCommand
            .Execute()
            .Subscribe()
            .DisposeWith(_disposables);
        
        renameHandled.Should().BeTrue();
    }
    
    [Fact]
    public void DeleteCommand_ShouldDoNothing_WhenSelectedListBoxItemIsNull()
    {
        var deleteHandled = false;
        
        _imageService
            .DeleteImageDialog
            .RegisterHandler(
                context =>
                {
                    deleteHandled = true;
                    context.SetOutput(YesNoDialogResult.No);
                })
            .DisposeWith(_disposables);
        
        _subject.DeleteCommand
            .Execute()
            .Subscribe()
            .DisposeWith(_disposables);
        
        deleteHandled.Should().BeFalse();
    }
    
    [Fact]
    public async Task DeleteCommand_ShouldCallDeleteImage()
    {
        await CreateImageFileAsync();
        
        _subject.SelectedListBoxItem = _subject.ListBoxItems[0];
        
        var deleteHandled = false;
        
        _imageService
            .DeleteImageDialog
            .RegisterHandler(
                context =>
                {
                    deleteHandled = true;
                    context.SetOutput(YesNoDialogResult.Yes);
                })
            .DisposeWith(_disposables);
        
        _subject.DeleteCommand
            .Execute()
            .Subscribe()
            .DisposeWith(_disposables);
        
        deleteHandled.Should().BeTrue();
    }

    [Fact]
    public void GetViewType_ShouldReturnExpected()
    {
        _subject.GetViewType()
            .Should()
            .Be(typeof(IViewFor<ImagesToolViewModel>));
    }
    
    [Fact]
    public void SplatResolve_ShouldResolveToSingleInstance()
    {
        var subject1 = Locator.Current.GetService<ImagesToolViewModel>()!;
        var subject2 = Locator.Current.GetService<ImagesToolViewModel>()!;

        subject1.Should().BeSameAs(subject2);
    }
    
    [Fact]
    public void SplatResolve_ShouldResolveToLazySingleInstance()
    {
        var lazy = Locator.Current.GetService<Lazy<ImagesToolViewModel>>()!;
        var subject = Locator.Current.GetService<ImagesToolViewModel>()!;
        
        lazy.Value
            .Should()
            .BeSameAs(subject);
    }
}