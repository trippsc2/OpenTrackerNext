using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Json;
using OpenTrackerNext.Core.Layouts;
using OpenTrackerNext.Core.Layouts.Dynamic;
using OpenTrackerNext.Core.Layouts.IconGrid;
using OpenTrackerNext.Core.Layouts.UniformGrid;
using OpenTrackerNext.Core.UnitTests.Documents;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Layouts;

[ExcludeFromCodeCoverage]
public sealed class LayoutDocumentFileTests() : DocumentFileTests<LayoutPrototype, LayoutId>(LayoutPrototype.TitlePrefix)
{
    [Fact]
    public void SavedData_ShouldInitializeAsDefault()
    {
        Subject.SavedData
            .Content
            .Should()
            .BeOfType<IconGridLayoutPrototype>();
    }
    
    [Fact]
    public void WorkingData_ShouldInitializeAsDefault()
    {
        Subject.WorkingData
            .Content
            .Should()
            .BeOfType<IconGridLayoutPrototype>();
    }
    
    [Fact]
    public void OpenedInDocuments_ShouldRevertWorkingData_WhenChangedFrom0ToPositiveValue()
    {
        var expectedMetLayout = LayoutId.New();
        var expectedUnmetLayout = LayoutId.New();

        Subject.SavedData.Content = new DynamicLayoutPrototype
        {
            MetLayout = expectedMetLayout,
            UnmetLayout = expectedUnmetLayout
        };
        
        Subject.WorkingData.Content = new UniformGridLayoutPrototype();
        
        Subject.OpenedInDocuments = 1;

        Subject.SavedData
            .Content
            .Should()
            .BeOfType<DynamicLayoutPrototype>()
            .Subject
            .MetLayout
            .Should()
            .Be(expectedMetLayout);
        
        Subject.SavedData
            .Content
            .Should()
            .BeOfType<DynamicLayoutPrototype>()
            .Subject
            .UnmetLayout
            .Should()
            .Be(expectedUnmetLayout);

        Subject.WorkingData
            .Content
            .Should()
            .BeOfType<DynamicLayoutPrototype>()
            .Subject
            .MetLayout
            .Should()
            .Be(expectedMetLayout);
        
        Subject.WorkingData
            .Content
            .Should()
            .BeOfType<DynamicLayoutPrototype>()
            .Subject
            .UnmetLayout
            .Should()
            .Be(expectedUnmetLayout);
    }
    
    [Fact]
    public void OpenedInDocuments_ShouldNotRevertWorkingData_WhenChangedFromPositiveValueTo0()
    {
        var savedMetLayout = LayoutId.New();
        var savedUnmetLayout = LayoutId.New();

        Subject.OpenedInDocuments = 1;
        
        Subject.SavedData.Content = new DynamicLayoutPrototype
        {
            MetLayout = savedMetLayout,
            UnmetLayout = savedUnmetLayout
        };
        
        Subject.WorkingData.Content = new UniformGridLayoutPrototype();
        
        Subject.OpenedInDocuments = 0;

        Subject.SavedData
            .Content
            .Should()
            .BeOfType<DynamicLayoutPrototype>()
            .Subject
            .MetLayout
            .Should()
            .Be(savedMetLayout);
        
        Subject.SavedData
            .Content
            .Should()
            .BeOfType<DynamicLayoutPrototype>()
            .Subject
            .UnmetLayout
            .Should()
            .Be(savedUnmetLayout);

        Subject.WorkingData
            .Content
            .Should()
            .BeOfType<UniformGridLayoutPrototype>();
    }

    [Fact]
    public void OpenedInDocuments_ShouldNotRevertWorkingData_WhenChangedFromPositiveValueToPositiveValue()
    {
        var savedMetLayout = LayoutId.New();
        var savedUnmetLayout = LayoutId.New();

        Subject.OpenedInDocuments = 1;
        
        Subject.SavedData.Content = new DynamicLayoutPrototype
        {
            MetLayout = savedMetLayout,
            UnmetLayout = savedUnmetLayout
        };
        
        Subject.WorkingData.Content = new UniformGridLayoutPrototype();

        Subject.OpenedInDocuments = 2;

        Subject.SavedData
            .Content
            .Should()
            .BeOfType<DynamicLayoutPrototype>()
            .Subject
            .MetLayout
            .Should()
            .Be(savedMetLayout);
        
        Subject.SavedData
            .Content
            .Should()
            .BeOfType<DynamicLayoutPrototype>()
            .Subject
            .UnmetLayout
            .Should()
            .Be(savedUnmetLayout);

        Subject.WorkingData
            .Content
            .Should()
            .BeOfType<UniformGridLayoutPrototype>();
    }

    [Fact]
    public async Task LoadDataFromFileAsync_ShouldSetSavedDataToFileContents()
    {
        const string friendlyId = "Test ID";
        
        var metLayout = LayoutId.New();
        var unmetLayout = LayoutId.New();

        var fileData = new NamedData<LayoutPrototype>
        {
            Name = friendlyId,
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

        Subject.FriendlyId
            .Should()
            .Be(friendlyId);

        Subject.SavedData
            .Content
            .Should()
            .BeOfType<DynamicLayoutPrototype>()
            .Subject
            .MetLayout
            .Should()
            .Be(metLayout);
        
        Subject.SavedData
            .Content
            .Should()
            .BeOfType<DynamicLayoutPrototype>()
            .Subject
            .UnmetLayout
            .Should()
            .Be(unmetLayout);

        Subject.WorkingData
            .Content
            .Should()
            .BeOfType<DynamicLayoutPrototype>()
            .Subject
            .MetLayout
            .Should()
            .Be(metLayout);
        
        Subject.WorkingData
            .Content
            .Should()
            .BeOfType<DynamicLayoutPrototype>()
            .Subject
            .UnmetLayout
            .Should()
            .Be(unmetLayout);
    }
    
    [Fact]
    public async Task LoadDataFromFileAsync_ShouldNotChangeValues_WhenFileDoesNotContainValidData()
    {
        var savedMetLayout = LayoutId.New();
        var savedUnmetLayout = LayoutId.New();
        
        Subject.SavedData.Content = new DynamicLayoutPrototype
        {
            MetLayout = savedMetLayout,
            UnmetLayout = savedUnmetLayout
        };
        
        Subject.WorkingData.Content = new UniformGridLayoutPrototype();
        
        await Subject.LoadDataFromFileAsync();
        
        Subject.SavedData
            .Content
            .Should()
            .BeOfType<DynamicLayoutPrototype>()
            .Subject
            .MetLayout
            .Should()
            .Be(savedMetLayout);
        
        Subject.SavedData
            .Content
            .Should()
            .BeOfType<DynamicLayoutPrototype>()
            .Subject
            .UnmetLayout
            .Should()
            .Be(savedUnmetLayout);
        
        Subject.WorkingData
            .Content
            .Should()
            .BeOfType<UniformGridLayoutPrototype>();
    }
    
    [Fact]
    public void Revert_ShouldMakeWorkingDataEqualToSavedData()
    {
        var expectedMetLayout = LayoutId.New();
        var expectedUnmetLayout = LayoutId.New();
        
        Subject.SavedData.Content = new DynamicLayoutPrototype
        {
            MetLayout = expectedMetLayout,
            UnmetLayout = expectedUnmetLayout
        };
        
        Subject.WorkingData.Content = new UniformGridLayoutPrototype();

        Subject.Revert();

        Subject.WorkingData
            .Content
            .Should()
            .BeOfType<DynamicLayoutPrototype>()
            .Subject
            .MetLayout
            .Should()
            .Be(expectedMetLayout);
        
        Subject.WorkingData
            .Content
            .Should()
            .BeOfType<DynamicLayoutPrototype>()
            .Subject
            .UnmetLayout
            .Should()
            .Be(expectedUnmetLayout);
    }
    
    [Fact]
    public async Task SaveAsync_ShouldSaveWorkingDataToFile()
    {
        const string expectedFriendlyId = "Test ID";
        
        var expectedMetLayout = LayoutId.New();
        var expectedUnmetLayout = LayoutId.New();
        
        Subject.WorkingData.Content = new DynamicLayoutPrototype
        {
            MetLayout = expectedMetLayout,
            UnmetLayout = expectedUnmetLayout
        };

        await Subject.RenameAsync(expectedFriendlyId);
        await Subject.SaveAsync();

        await using var stream = File.OpenRead();
        var namedJsonData = await JsonContext.DeserializeAsync<NamedData<LayoutPrototype>>(stream);

        namedJsonData.Should().NotBeNull();
        
        namedJsonData.Name
            .Should()
            .Be(expectedFriendlyId);
        
        namedJsonData.Data
            .Content
            .Should()
            .BeOfType<DynamicLayoutPrototype>()
            .Subject
            .MetLayout
            .Should()
            .Be(expectedMetLayout);
        
        namedJsonData.Data
            .Content
            .Should()
            .BeOfType<DynamicLayoutPrototype>()
            .Subject
            .UnmetLayout
            .Should()
            .Be(expectedUnmetLayout);

        Subject.FriendlyId
            .Should()
            .Be(expectedFriendlyId);
        
        Subject.SavedData
            .Content
            .Should()
            .BeOfType<DynamicLayoutPrototype>()
            .Subject
            .MetLayout
            .Should()
            .Be(expectedMetLayout);
        
        Subject.SavedData
            .Content
            .Should()
            .BeOfType<DynamicLayoutPrototype>()
            .Subject
            .UnmetLayout
            .Should()
            .Be(expectedUnmetLayout);

        Subject.WorkingData
            .Content
            .Should()
            .BeOfType<DynamicLayoutPrototype>()
            .Subject
            .MetLayout
            .Should()
            .Be(expectedMetLayout);
        
        Subject.WorkingData
            .Content
            .Should()
            .BeOfType<DynamicLayoutPrototype>()
            .Subject
            .UnmetLayout
            .Should()
            .Be(expectedUnmetLayout);
    }
}