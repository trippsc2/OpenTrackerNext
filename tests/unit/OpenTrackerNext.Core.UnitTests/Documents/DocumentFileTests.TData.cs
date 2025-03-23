using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using FluentAssertions;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Storage.Memory;
using Splat;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Documents;

[ExcludeFromCodeCoverage]
public abstract class DocumentFileTests<TData>
    where TData : class, ITitledDocumentData<TData>, new()
{
    protected readonly MemoryStorageFile File;
    protected readonly DocumentFile<TData> Subject;

    private readonly string _titlePrefix;

    protected DocumentFileTests(string titlePrefix)
    {
        var name = Guid.NewGuid().ToString();
        File = new MemoryStorageFile(Path.Combine(Path.GetTempPath(), name));
        Subject = new DocumentFile<TData>(File);
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
    public void FriendlyId_ShouldReturnEmptyString()
    {
        Subject.FriendlyId
            .Should()
            .BeEmpty();
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
        var factory = Locator.Current.GetService<IDocumentFile<TData>.Factory>()!;
        
        var subject1 = factory(File);
        var subject2 = factory(File);

        subject1.Should().BeOfType<DocumentFile<TData>>();
        subject2.Should().BeOfType<DocumentFile<TData>>();
        
        subject1.Should().NotBeSameAs(subject2);
    }
}