using TenantX.Common.Domain.Constants;
using TenantX.Common.Domain.Results;
using TenantX.Common.Domain.Shared;

namespace TenantX.Subscriptions.Domain.Subscriptions.ValueObjects;

/// <summary>
/// Represents the unique code generated for external wallet payments (e.g., Kuraimi, M-Floos).
/// </summary>
public record PaymentCode : ValueObject
{
    public string Value { get; init; }
    public DateTime ExpiresAtUtc { get; init; }

    private PaymentCode(string value, DateTime expiresAtUtc)
    {
        Value = value.Trim();
        ExpiresAtUtc = expiresAtUtc;
    }

    public static Result<PaymentCode> Create(string value, DateTime expiresAtUtc)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return SubscriptionErrors.PaymentCodeRequired;
        }

        return new PaymentCode(value, expiresAtUtc);
    }

    /// <summary>
    /// Checks if the code is still valid for payment confirmation.
    /// </summary>
    public bool IsExpired(DateTime currentUtc) => currentUtc > ExpiresAtUtc;

}