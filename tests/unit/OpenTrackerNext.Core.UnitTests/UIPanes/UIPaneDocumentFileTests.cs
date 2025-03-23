using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Json;
using OpenTrackerNext.Core.Layouts;
using OpenTrackerNext.Core.Requirements;
using OpenTrackerNext.Core.UIPanes;
using OpenTrackerNext.Core.UIPanes.Popup;
using OpenTrackerNext.Core.UnitTests.Documents;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.UIPanes;

[ExcludeFromCodeCoverage]
public sealed class UIPaneDocumentFileTests()
    : DocumentFileTests<UIPanePrototype, UIPaneId>(UIPanePrototype.TitlePrefix)
{
    [Fact]
    public void SavedData_ShouldInitializeAsDefault()
    {
        Subject.SavedData
            .Visibility
            .Content
            .Should()
            .BeOfType<NullRequirementPrototype>();
        
        Subject.SavedData
            .Popup
            .Should()
            .BeOfType<NullUIPanePopupPrototype>();
        
        Subject.SavedData
            .Title
            .Should()
            .BeEmpty();
        
        Subject.SavedData
            .Body
            .Should()
            .Be(LayoutId.Empty);
    }
    
    [Fact]
    public void WorkingData_ShouldInitializeAsDefault()
    {
        Subject.WorkingData
            .Visibility
            .Content
            .Should()
            .BeOfType<NullRequirementPrototype>();
        
        Subject.WorkingData
            .Popup
            .Should()
            .BeOfType<NullUIPanePopupPrototype>();
        
        Subject.WorkingData
            .Title
            .Should()
            .BeEmpty();
        
        Subject.WorkingData
            .Body
            .Should()
            .Be(LayoutId.Empty);
    }
    
    [Fact]
    public void OpenedInDocuments_ShouldRevertWorkingData_WhenChangedFrom0ToPositiveValue()
    {
        const string expectedTitle = "Test Title";
        
        var expectedBody = LayoutId.New();

        Subject.SavedData.Title = expectedTitle;
        Subject.SavedData.Body = expectedBody;

        Subject.WorkingData.Title = "Test Title 2";
        Subject.WorkingData.Body = LayoutId.New();
        
        Subject.OpenedInDocuments = 1;

        Subject.SavedData
            .Title
            .Should()
            .Be(expectedTitle);
        
        Subject.SavedData
            .Body
            .Should()
            .Be(expectedBody);
        
        Subject.WorkingData
            .Title
            .Should()
            .Be(expectedTitle);
        
        Subject.WorkingData
            .Body
            .Should()
            .Be(expectedBody);
    }
    
    [Fact]
    public void OpenedInDocuments_ShouldNotRevertWorkingData_WhenChangedFromPositiveValueTo0()
    {
        const string savedTitle = "Test Title";
        const string workingTitle = "Test Title 2";
        
        var savedBody = LayoutId.New();
        var workingBody = LayoutId.New();

        Subject.OpenedInDocuments = 1;

        Subject.SavedData.Title = savedTitle;
        Subject.SavedData.Body = savedBody;

        Subject.WorkingData.Title = workingTitle;
        Subject.WorkingData.Body = workingBody;
        
        Subject.OpenedInDocuments = 0;

        Subject.SavedData
            .Title
            .Should()
            .Be(savedTitle);
        
        Subject.SavedData
            .Body
            .Should()
            .Be(savedBody);

        Subject.WorkingData
            .Title
            .Should()
            .Be(workingTitle);
        
        Subject.WorkingData
            .Body
            .Should()
            .Be(workingBody);
    }

    [Fact]
    public void OpenedInDocuments_ShouldNotRevertWorkingData_WhenChangedFromPositiveValueToPositiveValue()
    {
        const string savedTitle = "Test Title";
        const string workingTitle = "Test Title 2";
        
        var savedBody = LayoutId.New();
        var workingBody = LayoutId.New();

        Subject.OpenedInDocuments = 1;

        Subject.SavedData.Title = savedTitle;
        Subject.SavedData.Body = savedBody;
        
        Subject.WorkingData.Title = workingTitle;
        Subject.WorkingData.Body = workingBody;

        Subject.OpenedInDocuments = 2;

        Subject.SavedData
            .Title
            .Should()
            .Be(savedTitle);
        
        Subject.SavedData
            .Body
            .Should()
            .Be(savedBody);

        Subject.WorkingData
            .Title
            .Should()
            .Be(workingTitle);
        
        Subject.WorkingData
            .Body
            .Should()
            .Be(workingBody);
    }

    [Fact]
    public async Task LoadDataFromFileAsync_ShouldSetSavedDataToFileContents()
    {
        const string friendlyId = "Test ID";
        const string title = "Test Title";
        
        var body = LayoutId.New();

        var fileData = new NamedData<UIPanePrototype>
        {
            Name = friendlyId,
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

        Subject.FriendlyId
            .Should()
            .Be(friendlyId);

        Subject.SavedData
            .Title
            .Should()
            .Be(title);
        
        Subject.SavedData
            .Body
            .Should()
            .Be(body);

        Subject.WorkingData
            .Title
            .Should()
            .Be(title);
        
        Subject.WorkingData
            .Body
            .Should()
            .Be(body);
    }
    
    [Fact]
    public async Task LoadDataFromFileAsync_ShouldNotChangeValues_WhenFileDoesNotContainValidData()
    {
        const string savedTitle = "Test Title";
        const string workingTitle = "Test Title 2";
        
        var savedBody = LayoutId.New();
        var workingBody = LayoutId.New();

        Subject.SavedData.Title = savedTitle;
        Subject.SavedData.Body = savedBody;
        
        Subject.WorkingData.Title = workingTitle;
        Subject.WorkingData.Body = workingBody;
        
        await Subject.LoadDataFromFileAsync();
        
        Subject.SavedData
            .Title
            .Should()
            .Be(savedTitle);
        
        Subject.SavedData
            .Body
            .Should()
            .Be(savedBody);

        Subject.WorkingData
            .Title
            .Should()
            .Be(workingTitle);
        
        Subject.WorkingData
            .Body
            .Should()
            .Be(workingBody);
    }
    
    [Fact]
    public void Revert_ShouldMakeWorkingDataEqualToSavedData()
    {
        const string expectedTitle = "Test Title";
        
        var expectedBody = LayoutId.New();
        
        Subject.SavedData.Title = expectedTitle;
        Subject.SavedData.Body = expectedBody;
        
        Subject.WorkingData.Title = "Test Title 2";
        Subject.WorkingData.Body = LayoutId.New();

        Subject.Revert();

        Subject.WorkingData
            .Title
            .Should()
            .Be(expectedTitle);
        
        Subject.WorkingData
            .Body
            .Should()
            .Be(expectedBody);
    }
    
    [Fact]
    public async Task SaveAsync_ShouldSaveWorkingDataToFile()
    {
        const string expectedFriendlyId = "Test ID";
        const string expectedTitle = "Test Title";
        
        var expectedBody = LayoutId.New();
        
        Subject.WorkingData.Title = expectedTitle;
        Subject.WorkingData.Body = expectedBody;

        await Subject.RenameAsync(expectedFriendlyId);
        await Subject.SaveAsync();

        await using var stream = File.OpenRead();
        var namedJsonData = await JsonContext.DeserializeAsync<NamedData<UIPanePrototype>>(stream);

        namedJsonData.Should().NotBeNull();
        
        namedJsonData.Name
            .Should()
            .Be(expectedFriendlyId);
        
        namedJsonData.Data
            .Title
            .Should()
            .Be(expectedTitle);
        
        namedJsonData.Data
            .Body
            .Should()
            .Be(expectedBody);

        Subject.FriendlyId
            .Should()
            .Be(expectedFriendlyId);
        
        Subject.SavedData
            .Title
            .Should()
            .Be(expectedTitle);
        
        Subject.SavedData
            .Body
            .Should()
            .Be(expectedBody);

        Subject.WorkingData
            .Title
            .Should()
            .Be(expectedTitle);
        
        Subject.WorkingData
            .Body
            .Should()
            .Be(expectedBody);
    }
}