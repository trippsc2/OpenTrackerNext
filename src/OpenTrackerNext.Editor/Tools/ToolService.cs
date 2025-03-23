using System;
using System.Collections.Generic;
using DynamicData;
using OpenTrackerNext.SplatRegistration;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Editor.Tools;

/// <inheritdoc cref="IToolService"/>
[Splat(RegisterAsType = typeof(IToolService))]
[SplatSingleInstance]
public sealed class ToolService : ReactiveObject, IToolService
{
    /// <summary>
    /// A <see cref="string"/> representing the tool format.
    /// </summary>
    public const string ToolFormat = "application/x-opentracker-tool";

    private readonly SourceCache<Tool, ToolId> _tools = new(x => x.Id);
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ToolService"/> class.
    /// </summary>
    /// <param name="toolFactory">
    /// The <see cref="IToolFactory"/>.
    /// </param>
    public ToolService(IToolFactory toolFactory)
    {
        _tools.AddOrUpdate(toolFactory.CreateTools());
    }

    /// <summary>
    /// Gets a <see cref="Dictionary{TKey,TValue}"/> of <see cref="ReactiveProperty{T}"/> of <see cref="Tool"/> indexed
    /// by <see cref="ToolBarPosition"/> representing the active tools in each position.
    /// </summary>
    public Dictionary<ToolBarPosition, ReactiveProperty<Tool?>> ActiveTools { get; } = new()
    {
        { ToolBarPosition.TopLeft, new ReactiveProperty<Tool?>() },
        { ToolBarPosition.TopRight, new ReactiveProperty<Tool?>() },
        { ToolBarPosition.BottomLeft, new ReactiveProperty<Tool?>() },
        { ToolBarPosition.BottomRight, new ReactiveProperty<Tool?>() }
    };
    
    /// <summary>
    /// Gets or sets a value indicating whether the drag hit boxes are active.
    /// </summary>
    [Reactive]
    public bool DragHitBoxesAreActive { get; set; }
    
    /// <inheritdoc/>
    public IObservable<IChangeSet<Tool, ToolId>> Connect()
    {
        return _tools.Connect();
    }

    /// <inheritdoc/>
    public void ToggleToolActivation(Tool tool)
    {
        var activeTool = ActiveTools[tool.Position];
        activeTool.Value = activeTool.Value == tool ? null : tool;
    }
    
    /// <inheritdoc/>
    public void DeactivateTool(Tool tool)
    {
        if (ActiveTools[tool.Position].Value != tool)
        {
            return;
        }

        ActiveTools[tool.Position].Value = null;
    }

    /// <inheritdoc/>
    public void MoveTool(Tool tool, ToolBarPosition position)
    {
        if (tool.Position == position)
        {
            return;
        }

        DeactivateTool(tool);
        tool.Position = position;
        ActivateTool(tool);
    }

    /// <inheritdoc/>
    public void DeactivateAllTools()
    {
        foreach (var position in ToolBarPosition.List)
        {
            ActiveTools[position].Value = null;
        }
    }
    
    private void ActivateTool(Tool tool)
    {
        ActiveTools[tool.Position].Value = tool;
    }
}