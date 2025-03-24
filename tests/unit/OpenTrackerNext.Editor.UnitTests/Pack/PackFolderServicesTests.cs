using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using OpenTrackerNext.Core.Utils;
using OpenTrackerNext.Editor.Entities;
using OpenTrackerNext.Editor.Icons;
using OpenTrackerNext.Editor.Layouts;
using OpenTrackerNext.Editor.MapLayouts;
using OpenTrackerNext.Editor.Maps;
using OpenTrackerNext.Editor.Pack;
using OpenTrackerNext.Editor.UIPanes;
using Splat;
using Xunit;

namespace OpenTrackerNext.Editor.UnitTests.Pack;

[ExcludeFromCodeCoverage]
public sealed class PackFolderServicesTests
{
    private readonly PackFolderServices _subject;

    // ReSharper disable once ConvertConstructorToMemberInitializers
    public PackFolderServicesTests()
    {
        _subject = new PackFolderServices();
    }
    
    [Fact]
    public void All_ShouldContainExpectedServices()
    {
        _subject.All
            .Should()
            .ContainItemsAssignableTo<EntityService>()
            .And.ContainItemsAssignableTo<IconService>()
            .And.ContainItemsAssignableTo<LayoutService>()
            .And.ContainItemsAssignableTo<MapLayoutService>()
            .And.ContainItemsAssignableTo<MapService>()
            .And.ContainItemsAssignableTo<UIPaneService>();
    }
    
    [Fact]
    public void SplatResolve_ShouldResolveToSingleInstance()
    {
        var subject1 = Locator.Current.GetService<IServiceCollection<IPackFolderService>>()!;
        var subject2 = Locator.Current.GetService<IServiceCollection<IPackFolderService>>()!;

        subject1.Should().BeOfType<PackFolderServices>();
        subject2.Should().BeOfType<PackFolderServices>();
        
        subject1.Should().BeSameAs(subject2);
    }
    
    [Fact]
    public void SplatResolve_ShouldResolveToLazySingleInstance()
    {
        var lazy = Locator.Current.GetService<Lazy<IServiceCollection<IPackFolderService>>>()!;
        var subject = Locator.Current.GetService<IServiceCollection<IPackFolderService>>()!;
        
        lazy.Value
            .Should()
            .BeOfType<PackFolderServices>();
        
        subject.Should().BeOfType<PackFolderServices>();

        lazy.Value
            .Should()
            .BeSameAs(subject);
    }
}