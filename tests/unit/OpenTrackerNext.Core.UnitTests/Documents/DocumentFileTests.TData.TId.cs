using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Ids;
using OpenTrackerNext.Core.Json;
using OpenTrackerNext.Core.Storage.Memory;
using Splat;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Documents;

[ExcludeFromCodeCoverage]
public abstract class DocumentFileTests<TData, TId>
    where TData : class, ITitledDocumentData<TData>, new()
    where TId : IGuidId<TId>, new()
{
    protected readonly MemoryStorageFile File;
    protected readonly DocumentFile<TData, TId> Subject;
    private readonly string _titlePrefix;
    
    protected DocumentFileTests(string titlePrefix)
    {
        var name = Guid.NewGuid().ToString();
        File = new MemoryStorageFile(Path.Combine(Path.GetTempPath(), name));
        Subject = new DocumentFile<TData, TId>(File);
        _titlePrefix = titlePrefix;
    }

    [Fact]
    public void TitlePrefix_ShouldReturnSavedDataValue()
    {
        Subject.TitlePrefix
            .Should()
            .Be(_titlePrefix);
    }

    [Fact]
    public void FriendlyId_ShouldDefaultToEmptyString()
    {
        Subject.FriendlyId
            .Should()
            .BeEmpty();
    }

    [Fact]
    public async Task FriendlyId_ShouldRaisePropertyChanged()
    {
        using var monitor = Subject.Monitor();

        await Subject.RenameAsync("Test");

        monitor.Should().RaisePropertyChangeFor(x => x.FriendlyId);
    }
    
    [Fact]
    public void OpenedInDocuments_ShouldInitializeToZero()
    {
        Subject.OpenedInDocuments
            .Should()
            .Be(0);
    }
    
    [Fact]
    public void OpenedInDocuments_ShouldRaisePropertyChanged()
    {
        using var monitor = Subject.Monitor();
        
        Subject.OpenedInDocuments = 1;
        
        monitor.Should().RaisePropertyChangeFor(x => x.OpenedInDocuments);
    }

    [Fact]
    public void Id_ShouldReturnExpected()
    {
        Subject.Id
            .ToString()?
            .ToLowerInvariant()
            .Should()
            .Be(File.Name);
    }
    
    [Fact]
    public async Task RenameAsync_ShouldChangeFriendlyId_ShouldSaveDataToFile()
    {
        const string friendlyId = "Test";

        await Subject.RenameAsync(friendlyId);

        Subject.FriendlyId
            .Should()
            .Be(friendlyId);

        await using var stream = File.OpenRead();
        var namedJsonData = await JsonContext.DeserializeAsync<NamedData<TData>>(stream);

        namedJsonData.Should().NotBeNull();
        
        namedJsonData.Name
            .Should()
            .Be(friendlyId);
    }
    
    [Fact]
    public void Delete_ShouldDeleteFile()
    {
        Subject.Delete();

        File.Exists()
            .Should()
            .BeFalse();
    }

    [Fact]
    public void SplatResolve_ShouldResolveToFactory()
    {
        var factory = Locator.Current.GetService<IDocumentFile<TData, TId>.Factory>()!;
        
        var subject1 = factory(File);
        var subject2 = factory(File);
        
        subject1.Should().BeOfType<DocumentFile<TData, TId>>();
        subject2.Should().BeOfType<DocumentFile<TData, TId>>();
        
        subject1.Should().NotBeSameAs(subject2);
    }
}