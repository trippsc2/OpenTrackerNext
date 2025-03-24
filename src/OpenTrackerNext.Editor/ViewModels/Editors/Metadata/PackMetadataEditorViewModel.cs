using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using OpenTrackerNext.Core.Metadata;
using OpenTrackerNext.Core.ViewModels;
using OpenTrackerNext.SplatRegistration;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Editor.ViewModels.Editors.Metadata;

/// <summary>
/// Represents the metadata editor control view model.
/// </summary>
[Splat(RegisterAsType = typeof(Factory))]
public sealed class PackMetadataEditorViewModel : ViewModel
{
    private readonly PackMetadata _packMetadata;

    /// <summary>
    /// Initializes a new instance of the <see cref="PackMetadataEditorViewModel"/> class.
    /// </summary>
    /// <param name="metadata">
    ///     The <see cref="PackMetadata"/> to be edited.
    /// </param>
    public PackMetadataEditorViewModel(PackMetadata metadata)
    {
        _packMetadata = metadata;
        Name = metadata.Name;
        Author = metadata.Author;
        
        this.WhenActivated(
            disposables =>
            {
                this.WhenAnyValue(x => x._packMetadata.Name)
                    .SubscribeOn(RxApp.MainThreadScheduler)
                    .Subscribe(name => Name = name)
                    .DisposeWith(disposables);
                
                this.WhenAnyValue(x => x._packMetadata.Author)
                    .SubscribeOn(RxApp.MainThreadScheduler)
                    .Subscribe(author => Author = author)
                    .DisposeWith(disposables);
                
                this.WhenAnyValue(x => x._packMetadata.Version)
                    .SubscribeOn(RxApp.MainThreadScheduler)
                    .Subscribe(version =>
                    {
                        MajorVersion = version.Major;
                        MinorVersion = version.Minor;
                        BuildVersion = version.Build;
                        RevisionVersion = version.Revision;
                    })
                    .DisposeWith(disposables);
                
                this.WhenAnyValue(x => x.Name)
                    .Subscribe(name => _packMetadata.Name = name)
                    .DisposeWith(disposables);
                
                this.WhenAnyValue(x => x.Author)
                    .Subscribe(author => _packMetadata.Author = author)
                    .DisposeWith(disposables);
                
                this.WhenAnyValue(
                        x => x.MajorVersion,
                        x => x.MinorVersion,
                        x => x.BuildVersion,
                        x => x.RevisionVersion,
                        (major, minor, build, revision) => new Version(major, minor, build, revision))
                    .Subscribe(version => _packMetadata.Version = version)
                    .DisposeWith(disposables);
            });
    }
    
    /// <summary>
    /// A factory method for creating new <see cref="PackMetadataEditorViewModel"/> objects.
    /// </summary>
    /// <param name="metadata">
    ///     The <see cref="PackMetadata"/> to be edited.
    /// </param>
    /// <returns>
    ///     A new <see cref="PackMetadataEditorViewModel"/> object.
    /// </returns>
    public delegate PackMetadataEditorViewModel Factory(PackMetadata metadata);
    
    /// <summary>
    /// Gets or sets a <see cref="string"/> representing the pack name.
    /// </summary>
    [Reactive]
    public string Name { get; set; }
    
    /// <summary>
    /// Gets or sets a <see cref="string"/> representing the pack author.
    /// </summary>
    [Reactive]
    public string Author { get; set; }
    
    /// <summary>
    /// Gets or sets an <see cref="int"/> representing the pack major version.
    /// </summary>
    [Reactive]
    public int MajorVersion { get; set; }
    
    /// <summary>
    /// Gets or sets an <see cref="int"/> representing the pack minor version.
    /// </summary>
    [Reactive]
    public int MinorVersion { get; set; }
    
    /// <summary>
    /// Gets or sets an <see cref="int"/> representing the pack build version.
    /// </summary>
    [Reactive]
    public int BuildVersion { get; set; }
    
    /// <summary>
    /// Gets or sets an <see cref="int"/> representing the pack revision version.
    /// </summary>
    [Reactive]
    public int RevisionVersion { get; set; }
}