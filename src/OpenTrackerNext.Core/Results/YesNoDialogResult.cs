using System.Diagnostics.CodeAnalysis;
using OneOf;

namespace OpenTrackerNext.Core.Results;

/// <summary>
/// Represents the result of a yes/no dialog box.
/// </summary>
[ExcludeFromCodeCoverage]
[GenerateOneOf]
public sealed partial class YesNoDialogResult : OneOfBase<Yes, No>
{
    /// <summary>
    /// A <see cref="Yes"/> representing a result of yes.
    /// </summary>
    public static readonly Yes Yes = default;
    
    /// <summary>
    /// A <see cref="No"/> representing a result of no.
    /// </summary>
    public static readonly No No = default;

    /// <summary>
    /// Gets a value indicating whether the result is <see cref="Yes"/>.
    /// </summary>
    public bool IsYes => IsT0;
    
    /// <summary>
    /// Gets a value indicating whether the result is <see cref="No"/>.
    /// </summary>
    public bool IsNo => IsT1;
}