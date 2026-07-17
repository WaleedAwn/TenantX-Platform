using TenantX.Common.Domain;

namespace TenantX.Subscriptions.Domain.Features;

public record FeatureCreatedDomainEvent(FeatureId FeatureId, FeatureKey Key) : DomainEvent;
public record FeatureArchivedDomainEvent(FeatureId FeatureId) : DomainEvent;
public record FeatureUpdatedDomainEvent(FeatureId FeatureId) : DomainEvent;