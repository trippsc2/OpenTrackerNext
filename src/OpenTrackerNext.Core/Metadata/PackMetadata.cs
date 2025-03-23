using System;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Document;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.Metadata;

/// <summary>
/// Represents the metadata for a pack.
/// </summary>
[Document]
public sealed partial class PackMetadata : ReactiveObject, ITitledDocumentData<PackMetadata>
{
    /// <summary>
    /// A <see cref="string"/> representing the title for the metadata document.
    /// </summary>
    public const string MetadataTitle = "Metadata";

    /// <inheritdoc/>
    public static string TitlePrefix => MetadataTitle;
    
    /// <summary>
    /// Gets or sets a <see cref="string"/> representing the name of the pack.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets a <see cref="string"/> representing the author of the pack.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public string Author { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets a <see cref="Version"/> representing the version of the pack.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public Version Version { get; set; } = new(1, 0, 0, 0);
}