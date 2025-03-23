using System;
using System.Collections.Generic;
using System.ComponentModel;
using DynamicData;
using ReactiveUI;

namespace OpenTrackerNext.Editor.Tools;

/// <summary>
/// Manages the state of tools.
/// </summary>
public interface IToolService : INotifyPropertyChanged
{
    /// <summary>
    /// Gets a <see cref="Dictionary{TKey,TValue}"/> of <see cref="ReactiveProperty{T}"/> of <see cref="Tool"/> indexed
    /// by <see cref="ToolBarPosition"/> representing the active tools.
    /// </summary>
    Dictionary<ToolBarPosition, ReactiveProperty<Tool?>> ActiveTools { get; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the drag hit boxes are active.
    /// </summary>
    bool DragHitBoxesAreActive { get; set; }

    /// <summary>
    /// Connects to the tool cache observable.
    /// </summary>
    /// <returns>
    /// An <see cref="IObservable{T}"/> of <see cref="IChangeSet"/> of <see cref="Tool"/> objects indexed by
    /// <see cref="string"/> representing the tools collection changes.
    /// </returns>
    IObservable<IChangeSet<Tool, ToolId>> Connect();
    
    /// <summary>
    /// Toggles the activation status of the specified tool.
    /// </summary>
    /// <param name="tool">
    /// A <see cref="Tool"/> representing the tool for which to toggle activation.
    /// </param>
    void ToggleToolActivation(Tool tool);
    
    /// <summary>
    /// Deactivates the specified tool, if it is active.
    /// </summary>
    /// <param name="tool">
    /// A <see cref="Tool"/> representing the tool to deactivate.
    /// </param>
    void DeactivateTool(Tool tool);
    
    /// <summary>
    /// Moves the specified tool to the specified tool bar position.
    /// </summary>
    /// <param name="tool">
    /// A <see cref="Tool"/> representing the tool to move.
    /// </param>
    /// <param name="position">
    /// A <see cref="ToolBarPosition"/> representing the new tool bar position.
    /// </param>
    void MoveTool(Tool tool, ToolBarPosition position);
    
    /// <summary>
    /// Deactivates all active tools.
    /// </summary>
    void DeactivateAllTools();
}