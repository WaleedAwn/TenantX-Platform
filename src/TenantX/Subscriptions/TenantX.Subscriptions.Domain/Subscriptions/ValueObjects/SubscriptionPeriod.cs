using TenantX.Common.Domain.Constants;
using TenantX.Common.Domain.Results;
using TenantX.Common.Domain.Shared;

namespace TenantX.Subscriptions.Domain.Subscriptions.ValueObjects;

// <summary>
/// Manages the temporal boundaries of a subscription, including the Tenant's grace period.
/// </summary>
public record SubscriptionPeriod : ValueObject
{
  public DateTime StartDateUtc { get; init; }
  public DateTime EndDateUtc { get; init; }
  public int GracePeriodInDays { get; init; }

  private SubscriptionPeriod(DateTime start, DateTime end, int gracePeriod)
  {
    StartDateUtc = start;
    EndDateUtc = end;
    GracePeriodInDays = gracePeriod;
  }

  public static Result<SubscriptionPeriod> Create(DateTime start, DateTime end, int gracePeriod = 0)
  {
    if (end <= start)
    {
      return SubscriptionErrors.PeriodEndBeforeStart;
    }

    if (gracePeriod < 0)
    {
      return SubscriptionErrors.NegativeGracePeriod;
    }

    return new SubscriptionPeriod(start, end, gracePeriod);
  }

  /// <summary>
  /// Calculates if the user is currently within their paid time or the extra grace period.
  /// </summary>
  public bool HasAccess(DateTime currentUtc)
  {
    var effectiveEndDate = EndDateUtc.AddDays(GracePeriodInDays);
    return currentUtc >= StartDateUtc && currentUtc <= effectiveEndDate;
  }

}
