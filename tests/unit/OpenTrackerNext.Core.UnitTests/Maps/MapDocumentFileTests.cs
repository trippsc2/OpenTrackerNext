using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Images;
using OpenTrackerNext.Core.Json;
using OpenTrackerNext.Core.Maps;
using OpenTrackerNext.Core.Maps.Dynamic;
using OpenTrackerNext.Core.Maps.Static;
using OpenTrackerNext.Core.UnitTests.Documents;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Maps;

[ExcludeFromCodeCoverage]
public sealed class MapDocumentFileTests() : DocumentFileTests<MapPrototype, MapId>(MapPrototype.TitlePrefix)
{
    [Fact]
    public void SavedData_ShouldInitializeAsDefault()
    {
        Subject.SavedData
            .Content
            .Should()
            .BeOfType<StaticMapPrototype>();
    }
    
    [Fact]
    public void WorkingData_ShouldInitializeAsDefault()
    {
        Subject.WorkingData
            .Content
            .Should()
            .BeOfType<StaticMapPrototype>();
    }
    
    [Fact]
    public void OpenedInDocuments_ShouldRevertWorkingData_WhenChangedFrom0ToPositiveValue()
    {
        var expectedMetImage = ImageId.New();
        var expectedUnmetImage = ImageId.New();

        Subject.SavedData.Content = new DynamicMapPrototype
        {
            MetImage = expectedMetImage,
            UnmetImage = expectedUnmetImage
        };

        Subject.WorkingData.Content = new StaticMapPrototype();
        
        Subject.OpenedInDocuments = 1;

        Subject.SavedData
            .Content
            .Should()
            .BeOfType<DynamicMapPrototype>()
            .Subject
            .MetImage
            .Should()
            .Be(expectedMetImage);
        
        Subject.SavedData
            .Content
            .Should()
            .BeOfType<DynamicMapPrototype>()
            .Subject
            .UnmetImage
            .Should()
            .Be(expectedUnmetImage);
        
        Subject.WorkingData
            .Content
            .Should()
            .BeOfType<DynamicMapPrototype>()
            .Subject
            .MetImage
            .Should()
            .Be(expectedMetImage);
        
        Subject.WorkingData
            .Content
            .Should()
            .BeOfType<DynamicMapPrototype>()
            .Subject
            .UnmetImage
            .Should()
            .Be(expectedUnmetImage);
    }
    
    [Fact]
    public void OpenedInDocuments_ShouldNotRevertWorkingData_WhenChangedFromPositiveValueTo0()
    {
        var savedMetImage = ImageId.New();
        var savedUnmetImage = ImageId.New();

        Subject.OpenedInDocuments = 1;

        Subject.SavedData.Content = new DynamicMapPrototype
        {
            MetImage = savedMetImage,
            UnmetImage = savedUnmetImage
        };

        Subject.WorkingData.Content = new StaticMapPrototype();
        
        Subject.OpenedInDocuments = 0;

        Subject.SavedData
            .Content
            .Should()
            .BeOfType<DynamicMapPrototype>()
            .Subject
            .MetImage
            .Should()
            .Be(savedMetImage);
        
        Subject.SavedData
            .Content
            .Should()
            .BeOfType<DynamicMapPrototype>()
            .Subject
            .UnmetImage
            .Should()
            .Be(savedUnmetImage);

        Subject.WorkingData
            .Content
            .Should()
            .BeOfType<StaticMapPrototype>();
    }

    [Fact]
    public void OpenedInDocuments_ShouldNotRevertWorkingData_WhenChangedFromPositiveValueToPositiveValue()
    {
        var savedMetImage = ImageId.New();
        var savedUnmetImage = ImageId.New();

        Subject.OpenedInDocuments = 1;

        Subject.SavedData.Content = new DynamicMapPrototype
        {
            MetImage = savedMetImage,
            UnmetImage = savedUnmetImage
        };
        
        Subject.WorkingData.Content = new StaticMapPrototype();

        Subject.OpenedInDocuments = 2;

        Subject.SavedData
            .Content
            .Should()
            .BeOfType<DynamicMapPrototype>()
            .Subject
            .MetImage
            .Should()
            .Be(savedMetImage);
        
        Subject.SavedData
            .Content
            .Should()
            .BeOfType<DynamicMapPrototype>()
            .Subject
            .UnmetImage
            .Should()
            .Be(savedUnmetImage);

        Subject.WorkingData
            .Content
            .Should()
            .BeOfType<StaticMapPrototype>();
    }

    [Fact]
    public async Task LoadDataFromFileAsync_ShouldSetSavedDataToFileContents()
    {
        const string friendlyId = "Test ID";
        
        var metImage = ImageId.New();
        var unmetImage = ImageId.New();

        var fileData = new NamedData<MapPrototype>
        {
            Name = friendlyId,
            Data = new MapPrototype
            {
                Content = new DynamicMapPrototype
                {
                    MetImage = metImage,
                    UnmetImage = unmetImage
                }
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
            .Content
            .Should()
            .BeOfType<DynamicMapPrototype>()
            .Subject
            .MetImage
            .Should()
            .Be(metImage);
        
        Subject.SavedData
            .Content
            .Should()
            .BeOfType<DynamicMapPrototype>()
            .Subject
            .UnmetImage
            .Should()
            .Be(unmetImage);

        Subject.WorkingData
            .Content
            .Should()
            .BeOfType<DynamicMapPrototype>()
            .Subject
            .MetImage
            .Should()
            .Be(metImage);
        
        Subject.WorkingData
            .Content
            .Should()
            .BeOfType<DynamicMapPrototype>()
            .Subject
            .UnmetImage
            .Should()
            .Be(unmetImage);
    }
    
    [Fact]
    public async Task LoadDataFromFileAsync_ShouldNotChangeValues_WhenFileDoesNotContainValidData()
    {
        var savedMetImage = ImageId.New();
        var savedUnmetImage = ImageId.New();

        Subject.SavedData.Content = new DynamicMapPrototype
        {
            MetImage = savedMetImage,
            UnmetImage = savedUnmetImage
        };
        
        Subject.WorkingData.Content = new StaticMapPrototype();
        
        await Subject.LoadDataFromFileAsync();
        
        Subject.SavedData
            .Content
            .Should()
            .BeOfType<DynamicMapPrototype>()
            .Subject
            .MetImage
            .Should()
            .Be(savedMetImage);
        
        Subject.SavedData
            .Content
            .Should()
            .BeOfType<DynamicMapPrototype>()
            .Subject
            .UnmetImage
            .Should()
            .Be(savedUnmetImage);

        Subject.WorkingData
            .Content
            .Should()
            .BeOfType<StaticMapPrototype>();
    }
    
    [Fact]
    public void Revert_ShouldMakeWorkingDataEqualToSavedData()
    {
        var expectedMetImage = ImageId.New();
        var expectedUnmetImage = ImageId.New();
        
        Subject.SavedData.Content = new DynamicMapPrototype
        {
            MetImage = expectedMetImage,
            UnmetImage = expectedUnmetImage
        };
        
        Subject.WorkingData.Content = new StaticMapPrototype();

        Subject.Revert();

        Subject.WorkingData
            .Content
            .Should()
            .BeOfType<DynamicMapPrototype>()
            .Subject
            .MetImage
            .Should()
            .Be(expectedMetImage);
        
        Subject.WorkingData
            .Content
            .Should()
            .BeOfType<DynamicMapPrototype>()
            .Subject
            .UnmetImage
            .Should()
            .Be(expectedUnmetImage);
    }
    
    [Fact]
    public async Task SaveAsync_ShouldSaveWorkingDataToFile()
    {
        const string expectedFriendlyId = "Test ID";
        
        var expectedMetImage = ImageId.New();
        var expectedUnmetImage = ImageId.New();
        
        Subject.WorkingData.Content = new DynamicMapPrototype
        {
            MetImage = expectedMetImage,
            UnmetImage = expectedUnmetImage
        };

        await Subject.RenameAsync(expectedFriendlyId);
        await Subject.SaveAsync();

        await using var stream = File.OpenRead();
        var namedJsonData = await JsonContext.DeserializeAsync<NamedData<MapPrototype>>(stream);

        namedJsonData.Should().NotBeNull();
        
        namedJsonData.Name
            .Should()
            .Be(expectedFriendlyId);
        
        namedJsonData.Data
            .Content
            .Should()
            .BeOfType<DynamicMapPrototype>()
            .Subject
            .MetImage
            .Should()
            .Be(expectedMetImage);
        
        namedJsonData.Data
            .Content
            .Should()
            .BeOfType<DynamicMapPrototype>()
            .Subject
            .UnmetImage
            .Should()
            .Be(expectedUnmetImage);

        Subject.FriendlyId
            .Should()
            .Be(expectedFriendlyId);
        
        Subject.SavedData
            .Content
            .Should()
            .BeOfType<DynamicMapPrototype>()
            .Subject
            .MetImage
            .Should()
            .Be(expectedMetImage);
        
        Subject.SavedData
            .Content
            .Should()
            .BeOfType<DynamicMapPrototype>()
            .Subject
            .UnmetImage
            .Should()
            .Be(expectedUnmetImage);

        Subject.WorkingData
            .Content
            .Should()
            .BeOfType<DynamicMapPrototype>()
            .Subject
            .MetImage
            .Should()
            .Be(expectedMetImage);
        
        Subject.WorkingData
            .Content
            .Should()
            .BeOfType<DynamicMapPrototype>()
            .Subject
            .UnmetImage
            .Should()
            .Be(expectedUnmetImage);
    }
}