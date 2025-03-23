using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using OpenTrackerNext.Core.Factories;
using OpenTrackerNext.Core.ViewModels;
using OpenTrackerNext.Editor.Documents;
using OpenTrackerNext.Editor.ViewModels.Documents;
using Splat;
using Xunit;

namespace OpenTrackerNext.Editor.UnitTests.ViewModels.Documents;

[ExcludeFromCodeCoverage]
public sealed class DocumentViewModelFactoryTests
{
    [Fact]
    public void SplatResolve_ShouldResolveToSingleInstance()
    {
        var subject1 = Locator.Current.GetService<IFactory<IDocument, ViewModel>>();
        var subject2 = Locator.Current.GetService<IFactory<IDocument, ViewModel>>();

        subject1.Should().BeOfType<DocumentViewModelFactory>();
        subject2.Should().BeOfType<DocumentViewModelFactory>();
        
        subject1.Should().BeSameAs(subject2);
    }
    
    [Fact]
    public void SplatResolve_ShouldResolveToLazySingleInstance()
    {
        var lazy = Locator.Current.GetService<Lazy<IFactory<IDocument, ViewModel>>>()!;
        var subject = Locator.Current.GetService<IFactory<IDocument, ViewModel>>()!;

        lazy.Value
            .Should()
            .BeOfType<DocumentViewModelFactory>();
        
        subject.Should().BeOfType<DocumentViewModelFactory>();
        
        lazy.Value
            .Should()
            .BeSameAs(subject);
    }
}