using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using FluentAssertions;
using NSubstitute;
using OpenTrackerNext.Core.Entities;
using OpenTrackerNext.Core.Requirements.Entity.AtLeast;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Requirements.Entity.AtLeast;

[ExcludeFromCodeCoverage]
public sealed class EntityAtLeastRequirementTests : IDisposable
{
    private readonly CompositeDisposable _disposables = new();
    private readonly IEntity _entity = Substitute.For<IEntity>();

    public void Dispose()
    {
        _disposables.Dispose();
    }
    
    [Theory]
    [InlineData(false, 0, 1)]
    [InlineData(true, 1, 1)]
    [InlineData(true, 2, 1)]
    [InlineData(false, 0, 2)]
    [InlineData(false, 1, 2)]
    [InlineData(true, 2, 2)]
    public void IsMet_ShouldReturnExpected(bool expected, int current, int requiredValue)
    {
        var subject = new EntityAtLeastRequirement(_entity, requiredValue);
        
        subject.DisposeWith(_disposables);
        
        _entity.Current.Returns(current);
        _entity.PropertyChanged += Raise
            .Event<PropertyChangedEventHandler>(
                _entity,
                new PropertyChangedEventArgs(nameof(IEntity.Current)));

        subject.IsMet
            .Should()
            .Be(expected);
    }
    
    [Fact]
    public void IsMet_ShouldRaisePropertyChanged()
    {
        var subject = new EntityAtLeastRequirement(_entity, 1);
        
        subject.DisposeWith(_disposables);
        
        using var monitor = subject.Monitor();
        
        _entity.Current.Returns(1);
        _entity.PropertyChanged += Raise
            .Event<PropertyChangedEventHandler>(
                _entity,
                new PropertyChangedEventArgs(nameof(IEntity.Current)));
        
        monitor.Should().RaisePropertyChangeFor(x => x.IsMet);
    }
}