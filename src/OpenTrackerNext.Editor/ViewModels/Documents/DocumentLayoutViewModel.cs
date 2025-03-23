using System.Reactive.Disposables;
using System.Reactive.Linq;
using OpenTrackerNext.Core.ViewModels;
using OpenTrackerNext.Editor.Documents;
using OpenTrackerNext.SplatRegistration;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Editor.ViewModels.Documents;

/// <summary>
/// Represents a document layout control view model.
/// </summary>
[Splat]
public sealed class DocumentLayoutViewModel : ViewModel
{
    private readonly IDocumentService _documentService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentLayoutViewModel"/> class.
    /// </summary>
    /// <param name="documentService">
    /// The <see cref="IDocumentService"/>.
    /// </param>
    /// <param name="documentDockFactory">
    /// A factory method for creating new <see cref="DocumentDockViewModel"/> objects.
    /// </param>
    public DocumentLayoutViewModel(IDocumentService documentService, DocumentDockViewModel.Factory documentDockFactory)
    {
        _documentService = documentService;
        LeftDock = documentDockFactory(DocumentSide.Left);
        RightDock = documentDockFactory(DocumentSide.Right);
        
        this.WhenActivated(
            disposables =>
            {
                var rightHasDocuments = this
                    .WhenAnyValue(x => x._documentService.RightHasDocuments);

                rightHasDocuments
                    .ToPropertyEx(
                        this,
                        x => x.RightDockIsVisible,
                        scheduler: RxApp.MainThreadScheduler)
                    .DisposeWith(disposables);

                rightHasDocuments
                    .Select(x => x ? 1 : 3)
                    .ToPropertyEx(
                        this,
                        x => x.LeftDockColumnSpan,
                        scheduler: RxApp.MainThreadScheduler)
                    .DisposeWith(disposables);
            });
    }
    
    /// <summary>
    /// Gets a <see cref="DocumentDockViewModel"/> representing the left document dock.
    /// </summary>
    public DocumentDockViewModel LeftDock { get; }
    
    /// <summary>
    /// Gets a <see cref="DocumentDockViewModel"/> representing the right document dock.
    /// </summary>
    public DocumentDockViewModel RightDock { get; }

    /// <summary>
    /// Gets a value indicating whether the right document dock is visible.
    /// </summary>
    [ObservableAsProperty]
    public bool RightDockIsVisible { get; }
    
    /// <summary>
    /// Gets an <see cref="int"/> representing the number of columns spanned by the left document dock.
    /// </summary>
    [ObservableAsProperty]
    public int LeftDockColumnSpan { get; }
}