using TenantX.Subscriptions.Application.Features.Subscriptions.Dtos;
using TenantX.Subscriptions.Domain.Subscriptions;

namespace TenantX.Subscriptions.Application.Features.Subscriptions.Mappers;

public static class ConfirmPaymentMapper
{
  public static ConfirmPaymentResponse ToConfirmResponse(this Subscription subscription)
  {
    ArgumentNullException.ThrowIfNull(subscription);

    return new ConfirmPaymentResponse(
        subscription.Id.Value,
        subscription.Status.ToString(),
        subscription.Period?.EndDateUtc ?? DateTime.MinValue
    );
  }
  public static List<ConfirmPaymentResponse> ToConfirmResponses(this IEnumerable<Subscription> entities)
  {
    return [.. entities.Select(e => e.ToConfirmResponse())];
  }
}
