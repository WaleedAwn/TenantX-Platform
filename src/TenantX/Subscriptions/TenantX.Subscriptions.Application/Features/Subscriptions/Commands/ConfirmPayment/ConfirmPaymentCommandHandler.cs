using Microsoft.EntityFrameworkCore;

using TenantX.Common.Application.Clock;
using TenantX.Common.Domain.Constants;
using TenantX.Common.Domain.Enums;
using TenantX.Common.Domain.Results;
using TenantX.Subscriptions.Application.Abstractions.Data;
using TenantX.Subscriptions.Application.Features.Subscriptions.Dtos;
using TenantX.Subscriptions.Application.Features.Subscriptions.Mappers;
using TenantX.Subscriptions.Domain.Plans;
using TenantX.Subscriptions.Domain.Subscribers;
using TenantX.Subscriptions.Domain.Subscriptions;

namespace TenantX.Subscriptions.Application.Features.Subscriptions.Commands.ConfirmPayment;

public sealed class ConfirmPaymentCommandHandler
    : IRequestHandler<ConfirmPaymentCommand, Result<ConfirmPaymentResponse>>
{
  private readonly ISubscriptionsDbContext _context;
  private readonly IDateTimeProvider _dateTimeProvider;

  public ConfirmPaymentCommandHandler(
      ISubscriptionsDbContext context,
      IDateTimeProvider dateTimeProvider)
  {
    _context = context;
    _dateTimeProvider = dateTimeProvider;
  }

  public async Task<Result<ConfirmPaymentResponse>> Handle(
      ConfirmPaymentCommand request,
      CancellationToken cancellationToken)
  {
    var result = await ConfirmSubscriptionPaymentAsync(request, cancellationToken);

    if (result.IsError)
      return result.Errors;

    await _context.SaveChangesAsync(cancellationToken);

    return result.Value.ToConfirmResponse();
  }


  private async Task<Result<Subscription>> ConfirmSubscriptionPaymentAsync(
      ConfirmPaymentCommand request,
      CancellationToken ct)
  {
    DateTime utcNow = _dateTimeProvider.UtcNow;

    // Step 1: Load necessary data
    var subscription = await _context.Subscriptions
        .FirstOrDefaultAsync(s => s.Id == SubscriptionId.From(request.SubscriptionId), ct);

    if (subscription is null)
      return SubscriptionErrors.NotFound;

    var plan = await _context.Plans
        .FirstOrDefaultAsync(p => p.Id == subscription.PlanId, ct);
    if (plan is null)
      return PlanErrors.NotFound;

    // Step 2: Aggregate Logic - Calculate Timeline
    var periodResult = plan.CalculatePeriod(utcNow, ValidationConstants.Subscriptions.DefaultGracePeriodInDays);
    if (periodResult.IsError)
      return periodResult.Errors;

    // Step 3: Atomic Switch - Deactivate existing active plans
    await DeactivateActiveSubscriptionsAsync(subscription.SubscriberId, ct);

    // Step 4: Aggregate Logic - Confirm Payment
    var activationResult = subscription.ConfirmPayment(
        request.PaymentCode,
        request.ExternalTransactionId,
        periodResult.Value,
        utcNow);

    if (activationResult.IsError)
      return activationResult.Errors;

    return subscription;
  }

  /// <summary>
  /// Finds all currently valid plans and shuts them down to make room for the new one.
  /// </summary>
  private async Task DeactivateActiveSubscriptionsAsync(SubscriberId subscriberId, CancellationToken ct)
  {
    var existingActive = await _context.Subscriptions
        .Where(s => s.SubscriberId == subscriberId)
        .Where(s => s.Status == SubscriptionStatus.Active || s.Status == SubscriptionStatus.Canceled)
        .ToListAsync(ct);

    foreach (var oldSub in existingActive)
    {
      oldSub.Deactivate();
      _context.Subscriptions.Update(oldSub);
    }
  }
}
