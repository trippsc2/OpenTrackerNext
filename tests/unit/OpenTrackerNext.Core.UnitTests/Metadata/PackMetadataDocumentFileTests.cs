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
public sealed class PackMetadataDocumentFileTests() : DocumentFileTests<PackMetadata>(PackMetadata.MetadataTitle)
{
    [Fact]
    public void SavedData_ShouldInitializeAsDefault()
    {
        Subject.SavedData
            .Name
            .Should()
            .BeEmpty();
        
        Subject.SavedData
            .Author
            .Should()
            .BeEmpty();
        
        Subject.SavedData
            .Version
            .Should()
            .Be(new Version(1, 0, 0, 0));
    }

    [Fact]
    public void WorkingData_ShouldInitializeAsDefault()
    {
        Subject.WorkingData
            .Name
            .Should()
            .BeEmpty();
        
        Subject.WorkingData
            .Author
            .Should()
            .BeEmpty();
        
        Subject.WorkingData
            .Version
            .Should()
            .Be(new Version(1, 0, 0, 0));
    }
    
    [Fact]
    public void OpenedInDocuments_ShouldRevertWorkingData_WhenChangedFrom0ToPositiveValue()
    {
        const string expectedName = "Test Name";
        const string expectedAuthor = "Test Author";
        var expectedVersion = new Version(1, 2, 3, 4);
        
        Subject.SavedData.Name = expectedName;
        Subject.SavedData.Author = expectedAuthor;
        Subject.SavedData.Version = expectedVersion;
        
        Subject.WorkingData.Name = "Test Name 2";
        Subject.WorkingData.Author = "Test Author 2";
        Subject.WorkingData.Version = new Version(5, 6, 7, 8);
        
        Subject.OpenedInDocuments = 1;

        Subject.SavedData
            .Name
            .Should()
            .Be(expectedName);
        
        Subject.SavedData
            .Author
            .Should()
            .Be(expectedAuthor);
        
        Subject.SavedData
            .Version
            .Should()
            .Be(expectedVersion);

        Subject.WorkingData
            .Name
            .Should()
            .Be(expectedName);
        
        Subject.WorkingData
            .Author
            .Should()
            .Be(expectedAuthor);
        
        Subject.WorkingData
            .Version
            .Should()
            .Be(expectedVersion);
    }
    
    [Fact]
    public void OpenedInDocuments_ShouldNotRevertWorkingData_WhenChangedFromPositiveValueTo0()
    {
        const string savedName = "Test Name";
        const string savedAuthor = "Test Author";
        const string workingName = "Test Name 2";
        const string workingAuthor = "Test Author 2";
        
        var savedVersion = new Version(1, 2, 3, 4);
        var workingVersion = new Version(5, 6, 7, 8);

        Subject.OpenedInDocuments = 1;
        
        Subject.SavedData.Name = savedName;
        Subject.SavedData.Author = savedAuthor;
        Subject.SavedData.Version = savedVersion;
        
        Subject.WorkingData.Name = workingName;
        Subject.WorkingData.Author = workingAuthor;
        Subject.WorkingData.Version = workingVersion;
        
        Subject.OpenedInDocuments = 0;

        Subject.SavedData
            .Name
            .Should()
            .Be(savedName);
        
        Subject.SavedData
            .Author
            .Should()
            .Be(savedAuthor);
        
        Subject.SavedData
            .Version
            .Should()
            .Be(savedVersion);
        
        Subject.WorkingData
            .Name
            .Should()
            .Be(workingName);
        
        Subject.WorkingData
            .Author
            .Should()
            .Be(workingAuthor);
        
        Subject.WorkingData
            .Version
            .Should()
            .Be(workingVersion);
    }

    [Fact]
    public void OpenedInDocuments_ShouldNotRevertWorkingData_WhenChangedFromPositiveValueToPositiveValue()
    {
        const string savedName = "Test Name";
        const string savedAuthor = "Test Author";
        const string workingName = "Test Name 2";
        const string workingAuthor = "Test Author 2";
        
        var savedVersion = new Version(1, 2, 3, 4);
        var workingVersion = new Version(5, 6, 7, 8);

        Subject.OpenedInDocuments = 1;
        
        Subject.SavedData.Name = savedName;
        Subject.SavedData.Author = savedAuthor;
        Subject.SavedData.Version = savedVersion;
        
        Subject.WorkingData.Name = workingName;
        Subject.WorkingData.Author = workingAuthor;
        Subject.WorkingData.Version = workingVersion;

        Subject.OpenedInDocuments = 2;

        Subject.SavedData
            .Name
            .Should()
            .Be(savedName);
        
        Subject.SavedData
            .Author
            .Should()
            .Be(savedAuthor);
        
        Subject.SavedData
            .Version
            .Should()
            .Be(savedVersion);
        
        Subject.WorkingData
            .Name
            .Should()
            .Be(workingName);
        
        Subject.WorkingData
            .Author
            .Should()
            .Be(workingAuthor);
        
        Subject.WorkingData
            .Version
            .Should()
            .Be(workingVersion);
    }

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
        
        Subject.SavedData
            .Name
            .Should()
            .Be(name);
        
        Subject.SavedData
            .Author
            .Should()
            .Be(author);
        
        Subject.SavedData
            .Version
            .Should()
            .Be(version);
        
        Subject.WorkingData
            .Name
            .Should()
            .Be(name);
        
        Subject.WorkingData
            .Author
            .Should()
            .Be(author);
        
        Subject.WorkingData
            .Version
            .Should()
            .Be(version);
    }

    [Fact]
    public async Task LoadDataFromFileAsync_ShouldNotChangeValues_WhenFileDoesNotContainValidData()
    {
        const string savedName = "Test Name";
        const string savedAuthor = "Test Author";
        const string workingName = "Test Name 2";
        const string workingAuthor = "Test Author 2";
        
        var savedVersion = new Version(1, 2, 3, 4);
        var workingVersion = new Version(5, 6, 7, 8);
        
        Subject.SavedData.Name = savedName;
        Subject.SavedData.Author = savedAuthor;
        Subject.SavedData.Version = savedVersion;
        
        Subject.WorkingData.Name = workingName;
        Subject.WorkingData.Author = workingAuthor;
        Subject.WorkingData.Version = workingVersion;

        await Subject.LoadDataFromFileAsync();
        
        Subject.SavedData
            .Name
            .Should()
            .Be(savedName);
        
        Subject.SavedData
            .Author
            .Should()
            .Be(savedAuthor);
        
        Subject.SavedData
            .Version
            .Should()
            .Be(savedVersion);
        
        Subject.WorkingData
            .Name
            .Should()
            .Be(workingName);
        
        Subject.WorkingData
            .Author
            .Should()
            .Be(workingAuthor);
        
        Subject.WorkingData
            .Version
            .Should()
            .Be(workingVersion);
    }
    
    [Fact]
    public void Revert_ShouldMakeWorkingDataEqualToSavedData()
    {
        const string expectedName = "Test Name";
        const string expectedAuthor = "Test Author";
        
        var expectedVersion = new Version(1, 2, 3, 4);
        
        Subject.SavedData.Name = expectedName;
        Subject.SavedData.Author = expectedAuthor;
        Subject.SavedData.Version = expectedVersion;
        
        Subject.WorkingData.Name = "Test Name 2";
        Subject.WorkingData.Author = "Test Author 2";
        Subject.WorkingData.Version = new Version(5, 6, 7, 8);

        Subject.Revert();

        Subject.WorkingData
            .Name
            .Should()
            .Be(expectedName);
        
        Subject.WorkingData
            .Author
            .Should()
            .Be(expectedAuthor);
        
        Subject.WorkingData
            .Version
            .Should()
            .Be(expectedVersion);
    }
    
    [Fact]
    public async Task SaveAsync_ShouldSaveWorkingDataToFile()
    {
        const string expectedName = "Test Name";
        const string expectedAuthor = "Test Author";
        
        var expectedVersion = new Version(1, 2, 3, 4);
        
        Subject.WorkingData.Name = expectedName;
        Subject.WorkingData.Author = expectedAuthor;
        Subject.WorkingData.Version = expectedVersion;

        await Subject.SaveAsync();

        await using var stream = File.OpenRead();
        var data = await JsonContext.DeserializeAsync<PackMetadata>(stream);
        
        data.Should().NotBeNull();
        
        data.Name
            .Should()
            .Be(expectedName);
        
        data.Author
            .Should()
            .Be(expectedAuthor);
        
        data.Version
            .Should()
            .Be(expectedVersion);

        Subject.SavedData
            .Name
            .Should()
            .Be(expectedName);
        
        Subject.SavedData
            .Author
            .Should()
            .Be(expectedAuthor);
        
        Subject.SavedData
            .Version
            .Should()
            .Be(expectedVersion);
    }
}
