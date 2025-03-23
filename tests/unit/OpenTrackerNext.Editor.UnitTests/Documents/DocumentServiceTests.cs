using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using DynamicData;
using FluentAssertions;
using FluentAssertions.Reactive;
using NSubstitute;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Results;
using OpenTrackerNext.Editor.Documents;
using Splat;
using Xunit;

namespace OpenTrackerNext.Editor.UnitTests.Documents;

[ExcludeFromCodeCoverage]
public sealed class DocumentServiceTests
{
    private readonly CompositeDisposable _disposables = new();
    private readonly ReadOnlyObservableCollection<IDocument> _documents;
    private readonly DocumentService _subject = new();

    public DocumentServiceTests()
    {
        _subject.Connect()
            .Bind(out _documents)
            .Subscribe()
            .DisposeWith(_disposables);

        _subject.DisposeWith(_disposables);
    }

    private static IDocumentFile<MockDocumentData> CreateMockDocumentFile()
    {
        var file = Substitute.For<IDocumentFile<MockDocumentData>>();
        file.SavedData.Returns(new MockDocumentData());
        file.WorkingData.Returns(new MockDocumentData());

        return file;
    }

    [Fact]
    public void RightHasDocuments_ShouldReturnFalse_WhenNoDocuments()
    {
        _subject.RightHasDocuments
            .Should()
            .BeFalse();
    }

    [Fact]
    public void RightHasDocuments_ShouldReturnFalse_WhenOnlyLeftSideDocuments()
    {
        var document = Substitute.For<IDocument<MockDocumentData>>();
        _subject.Add(document);

        _subject.RightHasDocuments
            .Should()
            .BeFalse();
    }

    [Fact]
    public void RightHasDocuments_ShouldReturnTrue_WhenOnlyRightSideDocuments()
    {
        var leftDocument = Substitute.For<IDocument<MockDocumentData>>();
        var rightDocument = Substitute.For<IDocument<MockDocumentData>>();

        rightDocument.Side.Returns(DocumentSide.Right);

        _subject.Add(leftDocument);
        _subject.Add(rightDocument);

        _subject.RightHasDocuments
            .Should()
            .BeTrue();
    }

    [Fact]
    public void ActiveDocuments_ShouldDefaultToNullOnLeftAndRight()
    {
        _subject.ActiveDocuments
            .Should()
            .ContainKey(DocumentSide.Left)
            .And
            .ContainKey(DocumentSide.Right);

        _subject.ActiveDocuments[DocumentSide.Left]
            .Value
            .Should()
            .BeNull();

        _subject.ActiveDocuments[DocumentSide.Right]
            .Value
            .Should()
            .BeNull();
    }

    [Fact]
    public void ActivationRequests_ShouldContainLeftAndRightObservables()
    {
        _subject.ActivationRequests
            .Should()
            .ContainKey(DocumentSide.Left)
            .And
            .ContainKey(DocumentSide.Right);
    }

    [Fact]
    public void Open_ShouldAddDocumentAndActivate_WhenDocumentDoesNotExist()
    {
        var file = CreateMockDocumentFile();

        using var observer = _subject.ActivationRequests[DocumentSide.Left].Observe();

        _subject.Open(file);

        _documents
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .BeOfType<Document<MockDocumentData>>()
            .Which
            .File
            .Should()
            .Be(file);

        observer.Should().Push(1);
    }

    [Fact]
    public void Open_ShouldActivateDocument_WhenDocumentExists()
    {
        var file = CreateMockDocumentFile();
        var document = new Document<MockDocumentData>(file);

        _subject.Add(document);

        using var observer = _subject.ActivationRequests[DocumentSide.Left].Observe();

        _subject.Open(file);

        _documents
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .Be(document);

        observer.Should().Push(1);
    }

    [Theory]
    [InlineData(DocumentSide.Right, DocumentSide.Left)]
    [InlineData(DocumentSide.Left, DocumentSide.Right)]
    public void MoveToOtherSide_ShouldMoveDocumentToOtherSide(DocumentSide expected, DocumentSide originalSide)
    {
        var leftDocument = Substitute.For<IDocument>();

        _subject.Add(leftDocument);

        var file = CreateMockDocumentFile();
        var document = new Document<MockDocumentData>(file);

        document.Side = originalSide;

        _subject.Add(document);

        using var observer = _subject.ActivationRequests[expected].Observe();

        _subject.MoveToOtherSide(document);

        document.Side
            .Should()
            .Be(expected);

        observer.Should().Push(1);
    }

    [Fact]
    public async Task TryHandleUnsavedChangesAsync_ShouldReturnSuccess_WhenDocumentIsNotOpen()
    {
        var document = Substitute.For<IDocument<MockDocumentData>>();
        document.IsUnsaved.Returns(false);

        var result = await _subject.TryHandleUnsavedChangesAsync(document);

        result.Value
            .Should()
            .BeOfType<Success>();
    }

    [Fact]
    public async Task TryHandleUnsavedChangesAsync_ShouldReturnSuccess_WhenDocumentIsSaved()
    {
        var document = Substitute.For<IDocument<MockDocumentData>>();
        document.IsUnsaved.Returns(false);

        _subject.Add(document);

        var result = await _subject.TryHandleUnsavedChangesAsync(document);

        result.Value
            .Should()
            .BeOfType<Success>();
    }

    [Fact]
    public async Task TryHandleUnsavedChangesAsync_ShouldReturnSuccess_WhenUnsavedDocumentIsSaved()
    {
        var document = Substitute.For<IDocument<MockDocumentData>>();
        document.IsUnsaved.Returns(true);

        _subject.Add(document);

        _subject.RequestSaveDialog
            .RegisterHandler(context => context.SetOutput(YesNoCancelDialogResult.Yes))
            .DisposeWith(_disposables);

        using var observer = _subject.ActivationRequests[DocumentSide.Left].Observe();

        var result = await _subject.TryHandleUnsavedChangesAsync(document);

        result.Value
            .Should()
            .BeOfType<Success>();

        await document.Received(1).SaveAsync();

        await observer.Should().PushAsync(1);
    }

    [Fact]
    public async Task TryHandleUnsavedChangesAsync_ShouldReturnSuccess_WhenUnsavedDocumentIsNotSaved()
    {
        var document = Substitute.For<IDocument<MockDocumentData>>();
        document.IsUnsaved.Returns(true);

        _subject.Add(document);

        _subject.RequestSaveDialog
            .RegisterHandler(context => context.SetOutput(YesNoCancelDialogResult.No))
            .DisposeWith(_disposables);

        using var observer = _subject.ActivationRequests[DocumentSide.Left].Observe();

        var result = await _subject.TryHandleUnsavedChangesAsync(document);

        result.Value
            .Should()
            .BeOfType<Success>();

        await observer.Should().PushAsync(1);
    }

    [Fact]
    public async Task TryHandleUnsavedChangesAsync_ShouldReturnCancel_WhenUnsavedDocumentFailsToSave()
    {
        var document = Substitute.For<IDocument<MockDocumentData>>();
        document.IsUnsaved.Returns(true);
        document.SaveAsync().Returns(_ => throw new Exception());

        _subject.Add(document);

        _subject.RequestSaveDialog
            .RegisterHandler(context => context.SetOutput(YesNoCancelDialogResult.Yes))
            .DisposeWith(_disposables);
        
        var exceptionDialogShown = false;

        _subject.ExceptionDialog
            .RegisterHandler(
                context =>
                {
                    context.SetOutput(Unit.Default);
                    exceptionDialogShown = true;
                })
            .DisposeWith(_disposables);

        using var observer = _subject.ActivationRequests[DocumentSide.Left].Observe();

        var result = await _subject.TryHandleUnsavedChangesAsync(document);

        result.Value
            .Should()
            .BeOfType<Cancel>();

        exceptionDialogShown.Should().BeTrue();

        await observer.Should().PushAsync(1);
    }

    [Fact]
    public async Task TryHandleUnsavedChangesAsync_ShouldReturnCancel_WhenUnsavedDocumentIsCanceled()
    {
        var document = Substitute.For<IDocument<MockDocumentData>>();
        document.IsUnsaved.Returns(true);

        _subject.Add(document);

        _subject.RequestSaveDialog
            .RegisterHandler(context => context.SetOutput(YesNoCancelDialogResult.Cancel))
            .DisposeWith(_disposables);

        using var observer = _subject.ActivationRequests[DocumentSide.Left].Observe();

        var result = await _subject.TryHandleUnsavedChangesAsync(document);

        result.Value
            .Should()
            .BeOfType<Cancel>();

        await observer.Should().PushAsync(1);
    }

    [Fact]
    public async Task TryHandleAllUnsavedChangesAsync_ShouldReturnSuccess_WhenNoDocuments()
    {
        var result = await _subject.TryHandleAllUnsavedChangesAsync();

        result.Value
            .Should()
            .BeOfType<Success>();
    }

    [Fact]
    public async Task TryHandleAllUnsavedChangesAsync_ShouldReturnSuccess_WhenAllDocumentsAreSaved()
    {
        var document = Substitute.For<IDocument<MockDocumentData>>();
        document.IsUnsaved.Returns(false);

        _subject.Add(document);

        var result = await _subject.TryHandleAllUnsavedChangesAsync();

        result.Value
            .Should()
            .BeOfType<Success>();
    }

    [Fact]
    public async Task TryHandleAllUnsavedChangesAsync_ShouldReturnSuccess_WhenUnsavedDocumentsAreSaved()
    {
        var document = Substitute.For<IDocument<MockDocumentData>>();
        document.IsUnsaved.Returns(true);

        _subject.Add(document);

        _subject.RequestSaveDialog
            .RegisterHandler(context => context.SetOutput(YesNoCancelDialogResult.Yes))
            .DisposeWith(_disposables);

        using var observer = _subject.ActivationRequests[DocumentSide.Left].Observe();

        var result = await _subject.TryHandleAllUnsavedChangesAsync();

        result.Value
            .Should()
            .BeOfType<Success>();
        
        await document.Received(1).SaveAsync();

        await observer.Should().PushAsync(1);
    }

    [Fact]
    public async Task TryHandleAllUnsavedChangesAsync_ShouldReturnSuccess_WhenUnsavedDocumentsAreNotSaved()
    {
        var document = Substitute.For<IDocument<MockDocumentData>>();
        document.IsUnsaved.Returns(true);

        _subject.Add(document);

        _subject.RequestSaveDialog
            .RegisterHandler(context => context.SetOutput(YesNoCancelDialogResult.No))
            .DisposeWith(_disposables);

        using var observer = _subject.ActivationRequests[DocumentSide.Left].Observe();

        var result = await _subject.TryHandleAllUnsavedChangesAsync();

        result.Value
            .Should()
            .BeOfType<Success>();

        await observer.Should().PushAsync(1);
    }

    [Fact]
    public async Task TryHandleAllUnsavedChangesAsync_ShouldReturnCancel_WhenUnsavedDocumentFailsToSave()
    {
        var document = Substitute.For<IDocument<MockDocumentData>>();
        document.IsUnsaved.Returns(true);
        document.SaveAsync().Returns(_ => throw new Exception());

        _subject.Add(document);

        _subject.RequestSaveDialog
            .RegisterHandler(context => context.SetOutput(YesNoCancelDialogResult.Yes))
            .DisposeWith(_disposables);
        
        var exceptionDialogShown = false;

        _subject.ExceptionDialog
            .RegisterHandler(
                context =>
                {
                    context.SetOutput(Unit.Default);
                    exceptionDialogShown = true;
                })
            .DisposeWith(_disposables);

        using var observer = _subject.ActivationRequests[DocumentSide.Left].Observe();

        var result = await _subject.TryHandleAllUnsavedChangesAsync();

        result.Value
            .Should()
            .BeOfType<Cancel>();

        exceptionDialogShown.Should().BeTrue();

        await observer.Should().PushAsync(1);
    }

    [Fact]
    public async Task TryHandleAllUnsavedChangesAsync_ShouldReturnCancel_WhenUnsavedDocumentsAreCanceled()
    {
        var document = Substitute.For<IDocument<MockDocumentData>>();
        document.IsUnsaved.Returns(true);

        _subject.Add(document);

        _subject.RequestSaveDialog
            .RegisterHandler(context => context.SetOutput(YesNoCancelDialogResult.Cancel))
            .DisposeWith(_disposables);

        using var observer = _subject.ActivationRequests[DocumentSide.Left].Observe();

        var result = await _subject.TryHandleAllUnsavedChangesAsync();

        result.Value
            .Should()
            .BeOfType<Cancel>();

        await observer.Should().PushAsync(1);
    }

    [Fact]
    public async Task TryCloseAsync_ShouldDoNothing_WhenDocumentIsNotOpen()
    {
        var document = Substitute.For<IDocument<MockDocumentData>>();
        document.IsUnsaved.Returns(false);

        await _subject.TryCloseAsync(document);
    }

    [Fact]
    public async Task TryCloseAsync_ShouldCloseDocument_WhenDocumentIsSaved()
    {
        var document = Substitute.For<IDocument<MockDocumentData>>();
        document.IsUnsaved.Returns(false);

        _subject.Add(document);

        await _subject.TryCloseAsync(document);

        _documents
            .Should()
            .NotContain(document);
    }

    [Fact]
    public async Task TryCloseAsync_ShouldCloseDocument_WhenUnsavedDocumentIsSaved()
    {
        var document = Substitute.For<IDocument<MockDocumentData>>();
        document.IsUnsaved.Returns(true);

        _subject.Add(document);

        _subject.RequestSaveDialog
            .RegisterHandler(context => context.SetOutput(YesNoCancelDialogResult.Yes))
            .DisposeWith(_disposables);

        using var observer = _subject.ActivationRequests[DocumentSide.Left].Observe();

        await _subject.TryCloseAsync(document);

        _documents.Should().NotContain(document);

        await document.Received(1).SaveAsync();

        await observer.Should().PushAsync(1);
    }

    [Fact]
    public async Task TryCloseAsync_ShouldCloseDocument_WhenUnsavedDocumentIsNotSaved()
    {
        var document = Substitute.For<IDocument<MockDocumentData>>();
        document.IsUnsaved.Returns(true);

        _subject.Add(document);

        _subject.RequestSaveDialog
            .RegisterHandler(context => context.SetOutput(YesNoCancelDialogResult.No))
            .DisposeWith(_disposables);

        using var observer = _subject.ActivationRequests[DocumentSide.Left].Observe();

        await _subject.TryCloseAsync(document);

        _documents.Should().NotContain(document);

        await observer.Should().PushAsync(1);
    }

    [Fact]
    public async Task TryCloseAsync_ShouldNotCloseDocument_WhenUnsavedDocumentFailsToSave()
    {
        var document = Substitute.For<IDocument<MockDocumentData>>();
        document.IsUnsaved.Returns(true);
        document.SaveAsync().Returns(_ => throw new Exception());

        _subject.Add(document);

        _subject.RequestSaveDialog
            .RegisterHandler(context => context.SetOutput(YesNoCancelDialogResult.Yes))
            .DisposeWith(_disposables);
        
        var exceptionDialogShown = false;

        _subject.ExceptionDialog
            .RegisterHandler(
                context =>
                {
                    context.SetOutput(Unit.Default);
                    exceptionDialogShown = true;
                })
            .DisposeWith(_disposables);

        using var observer = _subject.ActivationRequests[DocumentSide.Left].Observe();

        await _subject.TryCloseAsync(document);

        exceptionDialogShown.Should().BeTrue();
        
        _documents.Should().Contain(document);

        await observer.Should().PushAsync(1);
    }

    [Fact]
    public async Task TryCloseAsync_ShouldNotCloseDocument_WhenUnsavedDocumentIsCanceled()
    {
        var document = Substitute.For<IDocument<MockDocumentData>>();
        document.IsUnsaved.Returns(true);

        _subject.Add(document);

        _subject.RequestSaveDialog
            .RegisterHandler(context => context.SetOutput(YesNoCancelDialogResult.Cancel))
            .DisposeWith(_disposables);

        using var observer = _subject.ActivationRequests[DocumentSide.Left].Observe();

        await _subject.TryCloseAsync(document);

        _documents.Should().Contain(document);

        await observer.Should().PushAsync(1);
    }

    [Fact]
    public void Close_ShouldCloseDocumentForFile_WhenDocumentIsOpen()
    {
        var file = CreateMockDocumentFile();
        var document = new Document<MockDocumentData>(file);

        _subject.Add(document);
        _subject.ActiveDocuments[DocumentSide.Left].Value = document;

        _subject.Close(file);

        _documents.Should().NotContain(document);
        _subject.ActiveDocuments[DocumentSide.Left]
            .Value
            .Should()
            .BeNull();
    }

    [Fact]
    public void Close_ShouldDoNothing_WhenFileHasNoOpenDocuments()
    {
        var file = CreateMockDocumentFile();
        var document = new Document<MockDocumentData>(file);

        _subject.Close(file);

        _documents.Should().NotContain(document);
    }

    [Fact]
    public void Close_ShouldDisposeOfFileDocument()
    {
        var file = CreateMockDocumentFile();
        var document = Substitute.For<IDocument<MockDocumentData>>();
        document.File.Returns(file);
        
        _subject.Add(document);
        
        _subject.Close(file);
        
        document.Received(1).Dispose();
    }

    [Fact]
    public void Close_ShouldCloseDocument_WhenDocumentIsOpen()
    {
        var file = CreateMockDocumentFile();
        var document = new Document<MockDocumentData>(file);

        _subject.Add(document);
        _subject.ActiveDocuments[DocumentSide.Left].Value = document;

        _subject.Close(document);

        _documents.Should().NotContain(document);
        _subject.ActiveDocuments[DocumentSide.Left]
            .Value
            .Should()
            .BeNull();
    }

    [Fact]
    public void Close_ShouldDoNothing_WhenDocumentIsNotOpen()
    {
        var file = CreateMockDocumentFile();
        var document = new Document<MockDocumentData>(file);

        _subject.Close(document);

        _documents.Should().NotContain(document);
    }

    [Fact]
    public void Close_ShouldDisposeDocument()
    {
        var file = CreateMockDocumentFile();
        var document = Substitute.For<IDocument<MockDocumentData>>();
        document.File.Returns(file);
        
        _subject.Close(document);
        
        document.Received(1).Dispose();
    }

    [Fact]
    public void MoveRightDocumentsToLeft_ShouldBeCalled_WhenLeftHasNoDocuments()
    {
        var file1 = CreateMockDocumentFile();
        var file2 = CreateMockDocumentFile();
        
        var document1 = new Document<MockDocumentData>(file1);
        var document2 = new Document<MockDocumentData>(file2);
        
        _subject.Add(document1);
        _subject.Add(document2);
        
        document1.Side = DocumentSide.Right;

        _subject.ActiveDocuments[DocumentSide.Right].Value = document1;

        using var observer = _subject.ActivationRequests[DocumentSide.Left].Observe();
        
        document2.Side = DocumentSide.Right;

        document1.Side
            .Should()
            .Be(DocumentSide.Left);

        document2.Side
            .Should()
            .Be(DocumentSide.Left);
        
        observer.Should().PushMatch(x => x == document1);
    }

    [Fact]
    public void SplatResolve_ShouldResolveToSingleInstance()
    {
        var subject1 = Locator.Current.GetService<IDocumentService>()!;
        var subject2 = Locator.Current.GetService<IDocumentService>()!;

        subject1.Should().BeOfType<DocumentService>();
        subject2.Should().BeOfType<DocumentService>();
        
        subject1.Should().BeSameAs(subject2);
    }
    
    [Fact]
    public void SplatResolve_ShouldResolveToLazySingleInstance()
    {
        var lazy = Locator.Current.GetService<Lazy<IDocumentService>>()!;
        var subject = Locator.Current.GetService<IDocumentService>()!;

        lazy.Value
            .Should()
            .BeOfType<DocumentService>();
        
        subject.Should().BeOfType<DocumentService>();
        
        lazy.Value
            .Should()
            .BeSameAs(subject);
    }
}