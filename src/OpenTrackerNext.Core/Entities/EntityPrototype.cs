using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Document;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.Entities;

/// <summary>
/// Represents an <see cref="IEntity"/> within the editor.
/// </summary>
[Document]
public sealed partial class EntityPrototype : ReactiveObject, ITitledDocumentData<EntityPrototype>
{
    /// <summary>
    /// A <see cref="string"/> representing the title prefix for all entity prototypes.
    /// </summary>
    public const string EntityTitlePrefix = "Entity - ";

    /// <inheritdoc />
    public static string TitlePrefix => EntityTitlePrefix;

    /// <summary>
    /// Gets or sets an <see cref="int"/> representing the minimum state value.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public int Minimum { get; set; }
    
    /// <summary>
    /// Gets or sets an <see cref="int"/> representing the starting state value.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public int Starting { get; set; }

    /// <summary>
    /// Gets or sets an <see cref="int"/> representing the maximum state value.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public int Maximum { get; set; } = 1;
}