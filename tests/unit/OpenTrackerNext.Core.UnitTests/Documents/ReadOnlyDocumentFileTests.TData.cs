using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Storage.Memory;
using Splat;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Documents;

[ExcludeFromCodeCoverage]
public abstract class ReadOnlyDocumentFileTests<TData>
    where TData : class, ITitledDocumentData<TData>, new()
{
    protected readonly MemoryStorageFile File;
    protected readonly ReadOnlyDocumentFile<TData> Subject;

    protected ReadOnlyDocumentFileTests()
    {
        var name = Guid.NewGuid().ToString();
        File = new MemoryStorageFile(Path.Combine(Path.GetTempPath(), name));
        Subject = new ReadOnlyDocumentFile<TData>(File);
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
        var factory = Locator.Current.GetService<IReadOnlyDocumentFile<TData>.Factory>()!;
        
        var subject1 = factory(File);
        var subject2 = factory(File);

        subject1.Should().BeOfType<ReadOnlyDocumentFile<TData>>();
        subject2.Should().BeOfType<ReadOnlyDocumentFile<TData>>();
        
        subject1.Should().NotBeSameAs(subject2);
    }
}