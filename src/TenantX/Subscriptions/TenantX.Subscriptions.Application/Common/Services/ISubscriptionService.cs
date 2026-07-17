using TenantX.Common.Domain.Results;
using TenantX.Subscriptions.Domain.Subscriptions;

namespace TenantX.Subscriptions.Application.Common.Services;

public interface ISubscriptionService
{
  /// <summary>
  /// Coordinates the deactivation of an old plan and activation of a new one.
  /// This is the "Atomic Switch" logic.
  /// </summary>
  Task<Result<Success>> ConfirmSubscriptionPaymentAsync(
      SubscriptionId pendingSubId,
      string providedPaymentCode,
      string externalTransactionId,
      DateTime currentUtc,
      int tenantGracePeriod,
      CancellationToken ct = default);
}
