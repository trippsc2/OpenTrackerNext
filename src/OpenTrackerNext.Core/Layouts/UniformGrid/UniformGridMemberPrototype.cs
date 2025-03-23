using OpenTrackerNext.Document;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Core.Layouts.UniformGrid;

/// <summary>
/// Represents a member of a <see cref="UniformGridLayout"/> within the editor.
/// </summary>
[Document]
public sealed partial class UniformGridMemberPrototype : ReactiveObject
{
    /// <summary>
    /// Gets or sets an <see cref="LayoutId"/> representing the layout of the member.
    /// </summary>
    [DocumentMember]
    [Reactive]
    public LayoutId Layout { get; set; }
}