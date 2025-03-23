using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
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
public sealed class DocumentLayoutViewModelTests
{
    private sealed class MockViewModel : ViewModel;
    
    private readonly CompositeDisposable _disposables = new();
    private readonly DocumentService _documentService = new();
    private readonly IFactory<IDocument, ViewModel> _documentViewModelFactory;
    
    private readonly Dictionary<DocumentSide, DocumentDockViewModel> _documentDocks;
    
    private readonly DocumentLayoutViewModel _subject;

    public DocumentLayoutViewModelTests()
    {
        _documentViewModelFactory = Substitute.For<IFactory<IDocument, ViewModel>>();
        
        _documentDocks = new Dictionary<DocumentSide, DocumentDockViewModel>
        {
            { 
                DocumentSide.Left,
                new DocumentDockViewModel(_documentService, DocumentViewModelFactory, DocumentSide.Left)
            },
            {
                DocumentSide.Right,
                new DocumentDockViewModel(_documentService, DocumentViewModelFactory, DocumentSide.Right)
            }
        };

        _documentViewModelFactory
            .Create(Arg.Any<IDocument>())
            .Returns(_ => new MockViewModel());

        _subject = new DocumentLayoutViewModel(_documentService, DocumentDockFactory);

        _subject.Activator
            .Activate()
            .DisposeWith(_disposables);
    }
    
    private DocumentDockViewModel DocumentDockFactory(DocumentSide side)
    {
        return _documentDocks[side];
    }

    private DocumentBodyViewModel DocumentBodyFactory(IDocument document)
    {
        return new DocumentBodyViewModel(_documentViewModelFactory, document);
    }
    
    private DocumentViewModel DocumentViewModelFactory(IDocument document)
    {
        return new DocumentViewModel(_documentService, DocumentBodyFactory, document);
    }

    [Fact]
    public void LeftDock_ShouldReturnExpected()
    {
        _subject.LeftDock
            .Should()
            .Be(_documentDocks[DocumentSide.Left]);
    }
    
    [Fact]
    public void RightDock_ShouldReturnExpected()
    {
        _subject.RightDock
            .Should()
            .Be(_documentDocks[DocumentSide.Right]);
    }
    
    [Fact]
    public void RightDockIsVisible_ShouldReturnFalse_WhenRightDockHasNoDocuments()
    {
        _subject.RightDockIsVisible
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void RightDockIsVisible_ShouldReturnTrue_WhenRightDockHasDocuments()
    {
        var leftDocument = Substitute.For<IDocument>();
        leftDocument.Side.Returns(DocumentSide.Left);
        _documentService.Add(leftDocument);
        
        var rightDocument = Substitute.For<IDocument>();
        rightDocument.Side.Returns(DocumentSide.Right);
        _documentService.Add(rightDocument);

        _subject.RightDockIsVisible
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void LeftDockColumnSpan_ShouldReturnExpected_WhenRightDockHasNoDocuments()
    {
        _subject.LeftDockColumnSpan
            .Should()
            .Be(3);
    }
    
    [Fact]
    public void LeftDocumentDockColumnSpan_ShouldReturnExpected_WhenRightDocumentDockHasDocuments()
    {
        var leftDocument = Substitute.For<IDocument>();
        leftDocument.Side.Returns(DocumentSide.Left);
        _documentService.Add(leftDocument);
        
        var rightDocument = Substitute.For<IDocument>();
        rightDocument.Side.Returns(DocumentSide.Right);
        _documentService.Add(rightDocument);

        _subject.LeftDockColumnSpan
            .Should()
            .Be(1);
    }

    [Fact]
    public void GetViewType_ShouldReturnExpected()
    {
        _subject.GetViewType()
            .Should()
            .Be(typeof(IViewFor<DocumentLayoutViewModel>));
    }
    
    [Fact]
    public void SplatResolve_ShouldResolveToTransientInstance()
    {
        var subject1 = Locator.Current.GetService<DocumentLayoutViewModel>()!;
        var subject2 = Locator.Current.GetService<DocumentLayoutViewModel>()!;

        subject1.Should().NotBeSameAs(subject2);
    }
}