using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using FluentAssertions;
using NSubstitute;
using OpenTrackerNext.Core.Requirements;
using OpenTrackerNext.Core.Requirements.Alternative;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Requirements.Alternative;

[ExcludeFromCodeCoverage]
public sealed class AlternativeRequirementTests : IDisposable
{
    private readonly CompositeDisposable _disposables = new();
    
    private readonly IRequirement _requirement1 = Substitute.For<IRequirement>();
    private readonly IRequirement _requirement2 = Substitute.For<IRequirement>();
    
    private readonly AlternativeRequirement _subject;

    public AlternativeRequirementTests()
    {
        var requirements = new ObservableCollection<IRequirement> { _requirement1, _requirement2 };
        
        _subject = new AlternativeRequirement(requirements);
        
        _subject.DisposeWith(_disposables);
    }
    
    public void Dispose()
    {
        _disposables.Dispose();
    }
    
    [Theory]
    [InlineData(false, false, false)]
    [InlineData(true, true, false)]
    [InlineData(true, false, true)]
    [InlineData(true, true, true)]
    public void IsMet_ShouldReturnExpected(bool expected, bool requirement1, bool requirement2)
    {
        _requirement1.IsMet.Returns(requirement1);
        _requirement2.IsMet.Returns(requirement2);
        
        _requirement1.PropertyChanged += Raise
            .Event<PropertyChangedEventHandler>(
                _requirement1,
                new PropertyChangedEventArgs(nameof(IRequirement.IsMet)));

        _subject.IsMet
            .Should()
            .Be(expected);
    }
    
    [Fact]
    public void IsMet_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();
        
        _requirement1.IsMet.Returns(true);
        _requirement2.IsMet.Returns(true);
        
        _requirement1.PropertyChanged += Raise
            .Event<PropertyChangedEventHandler>(
                _requirement1,
                new PropertyChangedEventArgs(nameof(IRequirement.IsMet)));

        _requirement2.PropertyChanged += Raise
            .Event<PropertyChangedEventHandler>(
                _requirement2,
                new PropertyChangedEventArgs(nameof(IRequirement.IsMet)));

        monitor.Should().RaisePropertyChangeFor(x => x.IsMet);
    }
}