using System.Collections.ObjectModel;
using OpenTrackerNext.Document;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.Layouts.UniformGrid;

/// <summary>
/// Represents a <see cref="UniformGridLayout"/> within the editor.
/// </summary>
[Document]
public sealed partial class UniformGridLayoutPrototype : ReactiveObject, ILayoutSubtypePrototype
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
    /// Gets or sets an <see cref="int"/> representing the number of columns.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public int Columns { get; set; } = 1;
        
    /// <summary>
    /// Gets or sets an <see cref="int"/> representing the number of rows.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public int Rows { get; set; } = 1;
    
    /// <summary>
    /// Gets an <see cref="ObservableCollection{T}"/> of <see cref="UniformGridMemberPrototype"/> representing the
    /// members of the layout.
    /// </summary>
    [DocumentMember]
    public ObservableCollection<UniformGridMemberPrototype> Items { get; init; } = [];
}