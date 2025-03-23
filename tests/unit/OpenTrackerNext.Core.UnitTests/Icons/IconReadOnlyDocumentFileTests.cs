using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Icons;
using OpenTrackerNext.Core.Icons.Static;
using OpenTrackerNext.Core.Images;
using OpenTrackerNext.Core.Json;
using OpenTrackerNext.Core.UnitTests.Documents;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Icons;

[ExcludeFromCodeCoverage]
public sealed class IconReadOnlyDocumentFileTests : ReadOnlyDocumentFileTests<IconPrototype, IconId>
{
    [Fact]
    public async Task LoadDataFromFileAsync_ShouldSetSavedDataToFileContents()
    {
        var image = ImageId.New();

        var fileData = new NamedData<IconPrototype>
        {
            Name = "Test",
            Data = new IconPrototype
            {
                Content = new StaticIconPrototype { Image = image }
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
            .BeOfType<StaticIconPrototype>()
            .Subject
            .Image
            .Should()
            .Be(image);
    }
}