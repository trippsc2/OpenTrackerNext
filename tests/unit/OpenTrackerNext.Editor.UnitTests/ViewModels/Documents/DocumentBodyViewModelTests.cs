using System;
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
public sealed class DocumentBodyViewModelTests
{
    private sealed class MockViewModel : ViewModel;
    
    private readonly CompositeDisposable _disposables = new();
    private readonly IFactory<IDocument, ViewModel> _documentViewModelFactory =
        Substitute.For<IFactory<IDocument, ViewModel>>();
    private readonly IDocument _document = Substitute.For<IDocument>();
    private readonly MockViewModel _content = new();
    
    private readonly DocumentBodyViewModel _subject;

    public DocumentBodyViewModelTests()
    {
        _documentViewModelFactory
            .Create(Arg.Is(_document))
            .Returns(_content);
        
        _subject = new DocumentBodyViewModel(_documentViewModelFactory, _document);
    }

    [Fact]
    public void Content_ShouldReturnExpected()
    {
        _subject.Content
            .Should()
            .Be(_content);
    }
    
    [Fact]
    public void RevertCommand_ShouldCallRevertOnDocument()
    {
        _subject.Activator
            .Activate()
            .DisposeWith(_disposables);
        
        _subject.RevertCommand
            .Execute()
            .Subscribe()
            .DisposeWith(_disposables);
        
        _document.Received(1).Revert();
    }
    
    [Fact]
    public void SaveCommand_ShouldCallSaveOnDocument()
    {
        _subject.Activator
            .Activate()
            .DisposeWith(_disposables);
        
        _subject.SaveCommand
            .Execute()
            .Subscribe()
            .DisposeWith(_disposables);
        
        _document.Received(1).SaveAsync();
    }

    [Fact]
    public void GetViewType_ShouldReturnExpected()
    {
        _subject.GetViewType()
            .Should()
            .Be(typeof(IViewFor<DocumentBodyViewModel>));
    }
    
    [Fact]
    public void SplatResolve_ShouldResolveToFactory()
    {
        Locator.CurrentMutable.Register(() => _documentViewModelFactory, typeof(IFactory<IDocument, ViewModel>));
        
        var factory = Locator.Current.GetService<DocumentBodyViewModel.Factory>()!;

        var subject1 = factory(_document);
        var subject2 = factory(_document);

        subject1.Should().BeOfType<DocumentBodyViewModel>();
        subject2.Should().BeOfType<DocumentBodyViewModel>();

        subject1.Should().NotBeSameAs(subject2);
    }
}