using System.ComponentModel;
using System.Text.Json.Serialization;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Layouts.Dynamic;
using OpenTrackerNext.Core.Layouts.IconGrid;
using OpenTrackerNext.Core.Layouts.UniformGrid;

namespace OpenTrackerNext.Core.Layouts;

/// <summary>
/// Represents a specific layout type within the editor.
/// </summary>
[JsonDerivedType(typeof(DynamicLayoutPrototype), typeDiscriminator: "dynamic")]
[JsonDerivedType(typeof(IconGridLayoutPrototype), typeDiscriminator: "icon_grid")]
[JsonDerivedType(typeof(UniformGridLayoutPrototype), typeDiscriminator: "uniform_grid")]
public interface ILayoutSubtypePrototype : IDocumentData, INotifyPropertyChanged
{
    /// <summary>
    /// Gets or sets a <see cref="double"/> representing the left margin of the layout.
    /// </summary>
    double LeftMargin { get; set; }
    
    /// <summary>
    /// Gets or sets a <see cref="double"/> representing the top margin of the layout.
    /// </summary>
    double TopMargin { get; set; }
    
    /// <summary>
    /// Gets or sets a <see cref="double"/> representing the right margin of the layout.
    /// </summary>
    double RightMargin { get; set; }
    
    /// <summary>
    /// Gets or sets a <see cref="double"/> representing the bottom margin of the layout.
    /// </summary>
    double BottomMargin { get; set; }
}