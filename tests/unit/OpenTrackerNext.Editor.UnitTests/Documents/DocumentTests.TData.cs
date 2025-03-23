using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Editor.Documents;
using Splat;
using Xunit;

namespace OpenTrackerNext.Editor.UnitTests.Documents;

[ExcludeFromCodeCoverage]
public abstract class DocumentTests<TData>
    where TData : class, ITitledDocumentData<TData>, new()
{
    protected readonly CompositeDisposable Disposables = new();
    protected readonly string TitlePrefix;
    protected readonly TData WorkingData = new();
    protected readonly IDocumentFile<TData> File = Substitute.For<IDocumentFile<TData>>();
    
    protected readonly Document<TData> Subject;
    
    private readonly TData _savedData = new();

    protected DocumentTests(string titlePrefix)
    {
        TitlePrefix = titlePrefix;
        
        File.TitlePrefix.Returns(titlePrefix);
        File.SavedData.Returns(_savedData);
        File.WorkingData.Returns(WorkingData);

        Subject = new Document<TData>(File);
        
        Subject.DisposeWith(Disposables);
    }

    [Fact]
    public void File_ShouldReturnConstructorParameter()
    {
        Subject.File
            .Should()
            .Be(File);
    }
    
    [Theory]
    [InlineData("FriendlyName1")]
    [InlineData("FriendlyName2")]
    public void BaseTitle_ShouldReturnExpected(string friendlyId)
    {
        var expected = $"{TitlePrefix}{friendlyId}";

        File.FriendlyId.Returns(friendlyId);

        File.PropertyChanged += Raise
            .Event<PropertyChangedEventHandler>(
                File,
                new PropertyChangedEventArgs(nameof(IDocumentFile.FriendlyId)));

        Subject.BaseTitle
            .Should()
            .Be(expected);
    }

    [Fact]
    public void BaseTitle_ShouldRaisePropertyChanged()
    {
        using var monitor = Subject.Monitor();

        File.FriendlyId.Returns("FriendlyName");
        
        File.PropertyChanged += Raise
            .Event<PropertyChangedEventHandler>(
                File,
                new PropertyChangedEventArgs(nameof(IDocumentFile.FriendlyId)));
        
        monitor.Should().RaisePropertyChangeFor(x => x.BaseTitle);
    }
    
    [Fact]
    public void Revert_ShouldCallRevertOnContent()
    {
        Subject.Revert();
        
        File.Received(1).Revert();
    }

    [Fact]
    public async Task SaveAsync_ShouldCallSaveOnContent()
    {
        await Subject.SaveAsync();
        
        await File.Received(1).SaveAsync();
    }

    [Fact]
    public void SplatResolve_EntityPrototype_ShouldResolveToInterfaceFactory()
    {
        var factory = Locator.Current.GetService<IDocument<TData>.Factory>()!;

        var subject1 = factory(File);
        var subject2 = factory(File);

        subject1.Should().BeOfType<Document<TData>>();
        subject2.Should().BeOfType<Document<TData>>();
        
        subject1.Should().NotBeSameAs(subject2);
    }
}