using Microsoft.EntityFrameworkCore;

using TenantX.Common.Application.Clock;
using TenantX.Common.Domain.Constants;
using TenantX.Common.Domain.Results;
using TenantX.Subscriptions.Application.Abstractions.Data;
using TenantX.Subscriptions.Application.Abstractions.Services;
using TenantX.Subscriptions.Application.Features.Subscriptions.Dtos;
using TenantX.Subscriptions.Application.Features.Subscriptions.Mappers;
using TenantX.Subscriptions.Domain.Plans;
using TenantX.Subscriptions.Domain.Subscribers;
using TenantX.Subscriptions.Domain.Subscriptions;
using TenantX.Subscriptions.Domain.Subscriptions.ValueObjects;

namespace TenantX.Subscriptions.Application.Features.Subscriptions.Commands.InitiateSubscription;

public sealed class InitiateSubscriptionCommandHandler
    : IRequestHandler<InitiateSubscriptionCommand, Result<InitiateSubscriptionResponse>>
{
  private readonly ISubscriptionsDbContext _context;
  private readonly IDateTimeProvider _dateTimeProvider;
  private readonly IPaymentCodeGenerator _paymentCodeGenerator;

  public InitiateSubscriptionCommandHandler(
      ISubscriptionsDbContext context,
      IDateTimeProvider dateTimeProvider,
      IPaymentCodeGenerator paymentCodeGenerator)
  {
    _context = context;
    _dateTimeProvider = dateTimeProvider;
    _paymentCodeGenerator = paymentCodeGenerator;
  }

  public async Task<Result<InitiateSubscriptionResponse>> Handle(
      InitiateSubscriptionCommand request,
      CancellationToken cancellationToken)
  {
    // 1. Validate Plan existence and availability
    var plan = await _context.Plans
        .FirstOrDefaultAsync(p => p.Id == PlanId.From(request.PlanId), cancellationToken);

    if (plan is null)
      return PlanErrors.NotFound;

    if (plan.IsArchived)
      return PlanErrors.Archived;

    // 2. Validate Subscriber existence
    var subscriberExists = await _context.Subscribers
        .AnyAsync(s => s.Id == SubscriberId.From(request.SubscriberId), cancellationToken);

    if (!subscriberExists)
      return SubscriberErrors.NotFound;

    string codeValue = _paymentCodeGenerator.GenerateCode(ValidationConstants.Subscriptions.PaymentCodeLength);
    DateTime expiry = _dateTimeProvider.UtcNow.AddMinutes(ValidationConstants.Subscriptions.DefaultPaymentCodeExpiryMinutes);

    var paymentCodeResult = PaymentCode.Create(codeValue, expiry);
    if (paymentCodeResult.IsError)
      return paymentCodeResult.Errors;

    // 4. Create the Domain Aggregate
    var subscription = Subscription.Initiate(
        SubscriberId.From(request.SubscriberId),
        plan.Id,
        paymentCodeResult.Value);

    _context.Subscriptions.Add(subscription);
    await _context.SaveChangesAsync(cancellationToken);

    return subscription.ToInitiateResponse();
  }
}
