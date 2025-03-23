using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using OpenTrackerNext.Core.Factories;
using OpenTrackerNext.Core.ViewModels;
using OpenTrackerNext.Editor.Documents;
using OpenTrackerNext.Editor.ViewModels.Documents;
using ReactiveUI;
using Splat;
using Xunit;

namespace OpenTrackerNext.Editor.UnitTests.ViewModels.Documents;

[ExcludeFromCodeCoverage]
public sealed class DocumentDockViewModelTests
{
    private sealed class MockViewModel : ViewModel;
    
    private readonly CompositeDisposable _disposables = new();
    private readonly DocumentService _documentService = new();
    private readonly IFactory<IDocument, ViewModel> _documentViewModelFactory =
        Substitute.For<IFactory<IDocument, ViewModel>>();

    public DocumentDockViewModelTests()
    {
        _documentViewModelFactory
            .Create(Arg.Any<IDocument>())
            .Returns(_ => new MockViewModel());
    }
    
    private static IDocument CreateMockDocument()
    {
        var document = Substitute.For<IDocument>();
        
        return document;
    }

    private DocumentBodyViewModel DocumentBodyFactory(IDocument document)
    {
        return new DocumentBodyViewModel(_documentViewModelFactory, document);
    }

    private DocumentViewModel DocumentViewModelFactory(IDocument document)
    {
        return new DocumentViewModel(_documentService, DocumentBodyFactory, document);
    }
    
    [Theory]
    [InlineData(DocumentSide.Left)]
    [InlineData(DocumentSide.Right)]
    public void Documents_ShouldIncludeDocumentsOnTheMatchingSide(DocumentSide side)
    {
        var document = CreateMockDocument();
        
        document.Side.Returns(side);

        _documentService.Add(document);

        var subject = new DocumentDockViewModel(
            _documentService,
            DocumentViewModelFactory,
            side);
        
        subject.Activator
            .Activate()
            .DisposeWith(_disposables);

        subject.Documents
            .Should()
            .HaveCount(1)
            .And.Contain(x => x.Document == document);
    }

    [Theory]
    [InlineData(DocumentSide.Left)]
    [InlineData(DocumentSide.Right)]
    public void Documents_ShouldNotIncludeDocumentsOnTheOtherSide(DocumentSide side)
    {
        var otherSide = side switch
        {
            DocumentSide.Left => DocumentSide.Right,
            DocumentSide.Right => DocumentSide.Left,
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
        };
        
        var document = CreateMockDocument();
        document.Side.Returns(otherSide);

        _documentService.Add(document);

        var subject = new DocumentDockViewModel(_documentService, DocumentViewModelFactory, side);
        
        subject.Activator
            .Activate()
            .DisposeWith(_disposables);

        subject.Documents
            .Should()
            .BeEmpty();
    }

    [Theory]
    [InlineData(DocumentSide.Left)]
    [InlineData(DocumentSide.Right)]
    public void ActiveDocument_ShouldReturnNull_WhenNoActiveDocumentExistsOnTheMatchingSide(DocumentSide side)
    {
        var document = CreateMockDocument();
        document.Side.Returns(side);
        
        var subject = new DocumentDockViewModel(_documentService, DocumentViewModelFactory, side);
        
        subject.Activator
            .Activate()
            .DisposeWith(_disposables);
        
        _documentService.Add(document);
        
        _documentService.ActiveDocuments[side].Value = document;
        _documentService.ActiveDocuments[side].Value = null;

        subject.ActiveDocument
            .Should()
            .BeNull();
    }
    
    [Theory]
    [InlineData(DocumentSide.Left)]
    [InlineData(DocumentSide.Right)]
    public async Task ActiveDocument_ShouldUpdateActiveDocument_WhenDocumentServiceRequestsActivation(
        DocumentSide side)
    {
        var leftDocument = CreateMockDocument();
        _documentService.Add(leftDocument);
        
        var document = CreateMockDocument();
        document.Side.Returns(side);

        _documentService.Add(document);

        var subject = new DocumentDockViewModel(_documentService, DocumentViewModelFactory, side);
        
        subject.Activator
            .Activate()
            .DisposeWith(_disposables);

        _documentService.Activate(document);

        var attempts = 0;
        while (attempts < 50 && subject.ActiveDocument == null)
        {
            await Task.Delay(100, TestContext.Current.CancellationToken);
            attempts++;
        }

        subject.ActiveDocument
            .Should()
            .NotBeNull()
            .And
            .Subject
            .As<DocumentViewModel>()
            .Document
            .Should()
            .BeSameAs(document);
    }
    
    [Theory]
    [InlineData(DocumentSide.Left)]
    [InlineData(DocumentSide.Right)]
    public async Task ActiveDocument_ShouldUpdateDocumentService_WhenDocumentServiceRequestsActivation(DocumentSide side)
    {
        var document = CreateMockDocument();
        document.Side.Returns(side);

        _documentService.Add(document);

        var subject = new DocumentDockViewModel(_documentService, DocumentViewModelFactory, side);
        
        subject.Activator
            .Activate()
            .DisposeWith(_disposables);

        _documentService.Activate(document);
        
        var attempts = 0;
        while (attempts < 50 && subject.ActiveDocument == null)
        {
            await Task.Delay(100, TestContext.Current.CancellationToken);
            attempts++;
        }
        
        subject.ActiveDocument = null;
        
        attempts = 0;
        while (attempts < 50 && _documentService.ActiveDocuments[side].Value != null)
        {
            await Task.Delay(100, TestContext.Current.CancellationToken);
            attempts++;
        }

        _documentService.ActiveDocuments[side].Value
            .Should()
            .BeNull();
    }
    
    [Theory]
    [InlineData(DocumentSide.Left)]
    [InlineData(DocumentSide.Right)]
    public void ActiveDocument_ShouldUpdateDocumentService_WhenActiveDocumentIsChanged(DocumentSide side)
    {
        var document = CreateMockDocument();
        
        document.Side.Returns(side);

        _documentService.Add(document);

        var subject = new DocumentDockViewModel(_documentService, DocumentViewModelFactory, side);
        
        subject.Activator
            .Activate()
            .DisposeWith(_disposables);

        subject.ActiveDocument = subject.Documents
            .First(x => x.Document == document);

        _documentService.ActiveDocuments[side].Value
            .Should()
            .Be(document);
    }
    
    [Fact]
    public void GetViewType_ShouldReturnExpected()
    {
        var subject = new DocumentDockViewModel(_documentService, DocumentViewModelFactory, DocumentSide.Left);
        
        subject.GetViewType()
            .Should()
            .Be(typeof(IViewFor<DocumentDockViewModel>));
    }
    
    [Fact]
    public void SplatResolve_ShouldResolveToFactory()
    {
        var factory = Locator.Current.GetService<DocumentDockViewModel.Factory>()!;

        var subject1 = factory(DocumentSide.Left);
        var subject2 = factory(DocumentSide.Left);
        
        subject1.Should().BeOfType<DocumentDockViewModel>();
        subject2.Should().BeOfType<DocumentDockViewModel>();
        
        subject1.Should().NotBeSameAs(subject2);
    }
}