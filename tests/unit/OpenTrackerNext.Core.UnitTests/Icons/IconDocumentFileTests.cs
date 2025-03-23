using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Icons;
using OpenTrackerNext.Core.Icons.Counted;
using OpenTrackerNext.Core.Icons.Static;
using OpenTrackerNext.Core.Images;
using OpenTrackerNext.Core.Json;
using OpenTrackerNext.Core.UnitTests.Documents;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Icons;

[ExcludeFromCodeCoverage]
public sealed class IconDocumentFileTests() : DocumentFileTests<IconPrototype, IconId>(IconPrototype.TitlePrefix)
{
    [Fact]
    public void SavedData_ShouldInitializeAsDefault()
    {
        Subject.SavedData
            .Content
            .Should()
            .BeOfType<StaticIconPrototype>()
            .Subject
            .Image
            .Should()
            .Be(ImageId.Empty);
    }
    
    [Fact]
    public void WorkingData_ShouldInitializeAsDefault()
    {
        Subject.WorkingData
            .Content
            .Should()
            .BeOfType<StaticIconPrototype>()
            .Subject
            .Image
            .Should()
            .Be(ImageId.Empty);
    }
    
    [Fact]
    public void OpenedInDocuments_ShouldRevertWorkingData_WhenChangedFrom0ToPositiveValue()
    {
        var expectedImage = ImageId.New();

        Subject.SavedData.Content = new StaticIconPrototype { Image = expectedImage };
        
        Subject.WorkingData.Content = new CountedIconPrototype();
        
        Subject.OpenedInDocuments = 1;

        Subject.SavedData
            .Content
            .Should()
            .BeOfType<StaticIconPrototype>()
            .Subject
            .Image
            .Should()
            .Be(expectedImage);

        Subject.WorkingData
            .Content
            .Should()
            .BeOfType<StaticIconPrototype>()
            .Subject
            .Image
            .Should()
            .Be(expectedImage);
    }
    
    [Fact]
    public void OpenedInDocuments_ShouldNotRevertWorkingData_WhenChangedFromPositiveValueTo0()
    {
        var savedImage = ImageId.New();

        Subject.OpenedInDocuments = 1;
        
        Subject.SavedData.Content = new StaticIconPrototype { Image = savedImage };
        Subject.WorkingData.Content = new CountedIconPrototype();
        
        Subject.OpenedInDocuments = 0;

        Subject.SavedData
            .Content
            .Should()
            .BeOfType<StaticIconPrototype>()
            .Subject
            .Image
            .Should()
            .Be(savedImage);

        Subject.WorkingData
            .Content
            .Should()
            .BeOfType<CountedIconPrototype>();
    }

    [Fact]
    public void OpenedInDocuments_ShouldNotRevertWorkingData_WhenChangedFromPositiveValueToPositiveValue()
    {
        var savedImage = ImageId.New();

        Subject.OpenedInDocuments = 1;
        
        Subject.SavedData.Content = new StaticIconPrototype { Image = savedImage };
        Subject.WorkingData.Content = new CountedIconPrototype();

        Subject.OpenedInDocuments = 2;

        Subject.SavedData
            .Content
            .Should()
            .BeOfType<StaticIconPrototype>()
            .Subject
            .Image
            .Should()
            .Be(savedImage);

        Subject.WorkingData
            .Content
            .Should()
            .BeOfType<CountedIconPrototype>();
    }

    [Fact]
    public async Task LoadDataFromFileAsync_ShouldSetSavedDataToFileContents()
    {
        const string friendlyId = "Test ID";
        
        var image = ImageId.New();

        var fileData = new NamedData<IconPrototype>
        {
            Name = friendlyId,
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

        Subject.FriendlyId
            .Should()
            .Be(friendlyId);

        Subject.SavedData
            .Content
            .Should()
            .BeOfType<StaticIconPrototype>()
            .Subject
            .Image
            .Should()
            .Be(image);

        Subject.WorkingData
            .Content
            .Should()
            .BeOfType<StaticIconPrototype>()
            .Subject
            .Image
            .Should()
            .Be(image);
    }
    
    [Fact]
    public async Task LoadDataFromFileAsync_ShouldNotChangeValues_WhenFileDoesNotContainValidData()
    {
        var savedImage = ImageId.New();
        
        Subject.SavedData.Content = new StaticIconPrototype { Image = savedImage };
        Subject.WorkingData.Content = new CountedIconPrototype();
        
        await Subject.LoadDataFromFileAsync();
        
        Subject.SavedData
            .Content
            .Should()
            .BeOfType<StaticIconPrototype>()
            .Subject
            .Image
            .Should()
            .Be(savedImage);
        
        Subject.WorkingData
            .Content
            .Should()
            .BeOfType<CountedIconPrototype>();
    }
    
    [Fact]
    public void Revert_ShouldMakeWorkingDataEqualToSavedData()
    {
        var expectedImage = ImageId.New();
        
        Subject.SavedData.Content = new StaticIconPrototype { Image = expectedImage };
        Subject.WorkingData.Content = new CountedIconPrototype();

        Subject.Revert();

        Subject.WorkingData
            .Content
            .Should()
            .BeOfType<StaticIconPrototype>()
            .Subject
            .Image
            .Should()
            .Be(expectedImage);
    }
    
    [Fact]
    public async Task SaveAsync_ShouldSaveWorkingDataToFile()
    {
        const string expectedFriendlyId = "Test ID";
        
        var expectedImage = ImageId.New();
        
        Subject.WorkingData.Content = new StaticIconPrototype { Image = expectedImage };

        await Subject.RenameAsync(expectedFriendlyId);
        await Subject.SaveAsync();

        await using var stream = File.OpenRead();
        var namedJsonData = await JsonContext.DeserializeAsync<NamedData<IconPrototype>>(stream);

        namedJsonData.Should().NotBeNull();
        
        namedJsonData.Name
            .Should()
            .Be(expectedFriendlyId);
        
        namedJsonData.Data
            .Content
            .Should()
            .BeOfType<StaticIconPrototype>()
            .Subject
            .Image
            .Should()
            .Be(expectedImage);

        Subject.FriendlyId
            .Should()
            .Be(expectedFriendlyId);
        
        Subject.SavedData
            .Content
            .Should()
            .BeOfType<StaticIconPrototype>()
            .Subject
            .Image
            .Should()
            .Be(expectedImage);

        Subject.WorkingData
            .Content
            .Should()
            .BeOfType<StaticIconPrototype>()
            .Subject
            .Image
            .Should()
            .Be(expectedImage);
    }
}