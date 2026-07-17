using TenantX.Common.Domain.Shared;

namespace TenantX.Subscriptions.Domain.Plans;

public record PlanId(Guid Value) : EntityId<PlanId>(Value)
{
    public static new PlanId New() => new(Guid.NewGuid());
    public static new PlanId From(Guid value) => new(value);
}
