using System.ComponentModel;
using System.Text.Json.Serialization;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Icons.Counted;
using OpenTrackerNext.Core.Icons.DoubleEntity;
using OpenTrackerNext.Core.Icons.SingleEntity;
using OpenTrackerNext.Core.Icons.Static;

namespace OpenTrackerNext.Core.Icons;

/// <summary>
/// Represents a specific <see cref="IIcon"/> subtype within the editor.
/// </summary>
[JsonDerivedType(typeof(StaticIconPrototype), typeDiscriminator: "static")]
[JsonDerivedType(typeof(CountedIconPrototype), typeDiscriminator: "counted")]
[JsonDerivedType(typeof(SingleEntityIconPrototype), typeDiscriminator: "single")]
[JsonDerivedType(typeof(DoubleEntityIconPrototype), typeDiscriminator: "double")]
public interface IIconSubtypePrototype : IDocumentData, INotifyPropertyChanged;