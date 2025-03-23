using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Json;
using OpenTrackerNext.Core.Layouts;
using OpenTrackerNext.Core.Layouts.Dynamic;
using OpenTrackerNext.Core.UnitTests.Documents;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Layouts;

[ExcludeFromCodeCoverage]
public sealed class LayoutReadOnlyDocumentFileTests : ReadOnlyDocumentFileTests<LayoutPrototype, LayoutId>
{
    [Fact]
    public async Task LoadDataFromFileAsync_ShouldSetSavedDataToFileContents()
    {
        var metLayout = LayoutId.New();
        var unmetLayout = LayoutId.New();

        var fileData = new NamedData<LayoutPrototype>
        {
            Name = "Test",
            Data = new LayoutPrototype
            {
                Content = new DynamicLayoutPrototype
                {
                    MetLayout = metLayout,
                    UnmetLayout = unmetLayout
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
            .BeOfType<DynamicLayoutPrototype>()
            .Subject
            .MetLayout
            .Should()
            .Be(metLayout);
        
        Subject.Data
            .Content
            .Should()
            .BeOfType<DynamicLayoutPrototype>()
            .Subject
            .UnmetLayout
            .Should()
            .Be(unmetLayout);
    }
}