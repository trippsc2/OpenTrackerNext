using System.Reactive;
using System.Reactive.Disposables;
using OpenTrackerNext.Core.ViewModels;
using OpenTrackerNext.Editor.Tools;
using OpenTrackerNext.SplatRegistration;
using ReactiveUI;

namespace OpenTrackerNext.Editor.ViewModels.Tools;

/// <summary>
/// Represents a tool control view model.
/// </summary>
[Splat(RegisterAsType = typeof(Factory))]
public sealed class ToolViewModel : ViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ToolViewModel"/> class.
    /// </summary>
    /// <param name="toolService">
    /// The <see cref="IToolService"/>.
    /// </param>
    /// <param name="tool">
    /// A <see cref="Tool"/> that is the tool to be represented.
    /// </param>
    public ToolViewModel(IToolService toolService, Tool tool)
    {
        Tool = tool;
        Title = tool.Title;
        Content = tool.Content;

        DeactivateCommand = ReactiveCommand.Create(() => toolService.DeactivateTool(tool));
        EnableDragHitBoxesCommand = ReactiveCommand.Create(() => { toolService.DragHitBoxesAreActive = true; });
        DisableDragHitBoxesCommand = ReactiveCommand.Create(() => { toolService.DragHitBoxesAreActive = false; });
        MoveCommand = ReactiveCommand.Create<ToolBarPosition>(position => toolService.MoveTool(tool, position));

        this.WhenActivated(
            disposables =>
            {
                tool.IsActive = true;

                Disposable
                    .Create(() => tool.IsActive = false)
                    .DisposeWith(disposables);
            });
    }

    /// <summary>
    /// A factory method for creating new <see cref="ToolViewModel"/> objects for the specified tool.
    /// </summary>
    /// <param name="tool">
    /// A <see cref="Tool"/> that is the tool to be represented.
    /// </param>
    /// <returns>
    /// A new <see cref="ToolViewModel"/> object.
    /// </returns>
    public delegate ToolViewModel Factory(Tool tool);

    /// <summary>
    /// Gets a <see cref="Tool"/> that is the tool that this view model represents.
    /// </summary>
    public Tool Tool { get; }

    /// <summary>
    /// Gets a <see cref="string"/> representing the tool title.
    /// </summary>
    public string Title { get; }
    
    /// <summary>
    /// Gets a <see cref="ViewModel"/> representing the tool body content view model.
    /// </summary>
    public ViewModel Content { get; }

    /// <summary>
    /// Gets a <see cref="ReactiveCommand{TParam,TResult}"/> to deactivate the tool.
    /// </summary>
    public ReactiveCommand<Unit, Unit> DeactivateCommand { get; }
    
    /// <summary>
    /// Gets a <see cref="ReactiveCommand{TParam,TResult}"/> to enable drag and drop hit boxes.
    /// </summary>
    public ReactiveCommand<Unit, Unit> EnableDragHitBoxesCommand { get; }
    
    /// <summary>
    /// Gets a <see cref="ReactiveCommand{TParam,TResult}"/> to disable drag and drop hit boxes.
    /// </summary>
    public ReactiveCommand<Unit, Unit> DisableDragHitBoxesCommand { get; }
    
    /// <summary>
    /// Gets a <see cref="ReactiveCommand{TParam,TResult}"/> with a <see cref="ToolBarPosition"/> parameter to move the
    /// tool to the specified position.
    /// </summary>
    public ReactiveCommand<ToolBarPosition, Unit> MoveCommand { get; }
}