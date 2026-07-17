namespace TenantX.Subscriptions.Application.Features.Subscriptions.Dtos;

public record ConfirmPaymentResponse(
    Guid SubscriptionId,
    string Status,
    DateTime ActiveUntilUtc);
