using System;
using System.ComponentModel;

namespace OpenTrackerNext.Core.Requirements;

/// <summary>
/// Represents a requirement.
/// </summary>
public interface IRequirement : IDisposable, INotifyPropertyChanged
{
    /// <summary>
    /// Gets a value indicating whether the requirement has been met.
    /// </summary>
    bool IsMet { get; }
}