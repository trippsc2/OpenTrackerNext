using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Json;
using OpenTrackerNext.Core.MapLayouts;
using OpenTrackerNext.Core.Maps;
using OpenTrackerNext.Core.UnitTests.Documents;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.MapLayouts;

[ExcludeFromCodeCoverage]
public sealed class MapLayoutDocumentFileTests()
    : DocumentFileTests<MapLayoutPrototype, MapLayoutId>(MapLayoutPrototype.TitlePrefix)
{
    [Fact]
    public void SavedData_ShouldInitializeAsDefault()
    {
        Subject.SavedData
            .Maps
            .Should()
            .BeEmpty();
    }
    
    [Fact]
    public void WorkingData_ShouldInitializeAsDefault()
    {
        Subject.WorkingData
            .Maps
            .Should()
            .BeEmpty();
    }
    
    [Fact]
    public void OpenedInDocuments_ShouldRevertWorkingData_WhenChangedFrom0ToPositiveValue()
    {
        var expectedMap1 = MapId.New();
        var expectedMap2 = MapId.New();

        Subject.SavedData
            .Maps
            .Add(new MapLayoutMapPrototype { Map = expectedMap1 });
        
        Subject.SavedData
            .Maps
            .Add(new MapLayoutMapPrototype { Map = expectedMap2 });
        
        Subject.WorkingData.Maps.Clear();
        
        Subject.OpenedInDocuments = 1;

        Subject.SavedData
            .Maps
            .Should()
            .HaveCount(2)
            .And.Contain(x => x.Map == expectedMap1)
            .And.Contain(x => x.Map == expectedMap2);
        
        Subject.WorkingData
            .Maps
            .Should()
            .HaveCount(2)
            .And.Contain(x => x.Map == expectedMap1)
            .And.Contain(x => x.Map == expectedMap2);
    }
    
    [Fact]
    public void OpenedInDocuments_ShouldNotRevertWorkingData_WhenChangedFromPositiveValueTo0()
    {
        var savedMap1 = MapId.New();
        var savedMap2 = MapId.New();

        Subject.OpenedInDocuments = 1;
        
        Subject.SavedData
            .Maps
            .Add(new MapLayoutMapPrototype { Map = savedMap1 });
        Subject.SavedData
            .Maps
            .Add(new MapLayoutMapPrototype { Map = savedMap2 });
        
        Subject.WorkingData.Maps.Clear();
        
        Subject.OpenedInDocuments = 0;

        Subject.SavedData
            .Maps
            .Should()
            .HaveCount(2)
            .And.Contain(x => x.Map == savedMap1)
            .And.Contain(x => x.Map == savedMap2);

        Subject.WorkingData
            .Maps
            .Should()
            .BeEmpty();
    }

    [Fact]
    public void OpenedInDocuments_ShouldNotRevertWorkingData_WhenChangedFromPositiveValueToPositiveValue()
    {
        var savedMap1 = MapId.New();
        var savedMap2 = MapId.New();

        Subject.OpenedInDocuments = 1;
        
        Subject.SavedData
            .Maps
            .Add(new MapLayoutMapPrototype { Map = savedMap1 });
        Subject.SavedData
            .Maps
            .Add(new MapLayoutMapPrototype { Map = savedMap2 });
        
        Subject.WorkingData.Maps.Clear();

        Subject.OpenedInDocuments = 2;

        Subject.SavedData
            .Maps
            .Should()
            .HaveCount(2)
            .And.Contain(x => x.Map == savedMap1)
            .And.Contain(x => x.Map == savedMap2);

        Subject.WorkingData
            .Maps
            .Should()
            .BeEmpty();
    }

    [Fact]
    public async Task LoadDataFromFileAsync_ShouldSetSavedDataToFileContents()
    {
        const string friendlyId = "Test ID";
        
        var map1 = MapId.New();
        var map2 = MapId.New();

        var fileData = new NamedData<MapLayoutPrototype>
        {
            Name = friendlyId,
            Data = new MapLayoutPrototype
            {
                Maps =
                [
                    new MapLayoutMapPrototype { Map = map1 },
                    new MapLayoutMapPrototype { Map = map2 }
                ]
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
            .Maps
            .Should()
            .HaveCount(2)
            .And.Contain(x => x.Map == map1)
            .And.Contain(x => x.Map == map2);

        Subject.WorkingData
            .Maps
            .Should()
            .HaveCount(2)
            .And.Contain(x => x.Map == map1)
            .And.Contain(x => x.Map == map2);
    }
    
    [Fact]
    public async Task LoadDataFromFileAsync_ShouldNotChangeValues_WhenFileDoesNotContainValidData()
    {
        var savedMap1 = MapId.New();
        var savedMap2 = MapId.New();
        
        Subject.SavedData
            .Maps
            .Add(new MapLayoutMapPrototype { Map = savedMap1 });
        Subject.SavedData
            .Maps
            .Add(new MapLayoutMapPrototype { Map = savedMap2 });
        
        Subject.WorkingData.Maps.Clear();
        
        await Subject.LoadDataFromFileAsync();
        
        Subject.SavedData
            .Maps
            .Should()
            .HaveCount(2)
            .And.Contain(x => x.Map == savedMap1)
            .And.Contain(x => x.Map == savedMap2);

        Subject.WorkingData
            .Maps
            .Should()
            .BeEmpty();
    }
    
    [Fact]
    public void Revert_ShouldMakeWorkingDataEqualToSavedData()
    {
        var expectedMap1 = MapId.New();
        var expectedMap2 = MapId.New();
        
        Subject.SavedData
            .Maps
            .Add(new MapLayoutMapPrototype { Map = expectedMap1 });
        Subject.SavedData
            .Maps
            .Add(new MapLayoutMapPrototype { Map = expectedMap2 });
        
        Subject.WorkingData.Maps.Clear();

        Subject.Revert();

        Subject.WorkingData
            .Maps
            .Should()
            .HaveCount(2)
            .And.Contain(x => x.Map == expectedMap1)
            .And.Contain(x => x.Map == expectedMap2);
    }
    
    [Fact]
    public async Task SaveAsync_ShouldSaveWorkingDataToFile()
    {
        const string expectedFriendlyId = "Test ID";
        
        var expectedMap1 = MapId.New();
        var expectedMap2 = MapId.New();
        
        Subject.WorkingData
            .Maps
            .Add(new MapLayoutMapPrototype { Map = expectedMap1 });
        
        Subject.WorkingData
            .Maps
            .Add(new MapLayoutMapPrototype { Map = expectedMap2 });

        await Subject.RenameAsync(expectedFriendlyId);
        await Subject.SaveAsync();

        await using var stream = File.OpenRead();
        var namedJsonData = await JsonContext.DeserializeAsync<NamedData<MapLayoutPrototype>>(stream);

        namedJsonData.Should().NotBeNull();
        
        namedJsonData.Name
            .Should()
            .Be(expectedFriendlyId);
        
        namedJsonData.Data
            .Maps
            .Should()
            .HaveCount(2)
            .And.Contain(x => x.Map == expectedMap1)
            .And.Contain(x => x.Map == expectedMap2);

        Subject.FriendlyId
            .Should()
            .Be(expectedFriendlyId);
        
        Subject.SavedData
            .Maps
            .Should()
            .HaveCount(2)
            .And.Contain(x => x.Map == expectedMap1)
            .And.Contain(x => x.Map == expectedMap2);

        Subject.WorkingData
            .Maps
            .Should()
            .HaveCount(2)
            .And.Contain(x => x.Map == expectedMap1)
            .And.Contain(x => x.Map == expectedMap2);
    }
}