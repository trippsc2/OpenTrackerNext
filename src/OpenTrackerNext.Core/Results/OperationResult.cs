using System.Diagnostics.CodeAnalysis;
using OneOf;

namespace OpenTrackerNext.Core.Results;

/// <summary>
/// Represents a result of an operation that can produce errors.
/// </summary>
[ExcludeFromCodeCoverage]
[GenerateOneOf]
public sealed partial class OperationResult : OneOfBase<Success, Cancel>
{
    /// <summary>
    /// A <see cref="Success"/> representing a result of success.
    /// </summary>
    public static readonly Success Success = default;
    
    /// <summary>
    /// A <see cref="Cancel"/> representing a result of cancel.
    /// </summary>
    public static readonly Cancel Cancel = default;

    /// <summary>
    /// Gets a value indicating whether the result is <see cref="Success"/>.
    /// </summary>
    public bool IsSuccess => IsT0;
    
    /// <summary>
    /// Gets a value indicating whether the result is <see cref="Cancel"/>.
    /// </summary>
    public bool IsCancel => IsT1;
}