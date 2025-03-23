using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Images;
using OpenTrackerNext.Core.Json;
using OpenTrackerNext.Core.Maps;
using OpenTrackerNext.Core.Maps.Dynamic;
using OpenTrackerNext.Core.UnitTests.Documents;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Maps;

[ExcludeFromCodeCoverage]
public sealed class MapReadOnlyDocumentFileTests : ReadOnlyDocumentFileTests<MapPrototype, MapId>
{
    [Fact]
    public async Task LoadDataFromFileAsync_ShouldSetSavedDataToFileContents()
    {
        var metImage = ImageId.New();
        var unmetImage = ImageId.New();

        var fileData = new NamedData<MapPrototype>
        {
            Name = "Test",
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

        Subject.Data
            .Content
            .Should()
            .BeOfType<DynamicMapPrototype>()
            .Subject
            .MetImage
            .Should()
            .Be(metImage);
        
        Subject.Data
            .Content
            .Should()
            .BeOfType<DynamicMapPrototype>()
            .Subject
            .UnmetImage
            .Should()
            .Be(unmetImage);
    }
}