using System.Reactive;
using OpenTrackerNext.Core.ViewModels;
using OpenTrackerNext.Editor.Documents;
using OpenTrackerNext.SplatRegistration;
using ReactiveUI;

namespace OpenTrackerNext.Editor.ViewModels.Documents;

/// <summary>
/// Represents a document tab item control view model.
/// </summary>
[Splat(RegisterAsType = typeof(Factory))]
public sealed class DocumentViewModel : ViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentViewModel"/> class.
    /// </summary>
    /// <param name="documentService">
    /// The <see cref="IDocumentService"/>.
    /// </param>
    /// <param name="documentBodyFactory">
    /// A factory method for creating new <see cref="DocumentBodyViewModel"/> objects.
    /// </param>
    /// <param name="document">
    /// The <see cref="IDocument"/> to be represented.
    /// </param>
    public DocumentViewModel(
        IDocumentService documentService,
        DocumentBodyViewModel.Factory documentBodyFactory,
        IDocument document)
    {
        Document = document;
        Body = documentBodyFactory(document);
        
        CloseCommand = ReactiveCommand.CreateFromTask(() => documentService.TryCloseAsync(document));
    }
    
    /// <summary>
    /// A factory for creating new <see cref="DocumentViewModel"/> objects.
    /// </summary>
    /// <param name="document">
    /// A <see cref="IDocument"/> to be represented.
    /// </param>
    /// <returns>
    /// A new <see cref="DocumentViewModel"/> object.
    /// </returns>
    public delegate DocumentViewModel Factory(IDocument document);
    
    /// <summary>
    /// Gets a <see cref="IDocument"/> that is the document that this view model represents.
    /// </summary>
    public IDocument Document { get; }
    
    /// <summary>
    /// Gets a <see cref="DocumentBodyViewModel"/> representing the document body control view model.
    /// </summary>
    public DocumentBodyViewModel Body { get; }
    
    /// <summary>
    /// Gets a <see cref="ReactiveCommand{TParam,TResult}"/> that closes the document.
    /// </summary>
    public ReactiveCommand<Unit, Unit> CloseCommand { get; }
}