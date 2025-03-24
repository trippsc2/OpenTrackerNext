using System.ComponentModel.DataAnnotations;
using OpenTrackerNext.Core.Documents;

namespace OpenTrackerNext.Editor.Validation;

/// <summary>
/// Represents the validation attribute for an <see cref="IDocumentFile"/>.
/// </summary>
public sealed class DocumentFileValidationAttribute : ValidationAttribute
{
    /// <inheritdoc/>
    public override bool IsValid(object? value)
    {
        return value is not (null or NullDocumentFile);
    }
}