using TenantX.Common.Domain.Shared;

namespace TenantX.Subscriptions.Domain.Subscriptions;

public record SubscriptionId(Guid Value) : EntityId<SubscriptionId>(Value)
{
    public static new SubscriptionId New() => new(Guid.NewGuid());
    public static new SubscriptionId From(Guid value) => new(value);
}
