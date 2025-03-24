using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using OpenTrackerNext.Core.Entities;
using OpenTrackerNext.Core.ViewModels;
using OpenTrackerNext.SplatRegistration;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Editor.ViewModels.Editors.Entities;

/// <summary>
/// Represents the entity editor control view model.
/// </summary>
[Splat(RegisterAsType = typeof(Factory))]
public sealed class EntityEditorViewModel : ViewModel
{
    private readonly EntityPrototype _entityPrototype;

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityEditorViewModel"/> class.
    /// </summary>
    /// <param name="entityPrototype">
    ///     The <see cref="EntityPrototype"/> to be edited.
    /// </param>
    public EntityEditorViewModel(EntityPrototype entityPrototype)
    {
        _entityPrototype = entityPrototype;
        
        this.WhenActivated(
            disposables =>
            {
                this.WhenAnyValue(x => x._entityPrototype.Minimum)
                    .SubscribeOn(RxApp.MainThreadScheduler)
                    .Subscribe(minimum => Minimum = minimum)
                    .DisposeWith(disposables);
                
                this.WhenAnyValue(x => x._entityPrototype.Starting)
                    .SubscribeOn(RxApp.MainThreadScheduler)
                    .Subscribe(starting => Starting = starting)
                    .DisposeWith(disposables);
                
                this.WhenAnyValue(x => x._entityPrototype.Maximum)
                    .SubscribeOn(RxApp.MainThreadScheduler)
                    .Subscribe(maximum => Maximum = maximum)
                    .DisposeWith(disposables);

                this.WhenAnyValue(x => x.Minimum)
                    .Subscribe(minimum => _entityPrototype.Minimum = minimum)
                    .DisposeWith(disposables);
                
                this.WhenAnyValue(x => x.Starting)
                    .Subscribe(starting => _entityPrototype.Starting = starting)
                    .DisposeWith(disposables);
                
                this.WhenAnyValue(x => x.Maximum)
                    .Subscribe(maximum => _entityPrototype.Maximum = maximum)
                    .DisposeWith(disposables);
                
                this.WhenAnyValue(
                        x => x.Starting,
                        x => x.Maximum,
                        (starting, maximum) => (decimal)Math.Min(starting, maximum - 1))
                    .ToPropertyEx(
                        this,
                        x => x.MinimumMaximumValue,
                        scheduler: RxApp.MainThreadScheduler)
                    .DisposeWith(disposables);
                
                this.WhenAnyValue(x => x.Minimum)
                    .Select(minimum => (decimal)minimum)
                    .ToPropertyEx(
                        this,
                        x => x.StartingMinimumValue,
                        scheduler: RxApp.MainThreadScheduler)
                    .DisposeWith(disposables);
                
                this.WhenAnyValue(x => x.Maximum)
                    .Select(maximum => (decimal)maximum)
                    .ToPropertyEx(
                        this,
                        x => x.StartingMaximumValue,
                        scheduler: RxApp.MainThreadScheduler)
                    .DisposeWith(disposables);

                this.WhenAnyValue(
                        x => x.Minimum,
                        x => x.Starting,
                        (minimum, starting) => (decimal)Math.Max(minimum + 1, starting))
                    .ToPropertyEx(
                        this,
                        x => x.MaximumMinimumValue,
                        scheduler: RxApp.MainThreadScheduler)
                    .DisposeWith(disposables);
            });
    }
    
    /// <summary>
    /// A factory method for creating new <see cref="EntityEditorViewModel"/> objects.
    /// </summary>
    /// <param name="entityPrototype">
    ///     The <see cref="EntityPrototype"/> to be edited.
    /// </param>
    /// <returns>
    ///     A new <see cref="EntityEditorViewModel"/> object.
    /// </returns>
    public delegate EntityEditorViewModel Factory(EntityPrototype entityPrototype);
    
    /// <summary>
    /// Gets or sets an <see cref="int"/> representing the entity minimum value.
    /// </summary>
    [Reactive]
    public int Minimum { get; set; }
    
    /// <summary>
    /// Gets a <see cref="decimal"/> representing the maximum allowed entity minimum value.
    /// </summary>
    [ObservableAsProperty]
    public decimal MinimumMaximumValue { get; }
    
    /// <summary>
    /// Gets or sets an <see cref="int"/> representing the entity starting value.
    /// </summary>
    [Reactive]
    public int Starting { get; set; }
    
    /// <summary>
    /// Gets a <see cref="decimal"/> representing the minimum allowed entity starting value.
    /// </summary>
    [ObservableAsProperty]
    public decimal StartingMinimumValue { get; }
    
    /// <summary>
    /// Gets a <see cref="decimal"/> representing the maximum allowed entity starting value.
    /// </summary>
    [ObservableAsProperty]
    public decimal StartingMaximumValue { get; }
    
    /// <summary>
    /// Gets or sets an <see cref="int"/> representing the entity maximum value.
    /// </summary>
    [Reactive]
    public int Maximum { get; set; }
    
    /// <summary>
    /// Gets a <see cref="decimal"/> representing the minimum allowed entity maximum value.
    /// </summary>
    [ObservableAsProperty]
    public decimal MaximumMinimumValue { get; }
}