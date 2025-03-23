using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using OpenTrackerNext.Core.Factories;
using OpenTrackerNext.Core.ViewModels;
using OpenTrackerNext.Editor.Documents;
using OpenTrackerNext.SplatRegistration;
using ReactiveUI;

namespace OpenTrackerNext.Editor.ViewModels.Documents;

/// <summary>
/// Represents a document body control view model.
/// </summary>
[Splat(RegisterAsType = typeof(Factory))]
public sealed class DocumentBodyViewModel : ViewModel
{
    private readonly IDocument _document;

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentBodyViewModel"/> class.
    /// </summary>
    /// <param name="documentViewModelFactory">
    /// The <see cref="IFactory{TInput,TOutput}"/> that creates new <see cref="ViewModel"/> objects from
    /// <see cref="IDocument"/> objects.
    /// </param>
    /// <param name="document">
    /// The <see cref="IDocument"/> to be represented.
    /// </param>
    public DocumentBodyViewModel(IFactory<IDocument, ViewModel> documentViewModelFactory, IDocument document)
    {
        _document = document;
        Content = documentViewModelFactory.Create(document);
        
        this.WhenActivated(
            disposables =>
            {
                var isUnsaved = this
                    .WhenAnyValue(x => x._document.IsUnsaved)
                    .ObserveOn(RxApp.MainThreadScheduler);

                RevertCommand = ReactiveCommand
                    .Create(document.Revert, isUnsaved)
                    .DisposeWith(disposables);
                SaveCommand = ReactiveCommand
                    .CreateFromTask(document.SaveAsync, isUnsaved)
                    .DisposeWith(disposables);
            });
    }

    /// <summary>
    /// A factory for creating new <see cref="DocumentBodyViewModel"/> objects.
    /// </summary>
    /// <param name="document">
    /// The <see cref="IDocument"/> to be represented.
    /// </param>
    /// <returns>
    /// A new <see cref="DocumentBodyViewModel"/> object.
    /// </returns>
    public delegate DocumentBodyViewModel Factory(IDocument document);
    
    /// <summary>
    /// Gets a <see cref="ViewModel"/> representing the content of the document.
    /// </summary>
    public ViewModel Content { get; }

    /// <summary>
    /// Gets a <see cref="ReactiveCommand{TInput,TOutput}"/> that reverts the document.
    /// </summary>
    public ReactiveCommand<Unit, Unit> RevertCommand { get; private set; } = null!;
    
    /// <summary>
    /// Gets a <see cref="ReactiveCommand{TInput,TOutput}"/> that saves the document.
    /// </summary>
    public ReactiveCommand<Unit, Unit> SaveCommand { get; private set; } = null!;
}