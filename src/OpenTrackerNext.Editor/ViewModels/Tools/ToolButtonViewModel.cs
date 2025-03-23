using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using OpenTrackerNext.Core.ViewModels;
using OpenTrackerNext.Editor.Tools;
using OpenTrackerNext.SplatRegistration;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Editor.ViewModels.Tools;

/// <summary>
/// Represents a tool button control view model.
/// </summary>
[Splat(RegisterAsType = typeof(Factory))]
public sealed class ToolButtonViewModel : ViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ToolButtonViewModel"/> class.
    /// </summary>
    /// <param name="toolService">
    /// The <see cref="IToolService"/>.
    /// </param>
    /// <param name="tool">
    /// A <see cref="Tool"/> that is the tool to be represented.
    /// </param>
    public ToolButtonViewModel(IToolService toolService, Tool tool)
    {
        Tool = tool;
        Content = tool.Title;
        ToolTipHeader = tool.ToolTipHeader;
        ToolTipContent = tool.ToolTipContent;

        ToggleActivationCommand = ReactiveCommand.Create(() => toolService.ToggleToolActivation(Tool));

        this.WhenActivated(
            disposables =>
            {
                this.WhenAnyValue(x => x.Tool.Position)
                    .Select(x => x.ButtonAngle)
                    .ToPropertyEx(
                        this,
                        x => x.ButtonAngle,
                        scheduler: RxApp.MainThreadScheduler)
                    .DisposeWith(disposables);
                this.WhenAnyValue(x => x.Tool.IsActive)
                    .ToPropertyEx(
                        this,
                        x => x.IsActive,
                        scheduler: RxApp.MainThreadScheduler)
                    .DisposeWith(disposables);
            });
    }

    /// <summary>
    /// A factory method for creating new <see cref="ToolButtonViewModel"/> object with the specified tool.
    /// </summary>
    /// <param name="tool">
    /// A <see cref="Tool"/> that is the tool to be represented.
    /// </param>
    /// <returns>
    /// A new <see cref="ToolButtonViewModel"/> object.
    /// </returns>
    public delegate ToolButtonViewModel Factory(Tool tool);
    
    /// <summary>
    /// Gets a <see cref="Tool"/> that is the tool that this button view model represents.
    /// </summary>
    public Tool Tool { get; }
    
    /// <summary>
    /// Gets a <see cref="string"/> representing the tool button content.
    /// </summary>
    public string Content { get; }
    
    /// <summary>
    /// Gets a <see cref="string"/> representing the button tooltip header.
    /// </summary>
    public string? ToolTipHeader { get; }
    
    /// <summary>
    /// Gets a <see cref="string"/> representing the button tooltip content.
    /// </summary>
    public string? ToolTipContent { get; }

    /// <summary>
    /// Gets a <see cref="double"/> representing the angle of the tool bar button.
    /// </summary>
    [ObservableAsProperty]
    public double ButtonAngle { get; }
    
    /// <summary>
    /// Gets a value indicating whether the tool is active.
    /// </summary>
    [ObservableAsProperty]
    public bool IsActive { get; }

    /// <summary>
    /// Gets a <see cref="ReactiveCommand{TParam,TResult}"/> to toggle the tool activation.
    /// </summary>
    public ReactiveCommand<Unit, Unit> ToggleActivationCommand { get; }
}