using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using TenantX.Common.Domain.Enums;
using TenantX.Common.Domain.Results;
using TenantX.Subscriptions.Application.Abstractions.Data;
using TenantX.Subscriptions.Domain.Plans;
using TenantX.Subscriptions.Domain.Subscriptions;

namespace TenantX.Subscriptions.Application.Common.Services;

public sealed class SubscriptionService : ISubscriptionService
{
    private readonly ISubscriptionsDbContext _context;

    public SubscriptionService(ISubscriptionsDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Success>> ConfirmSubscriptionPaymentAsync(
        SubscriptionId pendingSubId,
        string providedPaymentCode,
        string externalTransactionId,
        DateTime currentUtc,
        int tenantGracePeriod,
        CancellationToken ct = default)
    {
        // 1. Load the pending subscription directly from the DbSet
        var newSubscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.Id == pendingSubId, ct);

        if (newSubscription is null)
            return SubscriptionErrors.NotFound;

        // 2. Load the Plan using the ID from the subscription
        var plan = await _context.Plans
            .FirstOrDefaultAsync(p => p.Id == newSubscription.PlanId, ct);

        if (plan is null)
            return PlanErrors.NotFound;

        // 3. Domain Logic: Let the Plan calculate the period
        var periodResult = plan.CalculatePeriod(currentUtc, tenantGracePeriod);
        if (periodResult.IsError)
            return periodResult.Errors;

        // 4. THE ATOMIC SWITCH LOGIC:
        // Find if the subscriber already has an active or canceled plan
        var oldSubscription = await _context.Subscriptions
            .Where(s => s.SubscriberId == newSubscription.SubscriberId)
            .Where(s => s.Status == SubscriptionStatus.Active || s.Status == SubscriptionStatus.Canceled)
            .FirstOrDefaultAsync(ct);

        // 5. Deactivate the old plan if it exists
        if (oldSubscription is not null)
        {
            oldSubscription.Deactivate();
            _context.Subscriptions.Update(oldSubscription);
        }

        // 6. Activate the new plan
        var activationResult = newSubscription.ConfirmPayment(
            providedPaymentCode,
            externalTransactionId,
            periodResult.Value,
            currentUtc);

        if (activationResult.IsError)
            return activationResult.Errors;

        // 7. Save changes (This acts as our Unit of Work)
        await _context.SaveChangesAsync(ct);

        return Result.Success;
    }
}
