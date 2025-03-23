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
public sealed class EntityReadOnlyDocumentFileTests : ReadOnlyDocumentFileTests<EntityPrototype, EntityId>
{
    [Fact]
    public async Task LoadDataFromFileAsync_ShouldSetSavedDataToFileContents()
    {
        const int minimum = 1;
        const int starting = 2;
        const int maximum = 3;

        var fileData = new NamedData<EntityPrototype>
        {
            Name = "Test",
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

        Subject.Data
            .Minimum
            .Should()
            .Be(minimum);
        
        Subject.Data
            .Starting
            .Should()
            .Be(starting);
        
        Subject.Data
            .Maximum
            .Should()
            .Be(maximum);
    }
}