using System.ComponentModel;
using System.Text.Json.Serialization;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Maps.Dynamic;
using OpenTrackerNext.Core.Maps.Static;

namespace OpenTrackerNext.Core.Maps;

/// <summary>
/// Represents a specific map type within the editor.
/// </summary>
[JsonDerivedType(typeof(DynamicMapPrototype), typeDiscriminator: "dynamic")]
[JsonDerivedType(typeof(StaticMapPrototype), typeDiscriminator: "static")]
public interface IMapSubtypePrototype : IDocumentData, INotifyPropertyChanged;