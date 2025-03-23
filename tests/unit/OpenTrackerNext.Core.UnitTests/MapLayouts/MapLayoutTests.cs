using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NSubstitute;
using OpenTrackerNext.Core.MapLayouts;
using OpenTrackerNext.Core.Maps;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.MapLayouts;

[ExcludeFromCodeCoverage]
public sealed class MapLayoutTests
{
    [Fact]
    public void Name_ShouldBeSetAtInitialization()
    {
        const string name = "Test";
        
        var subject = new MapLayout
        {
            Name = name,
            Maps = []
        };
        
        subject.Name
            .Should()
            .Be(name);
    }
    
    [Fact]
    public void Maps_ShouldBeSetAtInitialization()
    {
        var maps = new ObservableCollection<IMap>
        {
            Substitute.For<IMap>(),
            Substitute.For<IMap>()
        };
        
        var subject = new MapLayout
        {
            Name = "Test",
            Maps = maps
        };
        
        subject.Maps
            .Should()
            .BeSameAs(maps);
    }
}