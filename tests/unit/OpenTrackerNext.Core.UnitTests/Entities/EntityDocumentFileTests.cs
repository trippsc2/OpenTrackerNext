using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Entities;
using OpenTrackerNext.Core.Json;
using OpenTrackerNext.Core.UnitTests.Documents;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Entities;

[ExcludeFromCodeCoverage]
public sealed class EntityDocumentFileTests()
    : DocumentFileTests<EntityPrototype, EntityId>(EntityPrototype.EntityTitlePrefix)
{
    [Fact]
    public void SavedData_ShouldInitializeAsDefault()
    {
        Subject.SavedData
            .Minimum
            .Should()
            .Be(0);
        
        Subject.SavedData
            .Starting
            .Should()
            .Be(0);
        
        Subject.SavedData
            .Maximum
            .Should()
            .Be(1);
    }
    
    [Fact]
    public void WorkingData_ShouldInitializeAsDefault()
    {
        Subject.WorkingData
            .Minimum
            .Should()
            .Be(0);
        
        Subject.WorkingData
            .Starting
            .Should()
            .Be(0);
        
        Subject.WorkingData
            .Maximum
            .Should()
            .Be(1);
    }
    
    [Fact]
    public void OpenedInDocuments_ShouldRevertWorkingData_WhenChangedFrom0ToPositiveValue()
    {
        const int expectedMinimum = 0;
        const int expectedStarting = 1;
        const int expectedMaximum = 2;
        
        Subject.SavedData.Minimum = expectedMinimum;
        Subject.SavedData.Starting = expectedStarting;
        Subject.SavedData.Maximum = expectedMaximum;
        
        Subject.WorkingData.Minimum = 3;
        Subject.WorkingData.Starting = 4;
        Subject.WorkingData.Maximum = 5;
        
        Subject.OpenedInDocuments = 1;

        Subject.SavedData
            .Minimum
            .Should()
            .Be(expectedMinimum);
        
        Subject.SavedData
            .Starting
            .Should()
            .Be(expectedStarting);
        
        Subject.SavedData
            .Maximum
            .Should()
            .Be(expectedMaximum);

        Subject.WorkingData
            .Minimum
            .Should()
            .Be(expectedMinimum);
        
        Subject.WorkingData
            .Starting
            .Should()
            .Be(expectedStarting);
        
        Subject.WorkingData
            .Maximum
            .Should()
            .Be(expectedMaximum);
    }
    
    [Fact]
    public void OpenedInDocuments_ShouldNotRevertWorkingData_WhenChangedFromPositiveValueTo0()
    {
        const int savedMinimum = 0;
        const int savedStarting = 1;
        const int savedMaximum = 2;
        const int workingMinimum = 3;
        const int workingStarting = 4;
        const int workingMaximum = 5;

        Subject.OpenedInDocuments = 1;
        
        Subject.SavedData.Minimum = savedMinimum;
        Subject.SavedData.Starting = savedStarting;
        Subject.SavedData.Maximum = savedMaximum;
        
        Subject.WorkingData.Minimum = workingMinimum;
        Subject.WorkingData.Starting = workingStarting;
        Subject.WorkingData.Maximum = workingMaximum;
        
        Subject.OpenedInDocuments = 0;

        Subject.SavedData
            .Minimum
            .Should()
            .Be(savedMinimum);
        
        Subject.SavedData
            .Starting
            .Should()
            .Be(savedStarting);
        
        Subject.SavedData
            .Maximum
            .Should()
            .Be(savedMaximum);
        
        Subject.WorkingData
            .Minimum
            .Should()
            .Be(workingMinimum);
        
        Subject.WorkingData
            .Starting
            .Should()
            .Be(workingStarting);
        
        Subject.WorkingData
            .Maximum
            .Should()
            .Be(workingMaximum);
    }

    [Fact]
    public void OpenedInDocuments_ShouldNotRevertWorkingData_WhenChangedFromPositiveValueToPositiveValue()
    {
        const int savedMinimum = 0;
        const int savedStarting = 1;
        const int savedMaximum = 2;
        const int workingMinimum = 3;
        const int workingStarting = 4;
        const int workingMaximum = 5;

        Subject.OpenedInDocuments = 1;
        
        Subject.SavedData.Minimum = savedMinimum;
        Subject.SavedData.Starting = savedStarting;
        Subject.SavedData.Maximum = savedMaximum;
        
        Subject.WorkingData.Minimum = workingMinimum;
        Subject.WorkingData.Starting = workingStarting;
        Subject.WorkingData.Maximum = workingMaximum;

        Subject.OpenedInDocuments = 2;

        Subject.SavedData
            .Minimum
            .Should()
            .Be(savedMinimum);
        
        Subject.SavedData
            .Starting
            .Should()
            .Be(savedStarting);
        
        Subject.SavedData
            .Maximum
            .Should()
            .Be(savedMaximum);
        
        Subject.WorkingData
            .Minimum
            .Should()
            .Be(workingMinimum);
        
        Subject.WorkingData
            .Starting
            .Should()
            .Be(workingStarting);
        
        Subject.WorkingData
            .Maximum
            .Should()
            .Be(workingMaximum);
    }

    [Fact]
    public async Task LoadDataFromFileAsync_ShouldSetSavedDataToFileContents()
    {
        const string friendlyId = "Test ID";
        const int minimum = 1;
        const int starting = 2;
        const int maximum = 3;

        var fileData = new NamedData<EntityPrototype>
        {
            Name = friendlyId,
            Data = new EntityPrototype
            {
                Minimum = minimum,
                Starting = starting,
                Maximum = maximum
            }
        };
        
        var writeStream = File.OpenWrite();
        
        await JsonContext.SerializeAsync(writeStream, fileData);

        await writeStream.FlushAsync();
        await writeStream.DisposeAsync();
        
        await Subject.LoadDataFromFileAsync();

        Subject.FriendlyId
            .Should()
            .Be(friendlyId);
        
        Subject.SavedData
            .Minimum
            .Should()
            .Be(minimum);
        
        Subject.SavedData
            .Starting
            .Should()
            .Be(starting);
        
        Subject.SavedData
            .Maximum
            .Should()
            .Be(maximum);

        Subject.WorkingData
            .Minimum
            .Should()
            .Be(minimum);
        
        Subject.WorkingData
            .Starting
            .Should()
            .Be(starting);
        
        Subject.WorkingData
            .Maximum
            .Should()
            .Be(maximum);
    }
    
    [Fact]
    public async Task LoadDataFromFileAsync_ShouldNotChangeValues_WhenFileDoesNotContainValidData()
    {
        const int savedMinimum = 1;
        const int savedStarting = 2;
        const int savedMaximum = 3;
        const int workingMinimum = 4;
        const int workingStarting = 5;
        const int workingMaximum = 6;
        
        Subject.SavedData.Minimum = savedMinimum;
        Subject.SavedData.Starting = savedStarting;
        Subject.SavedData.Maximum = savedMaximum;
        
        Subject.WorkingData.Minimum = workingMinimum;
        Subject.WorkingData.Starting = workingStarting;
        Subject.WorkingData.Maximum = workingMaximum;
        
        await Subject.LoadDataFromFileAsync();
        
        Subject.SavedData
            .Minimum
            .Should()
            .Be(savedMinimum);
        
        Subject.SavedData
            .Starting
            .Should()
            .Be(savedStarting);
        
        Subject.SavedData
            .Maximum
            .Should()
            .Be(savedMaximum);
        
        Subject.WorkingData
            .Minimum
            .Should()
            .Be(workingMinimum);
        
        Subject.WorkingData
            .Starting
            .Should()
            .Be(workingStarting);
        
        Subject.WorkingData
            .Maximum
            .Should()
            .Be(workingMaximum);
    }
    
    [Fact]
    public void Revert_ShouldMakeWorkingDataEqualToSavedData()
    {
        const int expectedMinimum = 1;
        const int expectedStarting = 2;
        const int expectedMaximum = 3;
        
        Subject.SavedData.Minimum = expectedMinimum;
        Subject.SavedData.Starting = expectedStarting;
        Subject.SavedData.Maximum = expectedMaximum;
        
        Subject.WorkingData.Minimum = 4;
        Subject.WorkingData.Starting = 5;
        Subject.WorkingData.Maximum = 6;

        Subject.Revert();

        Subject.WorkingData
            .Minimum
            .Should()
            .Be(expectedMinimum);
        
        Subject.WorkingData
            .Starting
            .Should()
            .Be(expectedStarting);
        
        Subject.WorkingData
            .Maximum
            .Should()
            .Be(expectedMaximum);
    }
    
    [Fact]
    public async Task SaveAsync_ShouldSaveWorkingDataToFile()
    {
        const string expectedFriendlyId = "Test ID";
        const int expectedMinimum = 1;
        const int expectedStarting = 2;
        const int expectedMaximum = 3;
        
        Subject.WorkingData.Minimum = expectedMinimum;
        Subject.WorkingData.Starting = expectedStarting;
        Subject.WorkingData.Maximum = expectedMaximum;

        await Subject.RenameAsync(expectedFriendlyId);
        await Subject.SaveAsync();

        await using var stream = File.OpenRead();
        var namedJsonData = await JsonContext.DeserializeAsync<NamedData<EntityPrototype>>(stream);

        namedJsonData.Should().NotBeNull();
        
        namedJsonData.Name
            .Should()
            .Be(expectedFriendlyId);
        
        namedJsonData.Data
            .Minimum
            .Should()
            .Be(expectedMinimum);
        
        namedJsonData.Data
            .Starting
            .Should()
            .Be(expectedStarting);
        
        namedJsonData.Data
            .Maximum
            .Should()
            .Be(expectedMaximum);

        Subject.FriendlyId
            .Should()
            .Be(expectedFriendlyId);
        
        Subject.SavedData
            .Minimum
            .Should()
            .Be(expectedMinimum);
        
        Subject.SavedData
            .Starting
            .Should()
            .Be(expectedStarting);
        
        Subject.SavedData
            .Maximum
            .Should()
            .Be(expectedMaximum);

        Subject.WorkingData
            .Minimum
            .Should()
            .Be(expectedMinimum);
        
        Subject.WorkingData
            .Starting
            .Should()
            .Be(expectedStarting);
        
        Subject.WorkingData
            .Maximum
            .Should()
            .Be(expectedMaximum);
    }
}