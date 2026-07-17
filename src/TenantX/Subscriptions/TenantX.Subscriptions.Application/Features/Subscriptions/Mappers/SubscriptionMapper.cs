using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TenantX.Subscriptions.Application.Features.Subscriptions.Dtos;
using TenantX.Subscriptions.Domain.Subscriptions;

namespace TenantX.Subscriptions.Application.Features.Subscriptions.Mappers;

public static class SubscriptionMapper
{
    /// <summary>
    /// Maps the initial pending subscription to the initiation response.
    /// Used specifically for the Yemen Wallet flow to provide the PaymentCode.
    /// /// </summary>
    public static InitiateSubscriptionResponse ToInitiateResponse(this Subscription subscription)
    {
        ArgumentNullException.ThrowIfNull(subscription);

        // Security Note: We only map the PaymentCode if it exists (Status is Pending)
        return new InitiateSubscriptionResponse(
            subscription.Id.Value,
            subscription.PaymentCode?.Value ?? string.Empty,
            subscription.PaymentCode?.ExpiresAtUtc ?? DateTime.MinValue
        );
    }

    /// <summary>
    /// Helper to map a list of subscriptions (useful for history queries later)
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    public static List<InitiateSubscriptionResponse> ToInitiateResponses(this IEnumerable<Subscription> entities)
    {
        return [.. entities.Select(e => e.ToInitiateResponse())];
    }
}