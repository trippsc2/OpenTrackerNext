using System;
using System.Diagnostics.CodeAnalysis;
using OpenTrackerNext.Core.Generated;
using OpenTrackerNext.Editor.Generated;
using OpenTrackerNext.Editor.UnitTests;
using ReactiveUI;
using Splat;
using Xunit;

[assembly: AssemblyFixture(typeof(SplatFixture))]

namespace OpenTrackerNext.Editor.UnitTests;

[ExcludeFromCodeCoverage]
public sealed class SplatFixture : IDisposable
{
    private readonly IDependencyResolver _previousResolver;
    
    public SplatFixture()
    {
        var resolver = new ModernDependencyResolver();

        resolver.RegisterConstant(Locator.Current.GetService<IActivationForViewFetcher>());
        resolver.RegisterConstant(Locator.Current.GetService<IPropertyBindingHook>());
        
        resolver
            .RegisterTypesFromOpenTrackerNextCore()
            .RegisterTypesFromOpenTrackerNextEditor();

        _previousResolver = Locator.GetLocator();

        Locator.SetLocator(resolver);
    }
    
    public void Dispose()
    {
        Locator.SetLocator(_previousResolver);
    }
}