using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NSubstitute;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Metadata;
using OpenTrackerNext.Editor.UnitTests.Documents;
using Xunit;

namespace OpenTrackerNext.Editor.UnitTests.Metadata;

[ExcludeFromCodeCoverage]
public sealed class PackMetadataDocumentTests() : DocumentTests<PackMetadata>(PackMetadata.TitlePrefix)
{
    [Fact]
    public void IsUnsaved_ShouldReturnExpected()
    {
        Subject.IsUnsaved
            .Should()
            .BeFalse();

        WorkingData.Name = "Test Name";
        
        Subject.IsUnsaved
            .Should()
            .BeTrue();
    }

    [Fact]
    public void IsUnsaved_ShouldRaisePropertyChanged()
    {
        using var monitor = Subject.Monitor();
        
        WorkingData.Name = "Test Name";
        
        monitor.Should().RaisePropertyChangeFor(x => x.IsUnsaved);
    }

    [Theory]
    [InlineData("FriendlyName1", false)]
    [InlineData("FriendlyName1", true)]
    [InlineData("FriendlyName2", false)]
    [InlineData("FriendlyName2", true)]
    public void Title_ShouldReturnExpected(string friendlyId, bool isUnsaved)
    {
        var expected = $"{TitlePrefix}{friendlyId}{(isUnsaved ? "*" : "")}";

        File.FriendlyId.Returns(friendlyId);
        
        File.PropertyChanged += Raise
            .Event<PropertyChangedEventHandler>(
                File,
                new PropertyChangedEventArgs(nameof(IDocumentFile.FriendlyId)));

        if (isUnsaved)
        {
            WorkingData.Name = "Test Name";
        }
        
        Subject.Title
            .Should()
            .Be(expected);
    }
    
    [Fact]
    public void Title_ShouldRaisePropertyChanged()
    {
        using var monitor = Subject.Monitor();
        
        WorkingData.Name = "Test Name";
        
        monitor.Should().RaisePropertyChangeFor(x => x.Title);
    }
}