using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive;
using System.Threading.Tasks;
using DynamicData;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Results;
using OpenTrackerNext.Core.ViewModels.Dialogs;
using ReactiveUI;

namespace OpenTrackerNext.Editor.Documents;

/// <summary>
/// Manages the state of documents.
/// </summary>
public interface IDocumentService : IDisposable, INotifyPropertyChanged
{
    /// <summary>
    /// Gets a value indicating whether the right document dock has documents.
    /// </summary>
    bool RightHasDocuments { get; }
    
    /// <summary>
    /// Gets a <see cref="Dictionary{TKey,TValue}"/> of <see cref="ReactiveProperty{T}"/> of <see cref="IDocument"/>
    /// indexed by <see cref="DocumentSide"/> representing the active documents.
    /// </summary>
    IDictionary<DocumentSide, IReactiveProperty<IDocument?>> ActiveDocuments { get; }
    
    /// <summary>
    /// Gets a <see cref="Dictionary{TKey,TValue}"/> of <see cref="IObservable{T}"/> of <see cref="IDocument"/> indexed
    /// by <see cref="DocumentSide"/> representing the requests for document activation.
    /// </summary>
    IDictionary<DocumentSide, IObservable<IDocument>> ActivationRequests { get; }
    
    /// <summary>
    /// Gets an <see cref="Interaction{TInput,TOutput}"/> with an input of <see cref="YesNoCancelDialogViewModel"/> and
    /// an output of <see cref="YesNoCancelDialogResult"/> representing the request save dialog interaction.
    /// </summary>
    Interaction<YesNoCancelDialogViewModel, YesNoCancelDialogResult> RequestSaveDialog { get; }
    
    /// <summary>
    /// Gets an <see cref="Interaction{TInput,TOutput}"/> with an input of <see cref="ExceptionDialogViewModel"/>
    /// representing the exception dialog interaction.
    /// </summary>
    Interaction<ExceptionDialogViewModel, Unit> ExceptionDialog { get; }

    /// <summary>
    /// Connects to the document source list observable.
    /// </summary>
    /// <returns>
    /// An <see cref="IObservable{T}"/> of <see cref="IChangeSet{TObject}"/> of <see cref="IDocument"/> representing the
    /// document source list changes.
    /// </returns>
    IObservable<IChangeSet<IDocument>> Connect();

    /// <summary>
    /// Opens the document from the specified file.
    /// </summary>
    /// <param name="file">
    /// A <see cref="IDocumentFile{TData}"/> representing the file to open.
    /// </param>
    /// <typeparam name="TData">
    /// The type of the document data.
    /// </typeparam>
    void Open<TData>(IDocumentFile<TData> file)
        where TData : class, ITitledDocumentData<TData>, new();

    /// <summary>
    /// Moves the document to the other side.
    /// </summary>
    /// <param name="document">
    /// The <see cref="IDocument"/> to move.
    /// </param>
    void MoveToOtherSide(IDocument document);

    /// <summary>
    /// Tries to handle any unsaved changes in the specified document asynchronously.
    /// </summary>
    /// <param name="document">
    /// The <see cref="IDocument"/> for which to handle unsaved changes.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> returning <see cref="OperationResult"/> representing the asynchronous operation result.
    /// </returns>
    Task<OperationResult> TryHandleUnsavedChangesAsync(IDocument document);

    /// <summary>
    /// Tries to handle all unsaved changes asynchronously. 
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> returning <see cref="OperationResult"/> representing the asynchronous operation result.
    /// </returns>
    Task<OperationResult> TryHandleAllUnsavedChangesAsync();

    /// <summary>
    /// Tries to close the specified document asynchronously.
    /// </summary>
    /// <param name="document">
    /// The <see cref="IDocument"/> to close.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    Task TryCloseAsync(IDocument document);

    /// <summary>
    /// Closes the document for the specified file.
    /// </summary>
    /// <param name="file">
    /// A <see cref="IDocumentFile"/> representing the file to close.
    /// </param>
    /// <typeparam name="TData">
    /// The type of the document data.
    /// </typeparam>
    void Close<TData>(IDocumentFile<TData> file)
        where TData : class, ITitledDocumentData<TData>, new();

    /// <summary>
    /// Closes the specified document.
    /// </summary>
    /// <param name="document">
    /// The <see cref="IDocument"/> to close.
    /// </param>
    void Close(IDocument document);

    /// <summary>
    /// Closes all documents.
    /// </summary>
    void CloseAll();
}