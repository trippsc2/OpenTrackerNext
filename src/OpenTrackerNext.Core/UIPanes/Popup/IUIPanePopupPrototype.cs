using System.ComponentModel;
using System.Text.Json.Serialization;
using OpenTrackerNext.Core.Documents;

namespace OpenTrackerNext.Core.UIPanes.Popup;

/// <summary>
/// Represents a <see cref="IUIPanePopup"/> within the editor.
/// </summary>
[JsonDerivedType(typeof(NullUIPanePopupPrototype), "null")]
[JsonDerivedType(typeof(UIPanePopupPrototype), "default")]
public interface IUIPanePopupPrototype : IDocumentData, INotifyPropertyChanged;