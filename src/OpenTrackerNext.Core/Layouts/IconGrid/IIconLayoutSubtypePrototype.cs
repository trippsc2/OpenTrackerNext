using System.ComponentModel;
using System.Text.Json.Serialization;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Layouts.IconGrid.Dynamic;
using OpenTrackerNext.Core.Layouts.IconGrid.Static;

namespace OpenTrackerNext.Core.Layouts.IconGrid;

/// <summary>
/// Represents an icon within an icon grid layout within the editor.
/// </summary>
[JsonDerivedType(typeof(DynamicIconLayoutPrototype), typeDiscriminator: "dynamic")]
[JsonDerivedType(typeof(StaticIconLayoutPrototype), typeDiscriminator: "static")]
public interface IIconLayoutSubtypePrototype : IDocumentData, INotifyPropertyChanged;