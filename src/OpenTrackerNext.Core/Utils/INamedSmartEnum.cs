using Ardalis.SmartEnum;

namespace OpenTrackerNext.Core.Utils;

/// <summary>
/// Represents a <see cref="SmartEnum{TEnum}"/> that has a display name.
/// </summary>
public interface INamedSmartEnum : ISmartEnum
{
    /// <summary>
    /// Gets a <see cref="string"/> representing the display name of the smart enum value.
    /// </summary>
    string DisplayName { get; }
}