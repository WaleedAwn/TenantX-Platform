using TenantX.Common.Domain.Constants;
using TenantX.Common.Domain.Enums;
using TenantX.Common.Domain.Results;
using TenantX.Common.Domain.Shared;
using TenantX.Common.Domain.Shared.ValueObjects;

namespace TenantX.Subscriptions.Domain.Features;

/// <summary>
/// Represents the definition of a metric or capability (e.g., "max_projects") that can be limited.
/// This aggregate serves as the blueprint for how entitlements are validated within subscription plans.
/// </summary>
/// <remarks>
/// Business Rules:
/// 1. Tenant Isolation: Every feature is owned by a specific Tenant (Tenant-Scoped).
/// 2. Type Safety: The FeatureType (Numeric/Boolean) cannot be changed once created to maintain plan consistency.
/// 3. Lifecycle Protection: If a feature is Archived, it becomes read-only and metadata updates are forbidden.
/// </remarks>
public sealed class Feature : TenantScopedAggregateRoot<FeatureId>
{
  /// <summary>
  /// Friendly name shown in the UI (e.g., "Project Limit").
  /// </summary>
  public DisplayName DisplayName { get; private set; }

  /// <summary>
  /// Unique machine-readable slug (e.g., "max_projects").
  /// </summary>
  public FeatureKey Key { get; private set; }

  /// <summary>
  /// Detailed explanation of what this feature controls.
  /// </summary>
  public Description Description { get; private set; }

  /// <summary>
  /// Determines if the feature value is a checkbox (Boolean) or a counter (Numeric).
  /// </summary>
  public FeatureType Type { get; private set; }

  /// <summary>
  /// If true, the feature is retired. It cannot be edited or added to new plans.
  /// </summary>
  public bool IsArchived { get; private set; }

  // Private constructor for EF Core materialization
#pragma warning disable CS8618
  private Feature() { }
#pragma warning restore CS8618

  private Feature(FeatureId id, FeatureKey key, DisplayName name, Description description, FeatureType type) : base(id)
  {
    Key = key;
    DisplayName = name;
    Description = description;
    Type = type;
    IsArchived = false;
  }

  /// <summary>
  /// Factory method to create a new Feature definition.
  /// </summary>
  /// <param name="key">The validated unique slug key.</param>
  /// <param name="name">The validated display name.</param>
  /// <param name="description">The validated description text.</param>
  /// <param name="type">The type of value this feature supports.</param>
  /// <returns>A Result containing the Feature aggregate.</returns>
  public static Result<Feature> Create(FeatureKey key, DisplayName name, Description description, FeatureType type)
  {
    var feature = new Feature(FeatureId.New(), key, name, description, type);

    feature.RaiseDomainEvent(new FeatureCreatedDomainEvent(feature.Id, feature.Key));

    return feature;
  }

  /// <summary>
  /// Updates the metadata (Name and Description) of the feature.
  /// </summary>
  /// <returns>Updated Result or Conflict if the feature is archived.</returns>
  public Result<Updated> UpdateMetadata(DisplayName newName, Description newDescription)
  {
    if (IsArchived)
    {
      return FeatureErrors.Inactive;
    }

    DisplayName = newName;
    Description = newDescription;

    RaiseDomainEvent(new FeatureUpdatedDomainEvent(Id));

    return Result.Updated;
  }

  /// <summary>
  /// Retires the feature definition. Once archived, it remains for historical lookups 
  /// but cannot be modified or added to new plans.
  /// </summary>
  public void Archive()
  {
    if (IsArchived)
      return;

    IsArchived = true;
    RaiseDomainEvent(new FeatureArchivedDomainEvent(Id));
  }
}