using System.Collections.ObjectModel;
using OpenTrackerNext.Document;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.Layouts.IconGrid;

/// <summary>
/// Represents an <see cref="IconGridLayout"/> within the editor.
/// </summary>
[Document]
public sealed partial class IconGridLayoutPrototype : ReactiveObject, ILayoutSubtypePrototype
{
    /// <inheritdoc/>
    [DocumentMember]
    [Reactive]
    public double LeftMargin { get; set; }
    
    /// <inheritdoc/>
    [DocumentMember]
    [Reactive]
    public double TopMargin { get; set; }
    
    /// <inheritdoc/>
    [DocumentMember]
    [Reactive]
    public double RightMargin { get; set; }
    
    /// <inheritdoc/>
    [DocumentMember]
    [Reactive]
    public double BottomMargin { get; set; }
    
    /// <summary>
    /// Gets or sets an <see cref="int"/> representing the horizontal spacing between icons.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public int HorizontalSpacing { get; set; }

    /// <summary>
    /// Gets or sets an <see cref="int"/> representing the vertical spacing between icons.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public int VerticalSpacing { get; set; }

    /// <summary>
    /// Gets or sets an <see cref="int"/> representing the icon height.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public int IconHeight { get; set; } = 32;

    /// <summary>
    /// Gets or sets an <see cref="int"/> representing the icon width.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public int IconWidth { get; set; } = 32;

    /// <summary>
    /// Gets an <see cref="ObservableCollection{T}"/> of <see cref="IconGridRowPrototype"/> representing the rows of
    /// icons.
    /// </summary>
    [DocumentMember]
    public ObservableCollection<IconGridRowPrototype> Rows { get; init; } = [];
}