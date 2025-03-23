using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using OpenTrackerNext.Core.Factories;
using OpenTrackerNext.Core.ViewModels;
using OpenTrackerNext.Editor.Documents;
using OpenTrackerNext.Editor.ViewModels.Documents;
using Splat;
using Xunit;

namespace OpenTrackerNext.Editor.UnitTests.ViewModels.Documents;

[ExcludeFromCodeCoverage]
public sealed class DocumentViewModelTests
{
    private sealed class MockViewModel : ViewModel;
    
    private readonly CompositeDisposable _disposables = new();
    private readonly IFactory<IDocument, ViewModel> _documentViewModelFactory =
        Substitute.For<IFactory<IDocument, ViewModel>>();
    private readonly IDocumentService _documentService = Substitute.For<IDocumentService>();
    private readonly IDocument _document = Substitute.For<IDocument>();
    private readonly DocumentBodyViewModel _body;
    private readonly MockViewModel _content = new();

    private readonly DocumentViewModel _subject;

    public DocumentViewModelTests()
    {
        _documentViewModelFactory
            .Create(Arg.Is(_document))
            .Returns(_content);
        
        _body = new DocumentBodyViewModel(_documentViewModelFactory, _document);

        _subject = new DocumentViewModel(_documentService, DocumentBodyFactory, _document);

        _subject.Activator
            .Activate()
            .DisposeWith(_disposables);

        _body.Activator
            .Activate()
            .DisposeWith(_disposables);
    }
    
    private DocumentBodyViewModel DocumentBodyFactory(IDocument _)
    {
        return _body;
    }

    [Fact]
    public void Document_ShouldReturnExpected()
    {
        _subject.Document
            .Should()
            .Be(_document);
    }

    [Fact]
    public void Body_ShouldReturnExpected()
    {
        _subject.Body
            .Should()
            .Be(_body);
    }

    [Fact]
    public async Task CloseCommand_ShouldCallTryCloseAsyncOnDocumentService()
    {
        _subject.CloseCommand
            .Execute()
            .Subscribe()
            .DisposeWith(_disposables);

        await _documentService
            .Received(1)
            .TryCloseAsync(Arg.Is(_document));
    }

    [Fact]
    public void SplatResolve_ShouldResolveToFactory()
    {
        Locator.CurrentMutable
            .Register(
                () => _documentViewModelFactory,
                typeof(IFactory<IDocument, ViewModel>));
        
        var factory = Locator.Current.GetService<DocumentViewModel.Factory>()!;

        var subject1 = factory(_document);
        var subject2 = factory(_document);
        
        subject1.Should().BeOfType<DocumentViewModel>();
        subject2.Should().BeOfType<DocumentViewModel>();
        
        subject1.Should().NotBeSameAs(subject2);
    }
}