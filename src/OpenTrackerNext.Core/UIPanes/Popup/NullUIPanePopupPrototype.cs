using OpenTrackerNext.Document;
using ReactiveUI;

namespace OpenTrackerNext.Core.UIPanes.Popup;

/// <summary>
/// Represents a null <see cref="IUIPanePopupPrototype"/> object.
/// </summary>
[Document]
public sealed partial class NullUIPanePopupPrototype : ReactiveObject, IUIPanePopupPrototype;