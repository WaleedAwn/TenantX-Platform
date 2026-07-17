using TenantX.Common.Domain.Shared;

namespace TenantX.Subscriptions.Domain.Subscribers;

public record SubscriberId(Guid Value) : EntityId<SubscriberId>(Value)
{
  public static new SubscriberId New() => new(Guid.NewGuid());
  public static new SubscriberId From(Guid value) => new(value);
}
