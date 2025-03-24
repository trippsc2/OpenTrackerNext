using OpenTrackerNext.Core.Factories;
using OpenTrackerNext.Core.Metadata;
using OpenTrackerNext.Core.ViewModels;
using OpenTrackerNext.Editor.Documents;
using OpenTrackerNext.Factories;
using OpenTrackerNext.SplatRegistration;

namespace OpenTrackerNext.Editor.ViewModels.Editors.Metadata;

/// <summary>
/// Represents the creation logic for the <see cref="PackMetadataEditorViewModel"/>.
/// </summary>
[Factory]
[Splat]
[SplatSingleInstance]
public sealed partial class PackMetadataEditorViewModelFactory
    : ISpecificFactory<IDocument, ViewModel, Document<PackMetadata>>
{
    private readonly PackMetadataEditorViewModel.Factory _factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="PackMetadataEditorViewModelFactory"/> class.
    /// </summary>
    /// <param name="factory">
    /// A factory method for creating the <see cref="PackMetadataEditorViewModel"/> object.
    /// </param>
    public PackMetadataEditorViewModelFactory(PackMetadataEditorViewModel.Factory factory)
    {
        _factory = factory;
    }

    /// <inheritdoc/>
    public ViewModel Create(Document<PackMetadata> document)
    {
        return _factory(document.File.WorkingData);
    }
}