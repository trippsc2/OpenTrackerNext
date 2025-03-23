using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using DynamicData;
using OpenTrackerNext.Core.Images;
using OpenTrackerNext.Core.Results;
using OpenTrackerNext.Core.Storage;
using OpenTrackerNext.Core.Validation;
using OpenTrackerNext.Core.ViewModels.Dialogs;
using OpenTrackerNext.Editor.Pack;
using ReactiveUI;

namespace OpenTrackerNext.Editor.Images;

/// <summary>
/// Represents the logic for managing image data.
/// </summary>
public interface IImageService : IPackComponentService
{
    /// <summary>
    /// Gets a <see cref="List{T}"/> of <see cref="ValidationRule{T}"/> of <see cref="string"/> representing the
    /// validation rules for the image file friendly name.
    /// </summary>
    List<ValidationRule<string?>> NameValidationRules { get; }

    /// <summary>
    /// Gets an <see cref="Interaction{TInput,TOutput}"/> with an output of <see cref="IReadOnlyList{T}"/> of
    /// <see cref="IStorageFile"/> representing the add image dialog.
    /// </summary>
    Interaction<Unit, IReadOnlyList<Avalonia.Platform.Storage.IStorageFile>?> AddImagesDialog { get; }
    
    /// <summary>
    /// Gets an <see cref="Interaction{TInput,TOutput}"/> with an input of <see cref="TextBoxDialogViewModel"/> and an
    /// output of <see cref="OperationResult"/> representing the rename dialog.
    /// </summary>
    Interaction<TextBoxDialogViewModel, OperationResult> RenameImageDialog { get; }
    
    /// <summary>
    /// Gets an <see cref="Interaction{TInput,TOutput}"/> with an input of <see cref="YesNoDialogViewModel"/> and an
    /// output of <see cref="YesNoDialogResult"/> representing the delete dialog.
    /// </summary>
    Interaction<YesNoDialogViewModel, YesNoDialogResult> DeleteImageDialog { get; }
    
    /// <summary>
    /// Gets an <see cref="Interaction{TInput,TOutput}"/> with an input of <see cref="ExceptionDialogViewModel"/>
    /// representing the exception dialog.
    /// </summary>
    Interaction<ExceptionDialogViewModel, Unit> ExceptionDialog { get; }

    /// <summary>
    /// Returns an observable change set of the image file collection.
    /// </summary>
    /// <returns>
    /// An <see cref="IObservable{T}"/> of <see cref="IChangeSet{TObject,TKey}"/> of <see cref="IImageFile"/>
    /// indexed by <see cref="ImageId"/> representing changes to the image file collection.
    /// </returns>
    IObservable<IChangeSet<IImageFile, ImageId>> Connect();

    /// <summary>
    /// Initializes the folder within the specified pack folder asynchronously.
    /// </summary>
    /// <param name="folder">
    /// A <see cref="IStorageFolder"/> representing the pack folder.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    Task NewPackAsync(IStorageFolder folder);

    /// <summary>
    /// Adds the image files asynchronously.
    /// </summary>
    /// <returns>
    /// A <see cref="IReadOnlyList{T}"/> of <see cref="IImageFile"/> representing the new image files.
    /// </returns>
    Task<IReadOnlyList<IImageFile>> AddFilesAsync();

    /// <summary>
    /// Renames the specified image file asynchronously.
    /// </summary>
    /// <param name="imageFile">
    /// A <see cref="IImageFile"/> representing the image file to be renamed.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    Task RenameFileAsync(IImageFile imageFile);

    /// <summary>
    /// Deletes the specified image file asynchronously.
    /// </summary>
    /// <param name="imageFile">
    /// A <see cref="IImageFile"/> representing the image file to be deleted.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    Task DeleteFileAsync(IImageFile imageFile);
    
    /// <summary>
    /// Returns the image file with the specified ID.
    /// </summary>
    /// <param name="id">
    /// An <see cref="ImageId"/> representing the ID of the image file to be returned.
    /// </param>
    /// <returns>
    /// An <see cref="IImageFile"/> representing the image file.
    /// </returns>
    IImageFile GetImageFile(ImageId id);
}