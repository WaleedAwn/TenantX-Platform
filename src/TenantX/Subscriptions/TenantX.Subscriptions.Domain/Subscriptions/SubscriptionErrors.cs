using TenantX.Common.Domain.Constants;
using TenantX.Common.Domain.Results;
using TenantX.Common.Domain.Shared;

namespace TenantX.Subscriptions.Domain.Subscriptions;

public static class SubscriptionErrors
{
    // General Entity Errors
    public static readonly Error NotFound =
        Error.NotFound(DomainErrors.Subscriptions.NotFound, "Subscription not found.");

    public static readonly Error AlreadyActive =
        Error.Conflict(DomainErrors.Subscriptions.AlreadyActive, "This subscription is already active.");

    public static readonly Error NotPending =
        Error.Conflict(DomainErrors.Subscriptions.InvalidStatusTransition, "Only pending subscriptions can be confirmed.");

    public static readonly Error CannotCancel =
        Error.Conflict(DomainErrors.Subscriptions.InvalidStatusTransition, "Only active subscriptions can be canceled.");

    public static readonly Error PaymentCodeRequired =
        Error.Validation(DomainErrors.Subscriptions.PaymentCodeInvalid, "Payment code is required.");

    public static readonly Error CodeExpired =
        Error.Validation(DomainErrors.Subscriptions.PaymentCodeExpired, "The payment code has expired. Please initiate a new payment.");

    public static readonly Error InvalidCode =
        Error.Validation(DomainErrors.Subscriptions.PaymentCodeInvalid, "The provided payment code is incorrect.");

    public static readonly Error PeriodEndBeforeStart =
        Error.Validation(DomainErrors.Subscriptions.PeriodInvalid, "End date must be after start date.");

    public static readonly Error NegativeGracePeriod =
        Error.Validation(DomainErrors.Subscriptions.PeriodInvalid, "Grace period cannot be negative.");
}
