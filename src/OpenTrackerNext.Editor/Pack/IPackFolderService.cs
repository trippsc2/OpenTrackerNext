using System.Collections.Generic;
using System.Reactive;
using OpenTrackerNext.Core.Results;
using OpenTrackerNext.Core.Storage;
using OpenTrackerNext.Core.Validation;
using OpenTrackerNext.Core.ViewModels.Dialogs;
using ReactiveUI;

namespace OpenTrackerNext.Editor.Pack;

/// <summary>
/// Represents a service that manages a folder of pack files.
/// </summary>
public interface IPackFolderService : IPackComponentService
{
    /// <summary>
    /// Gets a <see cref="string"/> representing the folder name.
    /// </summary>
    string FolderName { get; }
    
    /// <summary>
    /// Gets a <see cref="string"/> representing the item name.
    /// </summary>
    string ItemName { get; }
    
    /// <summary>
    /// Gets a <see cref="List{T}"/> of <see cref="ValidationRule{T}"/> of <see cref="string"/> representing the
    /// validation rules for the file friendly name.
    /// </summary>
    List<ValidationRule<string?>> NameValidationRules { get; }

    /// <summary>
    /// Gets an <see cref="Interaction{TInput,TOutput}"/> with an input of <see cref="TextBoxDialogViewModel"/> and an
    /// output of <see cref="OperationResult"/> representing opening the add file dialog.
    /// </summary>
    Interaction<TextBoxDialogViewModel, OperationResult> AddFileDialog { get; }
    
    /// <summary>
    /// Gets an <see cref="Interaction{TInput,TOutput}"/> with an input of <see cref="TextBoxDialogViewModel"/> and an
    /// output of <see cref="OperationResult"/> representing the rename file dialog.
    /// </summary>
    Interaction<TextBoxDialogViewModel, OperationResult> RenameFileDialog { get; }
    
    /// <summary>
    /// Gets an <see cref="Interaction{TInput,TOutput}"/> with an input of <see cref="YesNoDialogViewModel"/> and an
    /// output of <see cref="YesNoDialogResult"/> representing the delete file dialog.
    /// </summary>
    Interaction<YesNoDialogViewModel, YesNoDialogResult> DeleteFileDialog { get; }
    
    /// <summary>
    /// Gets an <see cref="Interaction{TInput,TOutput}"/> with an input of <see cref="ExceptionDialogViewModel"/>
    /// representing the exception dialog.
    /// </summary>
    Interaction<ExceptionDialogViewModel, Unit> ExceptionDialog { get; }
    
    /// <summary>
    /// Initializes the folder within the specified pack folder.
    /// </summary>
    /// <param name="folder">
    /// A <see cref="IStorageFolder"/> representing the pack folder.
    /// </param>
    void NewPack(IStorageFolder folder);
}