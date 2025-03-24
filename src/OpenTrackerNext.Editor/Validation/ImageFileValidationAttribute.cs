using System;
using System.ComponentModel.DataAnnotations;
using OpenTrackerNext.Core.Images;

namespace OpenTrackerNext.Editor.Validation;

/// <summary>
/// Represents the validation attribute for an <see cref="IImageFile"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ImageFileValidationAttribute : ValidationAttribute
{
    /// <inheritdoc/>
    public override bool IsValid(object? value)
    {
        return value is not (null or NullImageFile);
    }
}