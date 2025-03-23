using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using OpenTrackerNext.Core.Json;
using OpenTrackerNext.Core.Metadata;
using OpenTrackerNext.Core.UnitTests.Documents;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Metadata;

[ExcludeFromCodeCoverage]
public sealed class PackMetadataReadOnlyDocumentFileTests : ReadOnlyDocumentFileTests<PackMetadata>
{
    [Fact]
    public async Task LoadDataFromFileAsync_ShouldSetSavedDataToFileContents()
    {
        const string name = "Test Name";
        const string author = "Test Author";
        
        var version = new Version(1, 2, 3, 4);
        
        var fileData = new PackMetadata
        {
            Name = name,
            Author = author,
            Version = version
        };
        
        var writeStream = File.OpenWrite();

        await JsonContext.SerializeAsync(writeStream, fileData);

        await writeStream.FlushAsync();
        await writeStream.DisposeAsync();
        
        await Subject.LoadDataFromFileAsync();
        
        Subject.Data
            .Name
            .Should()
            .Be(name);
        
        Subject.Data
            .Author
            .Should()
            .Be(author);
        
        Subject.Data
            .Version
            .Should()
            .Be(version);
    }
}