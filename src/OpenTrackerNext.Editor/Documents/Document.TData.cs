using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Entities;
using OpenTrackerNext.Core.Icons;
using OpenTrackerNext.Core.Layouts;
using OpenTrackerNext.Core.MapLayouts;
using OpenTrackerNext.Core.Maps;
using OpenTrackerNext.Core.Metadata;
using OpenTrackerNext.Core.UIPanes;
using OpenTrackerNext.SplatRegistration;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Editor.Documents;

/// <inheritdoc cref="IDocument{TData}"/>
[SplatGeneric(
    ConcreteType = typeof(Document<EntityPrototype>),
    RegisterAsType = typeof(IDocument<EntityPrototype>.Factory))]
[SplatGeneric(
    ConcreteType = typeof(Document<IconPrototype>),
    RegisterAsType = typeof(IDocument<IconPrototype>.Factory))]
[SplatGeneric(
    ConcreteType = typeof(Document<LayoutPrototype>),
    RegisterAsType = typeof(IDocument<LayoutPrototype>.Factory))]
[SplatGeneric(
    ConcreteType = typeof(Document<MapLayoutPrototype>),
    RegisterAsType = typeof(IDocument<MapLayoutPrototype>.Factory))]
[SplatGeneric(
    ConcreteType = typeof(Document<MapPrototype>),
    RegisterAsType = typeof(IDocument<MapPrototype>.Factory))]
[SplatGeneric(
    ConcreteType = typeof(Document<PackMetadata>),
    RegisterAsType = typeof(IDocument<PackMetadata>.Factory))]
[SplatGeneric(
    ConcreteType = typeof(Document<UIPanePrototype>),
    RegisterAsType = typeof(IDocument<UIPanePrototype>.Factory))]
public sealed class Document<TData> : ReactiveObject, IDocument<TData>
    where TData : class, ITitledDocumentData<TData>, new()
{
    private readonly CompositeDisposable _disposables = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Document{T}"/> class of <see cref="TData"/>.
    /// </summary>
    /// <param name="file">
    /// A <see cref="IDocumentFile"/> of <see cref="TData"/> representing the file.
    /// </param>
    public Document(IDocumentFile<TData> file)
    {
        File = file;

        file.SavedData
            .DocumentChanges
            .Merge(file.WorkingData.DocumentChanges)
            .Select(_ => !file.SavedData.ValueEquals(file.WorkingData))
            .ToPropertyEx(this, x => x.IsUnsaved)
            .DisposeWith(_disposables);
        this.WhenAnyValue(x => x.File.FriendlyId)
            .Select(friendlyId => $"{file.TitlePrefix}{friendlyId}")
            .ToPropertyEx(this, x => x.BaseTitle)
            .DisposeWith(_disposables);
        this.WhenAnyValue(
                x => x.File.FriendlyId,
                x => x.IsUnsaved,
                (friendlyId, isUnsaved) =>
                    $"{file.TitlePrefix}{friendlyId}{(isUnsaved ? "*" : string.Empty)}")
            .ToPropertyEx(this, x => x.Title)
            .DisposeWith(_disposables);
    }

    /// <inheritdoc/>
    public IDocumentFile<TData> File { get; }
    
    /// <inheritdoc/>
    [Reactive]
    public DocumentSide Side { get; set; }
    
    /// <inheritdoc/>
    [ObservableAsProperty]
    public bool IsUnsaved { get; }
    
    /// <inheritdoc/>
    [ObservableAsProperty]
    public string BaseTitle { get; } = null!;
    
    /// <inheritdoc/>
    [ObservableAsProperty]
    public string Title { get; } = null!;
    
    /// <inheritdoc/>
    public void Dispose()
    {
        _disposables.Dispose();
    }

    /// <inheritdoc/>
    public void Revert()
    {
        File.Revert();
    }

    /// <inheritdoc/>
    public Task SaveAsync()
    {
        return File.SaveAsync();
    }
}