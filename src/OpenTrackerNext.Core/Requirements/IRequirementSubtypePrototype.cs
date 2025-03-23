using System.ComponentModel;
using System.Text.Json.Serialization;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Requirements.Aggregate;
using OpenTrackerNext.Core.Requirements.Alternative;
using OpenTrackerNext.Core.Requirements.Entity.AtLeast;
using OpenTrackerNext.Core.Requirements.Entity.AtMost;
using OpenTrackerNext.Core.Requirements.Entity.Exact;
using OpenTrackerNext.Core.Requirements.Entity.Not;
using OpenTrackerNext.Core.Requirements.UIPanel;

namespace OpenTrackerNext.Core.Requirements;

/// <summary>
/// Represents a specific <see cref="IRequirement"/> subtype within the editor.
/// </summary>
[JsonDerivedType(typeof(NullRequirementPrototype), typeDiscriminator: "null")]
[JsonDerivedType(typeof(AggregateRequirementPrototype), typeDiscriminator: "aggregate")]
[JsonDerivedType(typeof(AlternativeRequirementPrototype), typeDiscriminator: "alternative")]
[JsonDerivedType(typeof(EntityAtLeastRequirementPrototype), typeDiscriminator: "entity_at_least")]
[JsonDerivedType(typeof(EntityAtMostRequirementPrototype), typeDiscriminator: "entity_at_most")]
[JsonDerivedType(typeof(EntityExactRequirementPrototype), typeDiscriminator: "entity_exact")]
[JsonDerivedType(typeof(EntityNotRequirementPrototype), typeDiscriminator: "entity_not")]
[JsonDerivedType(typeof(UIPanelDockRequirementPrototype), typeDiscriminator: "ui_panel_dock")]
public interface IRequirementSubtypePrototype : IDocumentData, INotifyPropertyChanged;