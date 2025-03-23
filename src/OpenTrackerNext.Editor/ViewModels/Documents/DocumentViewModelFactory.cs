using OpenTrackerNext.Core.Factories;
using OpenTrackerNext.Core.ViewModels;
using OpenTrackerNext.Editor.Documents;
using OpenTrackerNext.Factories;
using OpenTrackerNext.SplatRegistration;

namespace OpenTrackerNext.Editor.ViewModels.Documents;

/// <summary>
/// Creation logic for document view models.
/// </summary>
[AbstractFactory]
[Splat(RegisterAsType = typeof(IFactory<IDocument, ViewModel>))]
[SplatSingleInstance]
public sealed partial class DocumentViewModelFactory : IFactory<IDocument, ViewModel>;