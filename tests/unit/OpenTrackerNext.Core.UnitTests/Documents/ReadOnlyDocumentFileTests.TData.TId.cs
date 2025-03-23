using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Ids;
using OpenTrackerNext.Core.Storage.Memory;
using Splat;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Documents;

[ExcludeFromCodeCoverage]
public abstract class ReadOnlyDocumentFileTests<TData, TId>
    where TData : class, ITitledDocumentData<TData>, new()
    where TId : struct, IGuidId<TId>
{
    protected readonly MemoryStorageFile File;
    protected readonly ReadOnlyDocumentFile<TData, TId> Subject;
    
    protected ReadOnlyDocumentFileTests()
    {
        var name = Guid.NewGuid().ToString();
        File = new MemoryStorageFile(Path.Combine(Path.GetTempPath(), name));
        Subject = new ReadOnlyDocumentFile<TData, TId>(File);
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
    public void Data_ShouldDefaultToNull()
    {
        Subject.Data
            .Should()
            .BeNull();
    }
        
    [Fact]
    public async Task LoadDataFromFileAsync_ShouldNotChangeData_WhenFileDoesNotContainValidData()
    {
        await Subject.LoadDataFromFileAsync();

        Subject.Data
            .Should()
            .BeNull();
    }

    [Fact]
    public void SplatResolve_ShouldResolveToFactory()
    {
        var factory = Locator.Current.GetService<IReadOnlyDocumentFile<TData, TId>.Factory>()!;
        
        var subject1 = factory(File);
        var subject2 = factory(File);
        
        subject1.Should().BeOfType<ReadOnlyDocumentFile<TData, TId>>();
        subject2.Should().BeOfType<ReadOnlyDocumentFile<TData, TId>>();
        
        subject1.Should().NotBeSameAs(subject2);
    }
}