using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using OneOf;

namespace OpenTrackerNext.Core.Results;

/// <summary>
/// Represents a result of a yes/no/cancel dialog.
/// </summary>
[ExcludeFromCodeCoverage]
[GenerateOneOf]
public sealed partial class YesNoCancelDialogResult : OneOfBase<Yes, No, Cancel>
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
    /// A <see cref="Cancel"/> representing a result of cancel.
    /// </summary>
    public static readonly Cancel Cancel = default;

    /// <summary>
    /// Gets a value indicating whether the result is <see cref="Yes"/>.
    /// </summary>
    public bool IsYes => IsT0;
    
    /// <summary>
    /// Gets a value indicating whether the result is <see cref="No"/>.
    /// </summary>
    public bool IsNo => IsT1;
    
    /// <summary>
    /// Gets a value indicating whether the result is <see cref="Cancel"/>.
    /// </summary>
    public bool IsCancel => IsT2;

    /// <summary>
    /// Returns a value based on the result type.
    /// </summary>
    /// <param name="yesDelegate">
    /// A <see cref="Func{T, TResult}"/> representing the delegate for the <see cref="Yes"/> value.
    /// </param>
    /// <param name="noDelegate">
    /// A <see cref="Func{T, TResult}"/> representing the delegate for the <see cref="No"/> value.
    /// </param>
    /// <param name="cancelDelegate">
    /// A <see cref="Func{T, TResult}"/> representing the delegate for the <see cref="Cancel"/> value.
    /// </param>
    /// <typeparam name="TResult">
    /// The type of the return value.
    /// </typeparam>
    /// <returns>
    /// A <typeparamref name="TResult"/> representing the return value.
    /// </returns>
    public async Task<TResult> MatchAsync<TResult>(
        Func<Yes, Task<TResult>> yesDelegate,
        Func<No, TResult> noDelegate,
        Func<Cancel, TResult> cancelDelegate)
    {
        if (IsYes)
        {
            return await yesDelegate((Yes)Value);
        }

        return IsNo
            ? noDelegate((No)Value)
            : cancelDelegate((Cancel)Value);
    }

    /// <summary>
    /// Tries to pick the <see cref="Yes"/> value.
    /// If the value is <see cref="Yes"/>, the method returns true and you can use the <see cref="yes"/> out parameter.
    /// If the value is <see cref="No"/> or <see cref="Cancel"/>, the method returns false and you can use the
    /// <see cref="remainder"/> out parameter.
    /// </summary>
    /// <param name="yes">
    /// The <see cref="Yes"/> value.
    /// </param>
    /// <param name="remainder">
    /// The <see cref="OneOf{No,Cancel}"/> remainder.
    /// </param>
    /// <returns>
    /// A <see cref="bool"/> representing whether the value is <see cref="Yes"/>.
    /// </returns>
    public bool TryPickYes(out Yes yes, out OneOf<No, Cancel> remainder)
    {
        return TryPickT0(out yes, out remainder);
    }
}