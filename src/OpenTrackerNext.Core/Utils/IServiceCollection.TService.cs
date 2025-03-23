using System.Collections.Generic;

namespace OpenTrackerNext.Core.Utils;

/// <summary>
/// Represents a collection of services.
/// </summary>
/// <typeparam name="TService">
/// The type of the services in the collection.
/// </typeparam>
public interface IServiceCollection<out TService>
    where TService : class
{
    /// <summary>
    /// Gets a <see cref="IReadOnlyList{T}"/> of <typeparamref name="TService"/> representing all services in the collection.
    /// </summary>
    public IReadOnlyList<TService> All { get; }
}