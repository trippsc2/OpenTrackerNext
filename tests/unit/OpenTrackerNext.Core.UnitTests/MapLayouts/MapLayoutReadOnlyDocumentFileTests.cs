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
public sealed class MapLayoutReadOnlyDocumentFileTests : ReadOnlyDocumentFileTests<MapLayoutPrototype, MapLayoutId>
{
    [Fact]
    public async Task LoadDataFromFileAsync_ShouldSetSavedDataToFileContents()
    {
        var map1 = MapId.New();
        var map2 = MapId.New();

        var fileData = new NamedData<MapLayoutPrototype>
        {
            Name = "Test",
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

        Subject.Data
            .Maps
            .Should()
            .HaveCount(2)
            .And.Contain(x => x.Map == map1)
            .And.Contain(x => x.Map == map2);
    }
}