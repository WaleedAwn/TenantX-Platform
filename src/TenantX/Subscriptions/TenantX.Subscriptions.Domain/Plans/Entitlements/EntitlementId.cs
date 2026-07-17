using TenantX.Common.Domain.Shared;

namespace TenantX.Subscriptions.Domain.Plans.Entitlements;

public record EntitlementId(Guid Value) : EntityId<EntitlementId>(Value)
{
  public static new EntitlementId New() => new(Guid.NewGuid());
  public static new EntitlementId From(Guid value) => new(value);
}
