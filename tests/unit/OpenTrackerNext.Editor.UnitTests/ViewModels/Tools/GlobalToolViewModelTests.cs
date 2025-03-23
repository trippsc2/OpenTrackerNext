using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using FluentAssertions;
using FluentAssertions.Reactive;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Metadata;
using OpenTrackerNext.Core.Storage;
using OpenTrackerNext.Core.Storage.Memory;
using OpenTrackerNext.Editor.Documents;
using OpenTrackerNext.Editor.Metadata;
using OpenTrackerNext.Editor.ViewModels.Tools;
using Splat;
using Xunit;

namespace OpenTrackerNext.Editor.UnitTests.ViewModels.Tools;

[ExcludeFromCodeCoverage]
public sealed class GlobalToolViewModelTests : IAsyncLifetime
{
    private readonly CompositeDisposable _disposables = new();
    private readonly DocumentService _documentService = new();
    private readonly MetadataService _metadataService;
    private readonly MemoryStorageFolder _packFolder = new(Path.GetTempPath());
    
    private readonly GlobalToolViewModel _subject;

    public GlobalToolViewModelTests()
    {
        _metadataService = new MetadataService(_documentService, MetadataFileFactory);
        
        _subject = new GlobalToolViewModel(_documentService, _metadataService);

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
        await _metadataService.NewPackAsync(_packFolder);
    }
    
    public ValueTask DisposeAsync()
    { 
        _disposables.Dispose();
        return ValueTask.CompletedTask;
    }

    private static DocumentFile<PackMetadata> MetadataFileFactory(IStorageFile file)
    {
        return new DocumentFile<PackMetadata>(file);
    }

    [Fact]
    public void ListBoxItems_ShouldReturnExpected()
    {
        _subject.ListBoxItems
            .Should()
            .HaveCount(1)
            .And.Contain(x => x.Text == "Metadata");
    }

    [Fact]
    public void SelectedListBoxItem_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.SelectedListBoxItem = _subject.ListBoxItems[0];
        
        monitor.Should().RaisePropertyChangeFor(x => x.SelectedListBoxItem);
    }

    [Fact]
    public void OpenCommand_ShouldNotOpenDocument_WhenNothingIsSelected()
    {
        _subject.OpenCommand
            .Execute()
            .Subscribe()
            .DisposeWith(_disposables);

        _documentService
            .ActiveDocuments[DocumentSide.Left]
            .Value
            .Should()
            .BeNull();
    }
    
    [Fact]
    public void OpenCommand_ShouldOpenDocument_WhenMetadataIsSelected()
    {
        _subject.SelectedListBoxItem = _subject.ListBoxItems[0];
        
        using var observer = _documentService.ActivationRequests[DocumentSide.Left].Observe();
        
        _subject.OpenCommand
            .Execute()
            .Subscribe()
            .DisposeWith(_disposables);
        
        observer.Should()
            .PushMatch(
                x =>
                    x is Document<PackMetadata> &&
                    ((Document<PackMetadata>)x).File == _metadataService.File);
    }

    [Fact]
    public void SplatResolve_ShouldResolveToSingleInstance()
    {
        var subject1 = Locator.Current.GetService<GlobalToolViewModel>()!;
        var subject2 = Locator.Current.GetService<GlobalToolViewModel>()!;
        
        subject1.Should().BeSameAs(subject2);
    }
    
    [Fact]
    public void SplatResolve_ShouldResolveToLazySingleInstance()
    {
        var lazy = Locator.Current.GetService<Lazy<GlobalToolViewModel>>()!;
        var subject = Locator.Current.GetService<GlobalToolViewModel>()!;
        
        lazy.Value
            .Should()
            .BeSameAs(subject);
    }
}