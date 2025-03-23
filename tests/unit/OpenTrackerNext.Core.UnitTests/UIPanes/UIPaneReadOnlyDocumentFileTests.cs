using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Json;
using OpenTrackerNext.Core.Layouts;
using OpenTrackerNext.Core.UIPanes;
using OpenTrackerNext.Core.UnitTests.Documents;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.UIPanes;

[ExcludeFromCodeCoverage]
public sealed class UIPaneReadOnlyDocumentFileTests : ReadOnlyDocumentFileTests<UIPanePrototype, UIPaneId>
{
    [Fact]
    public async Task LoadDataFromFileAsync_ShouldSetSavedDataToFileContents()
    {
        const string title = "Test Title";
        
        var body = LayoutId.New();

        var fileData = new NamedData<UIPanePrototype>
        {
            Name = "Test",
            Data = new UIPanePrototype
            {
                Title = title,
                Body = body
            }
        };
        
        var writeStream = File.OpenWrite();
        
        await JsonContext.SerializeAsync(writeStream, fileData);

        await writeStream.FlushAsync();
        await writeStream.DisposeAsync();
        
        await Subject.LoadDataFromFileAsync();

        Subject.Data
            .Title
            .Should()
            .Be(title);
        Subject.Data
            .Body
            .Should()
            .Be(body);
    }
}