using OpenTrackerNext.Core.Entities;
using OpenTrackerNext.Core.Factories;
using OpenTrackerNext.Core.ViewModels;
using OpenTrackerNext.Editor.Documents;
using OpenTrackerNext.Factories;
using OpenTrackerNext.SplatRegistration;

namespace OpenTrackerNext.Editor.ViewModels.Editors.Entities;

/// <summary>
/// Creation logic for <see cref="EntityEditorViewModel"/> objects.
/// </summary>
[Factory]
[Splat]
[SplatSingleInstance]
public sealed partial class EntityEditorViewModelFactory
    : ISpecificFactory<IDocument, ViewModel, Document<EntityPrototype>>
{
    private readonly EntityEditorViewModel.Factory _factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityEditorViewModelFactory"/> class.
    /// </summary>
    /// <param name="factory">
    /// A factory for creating new <see cref="EntityEditorViewModel"/> objects.
    /// </param>
    public EntityEditorViewModelFactory(EntityEditorViewModel.Factory factory)
    {
        _factory = factory;
    }

    /// <inheritdoc/>
    public ViewModel Create(Document<EntityPrototype> input)
    {
        return _factory(input.File.WorkingData);
    }
}