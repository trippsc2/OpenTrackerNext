using System.Collections.Generic;
using OpenTrackerNext.Core.Entities;
using OpenTrackerNext.Core.Icons;
using OpenTrackerNext.Core.Layouts;
using OpenTrackerNext.Core.MapLayouts;
using OpenTrackerNext.Core.Maps;
using OpenTrackerNext.Core.UIPanes;
using OpenTrackerNext.Editor.ViewModels.Tools;
using OpenTrackerNext.SplatRegistration;

namespace OpenTrackerNext.Editor.Tools;

/// <inheritdoc cref="IToolFactory"/>
[Splat(RegisterAsType = typeof(IToolFactory))]
[SplatSingleInstance]
public sealed class ToolFactory : IToolFactory
{
    private readonly GlobalToolViewModel _globalToolViewModel;
    private readonly FolderToolViewModel<EntityPrototype, EntityId> _entitiesToolViewModel;
    private readonly ImagesToolViewModel _imagesToolViewModel;
    private readonly FolderToolViewModel<MapPrototype, MapId> _mapsToolViewModel;
    private readonly FolderToolViewModel<IconPrototype, IconId> _iconsToolViewModel;
    private readonly FolderToolViewModel<LayoutPrototype, LayoutId> _layoutsToolViewModel;
    private readonly FolderToolViewModel<MapLayoutPrototype, MapLayoutId> _mapLayoutsToolViewModel;
    private readonly FolderToolViewModel<UIPanePrototype, UIPaneId> _uiPanesToolViewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="ToolFactory"/> class.
    /// </summary>
    /// <param name="globalToolViewModel">
    ///     The <see cref="GlobalToolViewModel"/>.
    /// </param>
    /// <param name="entitiesToolViewModel">
    ///     The <see cref="FolderToolViewModel{TData,TId}"/> with <see cref="EntityPrototype"/> data and ID of
    ///     <see cref="EntityId"/>.
    /// </param>
    /// <param name="imagesToolViewModel">
    ///     The <see cref="ImagesToolViewModel"/>.
    /// </param>
    /// <param name="mapsToolViewModel">
    ///     The <see cref="FolderToolViewModel{TData,TId}"/> with <see cref="MapPrototype"/> data and ID of
    ///     <see cref="MapId"/>.
    /// </param>
    /// <param name="iconsToolViewModel">
    ///     The <see cref="FolderToolViewModel{TData,TId}"/> with <see cref="IconPrototype"/> data and ID of
    ///     <see cref="IconId"/>.
    /// </param>
    /// <param name="layoutsToolViewModel">
    ///     The <see cref="FolderToolViewModel{TData,TId}"/> with <see cref="LayoutPrototype"/> data and ID of
    ///     <see cref="LayoutId"/>.
    /// </param>
    /// <param name="mapLayoutsToolViewModel">
    ///     The <see cref="FolderToolViewModel{TData,TId}"/> with <see cref="MapLayoutPrototype"/> data and ID of
    ///     <see cref="MapLayoutId"/>.
    /// </param>
    /// <param name="uiPanesToolViewModel">
    ///     The <see cref="FolderToolViewModel{TData,TId}"/> with <see cref="UIPanePrototype"/> data and ID of
    ///     <see cref="UIPaneId"/>.
    /// </param>
    public ToolFactory(
        GlobalToolViewModel globalToolViewModel,
        FolderToolViewModel<EntityPrototype, EntityId> entitiesToolViewModel,
        ImagesToolViewModel imagesToolViewModel,
        FolderToolViewModel<MapPrototype, MapId> mapsToolViewModel,
        FolderToolViewModel<IconPrototype, IconId> iconsToolViewModel,
        FolderToolViewModel<LayoutPrototype, LayoutId> layoutsToolViewModel,
        FolderToolViewModel<MapLayoutPrototype, MapLayoutId> mapLayoutsToolViewModel,
        FolderToolViewModel<UIPanePrototype, UIPaneId> uiPanesToolViewModel)
    {
        _entitiesToolViewModel = entitiesToolViewModel;
        _globalToolViewModel = globalToolViewModel;
        _imagesToolViewModel = imagesToolViewModel;
        _mapsToolViewModel = mapsToolViewModel;
        _iconsToolViewModel = iconsToolViewModel;
        _layoutsToolViewModel = layoutsToolViewModel;
        _mapLayoutsToolViewModel = mapLayoutsToolViewModel;
        _uiPanesToolViewModel = uiPanesToolViewModel;
    }

    /// <inheritdoc/>
    public IEnumerable<Tool> CreateTools()
    {
        return new List<Tool>
        {
            CreateGlobalTool(),
            CreateEntitiesTool(),
            CreateImagesTool(),
            CreateMapsTool(),
            CreateIconsTool(),
            CreateLayoutsTool(),
            CreateMapLayoutsTool(),
            CreateUIPanesTool()
        };
    }

    private Tool CreateGlobalTool()
    {
        return new Tool
        {
            Id = new ToolId("global"),
            Title = "Global",
            Content = _globalToolViewModel,
            ToolTipHeader = "Global",
            ToolTipContent = "Global settings that affect the entire tracker pack",
            Position = ToolBarPosition.TopLeft
        };
    }

    private Tool CreateEntitiesTool()
    {
        return new Tool
        {
            Id = new ToolId("entities"),
            Title = "Entities",
            Content = _entitiesToolViewModel,
            ToolTipHeader = "Entities",
            ToolTipContent = "Entities are any variable that affects the tracker logic (modes, items, etc.)",
            Position = ToolBarPosition.TopLeft
        };
    }

    private Tool CreateImagesTool()
    {
        return new Tool
        {
            Id = new ToolId("images"),
            Title = "Images",
            Content = _imagesToolViewModel,
            ToolTipHeader = "Images",
            ToolTipContent = "Images are the image files to be used in the tracker",
            Position = ToolBarPosition.TopLeft
        };
    }

    private Tool CreateMapsTool()
    {
        return new Tool
        {
            Id = new ToolId("maps"),
            Title = "Maps",
            Content = _mapsToolViewModel,
            ToolTipHeader = "Maps",
            ToolTipContent = "Maps are areas of the game world to be presented in the tracker",
            Position = ToolBarPosition.TopLeft
        };
    }

    private Tool CreateIconsTool()
    {
        return new Tool
        {
            Id = new ToolId("icons"),
            Title = "Icons",
            Content = _iconsToolViewModel,
            ToolTipHeader = "Icons",
            ToolTipContent = "Icons are used to indicate and manipulate tracker state",
            Position = ToolBarPosition.TopLeft
        };
    }
    
    private Tool CreateLayoutsTool()
    {
        return new Tool
        {
            Id = new ToolId("layouts"),
            Title = "Layouts",
            Content = _layoutsToolViewModel,
            ToolTipHeader = "Layouts",
            ToolTipContent = "Layouts are the visual arrangement of the UI panes",
            Position = ToolBarPosition.TopRight
        };
    }
    
    private Tool CreateMapLayoutsTool()
    {
        return new Tool
        {
            Id = new ToolId("mapLayouts"),
            Title = "Map Layouts",
            Content = _mapLayoutsToolViewModel,
            ToolTipHeader = "Map Layouts",
            ToolTipContent = "Map layouts are the visual arrangement of the maps",
            Position = ToolBarPosition.TopRight
        };
    }
    
    private Tool CreateUIPanesTool()
    {
        return new Tool
        {
            Id = new ToolId("uiPanes"),
            Title = "UI Panes",
            Content = _uiPanesToolViewModel,
            ToolTipHeader = "UI Panes",
            ToolTipContent = "UI panes are the boxes separate from the maps",
            Position = ToolBarPosition.TopRight
        };
    }
}