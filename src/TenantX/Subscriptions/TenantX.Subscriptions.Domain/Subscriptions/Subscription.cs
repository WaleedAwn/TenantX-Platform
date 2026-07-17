using TenantX.Common.Domain.Enums;
using TenantX.Common.Domain.Results;
using TenantX.Common.Domain.Shared;
using TenantX.Subscriptions.Domain.Plans;
using TenantX.Subscriptions.Domain.Subscribers;
using TenantX.Subscriptions.Domain.Subscriptions.Events;
using TenantX.Subscriptions.Domain.Subscriptions.ValueObjects;

namespace TenantX.Subscriptions.Domain.Subscriptions;

/// <summary>
/// Represents the live contract between a Subscriber and a Plan.
/// This aggregate manages the lifecycle of access, payment verification, and time-based expiration.
/// </summary>
/// <remarks>
/// <b>Main Responsibilities:</b>
/// 1. <b>State Machine:</b> Manages transitions (Pending -> Active -> Canceled -> Expired).
/// 2. <b>Wallet Flow:</b> Validates short-lived Payment Codes for external wallet systems.
/// 3. <b>Access Authority:</b> Determines if a user has access today, including grace periods.
/// </remarks>
public sealed class Subscription : TenantScopedAggregateRoot<SubscriptionId>
{
  public SubscriberId SubscriberId { get; private set; }
  public PlanId PlanId { get; private set; }
  public SubscriptionStatus Status { get; private set; }

  /// <summary>
  /// The temporary code used by the user in the Wallet (e.g. Kuraimi) to pay.
  /// Cleared once the subscription is activated.
  /// </summary>
  public PaymentCode? PaymentCode { get; private set; }

  /// <summary>
  /// The final transaction reference returned by the payment provider (Wallet/Bank).
  /// </summary>
  public string? ExternalTransactionId { get; private set; }

  /// <summary>
  /// The defined timeframe for this subscription, including tenant-specific extra days.
  /// Null until the subscription is activated.
  /// </summary>
  public SubscriptionPeriod? Period { get; private set; }

  // Required for EF Core
#pragma warning disable CS8618
  private Subscription() { }
#pragma warning restore CS8618

  private Subscription(
      SubscriptionId id,
      SubscriberId subscriberId,
      PlanId planId,
      PaymentCode? paymentCode) : base(id)
  {
    SubscriberId = subscriberId;
    PlanId = planId;
    Status = SubscriptionStatus.Pending;
    PaymentCode = paymentCode;
  }

  /// <summary>
  /// Initiates a new subscription in the PENDING state.
  /// This generates the requirement for a wallet payment.
  /// </summary>
  public static Subscription Initiate(SubscriberId subscriberId, PlanId planId, PaymentCode paymentCode)
  {
    var subscription = new Subscription(SubscriptionId.New(), subscriberId, planId, paymentCode);

    subscription.RaiseDomainEvent(new SubscriptionInitiatedDomainEvent(subscription.Id, subscriberId));

    return subscription;
  }

  /// <summary>
  /// Business Rule: Confirms payment from an external wallet and grants access.
  /// </summary>
  /// <param name="providedCode">The code the user typed into the wallet.</param>
  /// <param name="transactionRef">The reference ID from the Wallet provider.</param>
  /// <param name="activatedPeriod">The calculated start/end dates for this plan.</param>
  /// <param name="currentUtc">The current system time.</param>
  public Result<Success> ConfirmPayment(
      string providedCode,
      string transactionRef,
      SubscriptionPeriod activatedPeriod,
      DateTime currentUtc)
  {
    // 1. Invariant: Only PENDING subscriptions can be activated
    if (Status != SubscriptionStatus.Pending)
      return SubscriptionErrors.NotPending;

    // 2. Invariant: Payment Code must exist
    if (PaymentCode == null)
      return SubscriptionErrors.InvalidCode;

    // 3. Invariant: Code must match
    if (PaymentCode.Value != providedCode)
      return SubscriptionErrors.InvalidCode;

    // 4. Invariant: Code must not be expired (Yemen Wallet Timeout)
    if (PaymentCode.IsExpired(currentUtc))
      return SubscriptionErrors.CodeExpired;

    // Success Transition
    Status = SubscriptionStatus.Active;
    ExternalTransactionId = transactionRef;
    Period = activatedPeriod;
    // Cleanup: Remove sensitive payment code data once used
    PaymentCode = null;

    RaiseDomainEvent(new SubscriptionActivatedDomainEvent(Id, PlanId));

    return Result.Success;
  }

  /// <summary>
  /// Business Rule: User stops auto-renewal. Access remains until the end of the current period.
  /// </summary>
  public Result<Success> CancelRenewal()
  {
    if (Status != SubscriptionStatus.Active)
      return SubscriptionErrors.CannotCancel;

    Status = SubscriptionStatus.Canceled;

    RaiseDomainEvent(new SubscriptionCanceledDomainEvent(Id));

    return Result.Success;
  }

  /// <summary>
  /// If a user canceled their renewal but changes their mind before the period ends.
  /// </summary>
  public Result<Success> ReactivateRenewal()
  {
    if (Status != SubscriptionStatus.Canceled)
      return Error.Conflict("Subscription.NotCanceled", "Only canceled subscriptions can be reactivated.");

    Status = SubscriptionStatus.Active;
    // Raise event if needed
    return Result.Success;
  }
  /// <summary>
  /// Business Rule: Naturally ends the subscription when time runs out.
  /// </summary>
  public void Expire()
  {
    if (Status is SubscriptionStatus.Expired or SubscriptionStatus.Deactivated)
      return;

    Status = SubscriptionStatus.Expired;
    RaiseDomainEvent(new SubscriptionExpiredDomainEvent(Id));
  }

  /// <summary>
  /// Used when a payment code expires without being used or is manually rejected.
  /// </summary>
  public void FailPayment()
  {
    if (Status != SubscriptionStatus.Pending)
      return;

    Status = SubscriptionStatus.Deactivated;
    PaymentCode = null;
  }
  /// <summary>
  /// Business Rule: Forcefully closes the subscription (used during Plan Upgrades/Atomic Switch).
  /// </summary>
  public void Deactivate()
  {
    if (Status == SubscriptionStatus.Deactivated)
      return;

    Status = SubscriptionStatus.Deactivated;
    RaiseDomainEvent(new SubscriptionDeactivatedDomainEvent(Id));
  }

  /// <summary>
  /// The ultimate authority for access. 
  /// Accounts for Status, EndDate, and the Tenant's Grace Period.
  /// </summary>
  public bool IsActiveAndValid(DateTime currentUtc)
  {
    // Must be in a status that allows access
    if (Status != SubscriptionStatus.Active && Status != SubscriptionStatus.Canceled)
      return false;

    // Must have an active period defined
    if (Period == null)
      return false;

    // Use the Value Object to check if we are within the timeline + grace period
    return Period.HasAccess(currentUtc);
  }
}
