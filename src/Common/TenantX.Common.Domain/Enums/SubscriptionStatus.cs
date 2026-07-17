namespace TenantX.Common.Domain.Enums;

public enum SubscriptionStatus
{
    Pending = 1,    // Waiting for payment
    Active = 2,     // Access granted
    Canceled = 3,   // Marked to stop at end of period
    Expired = 4,    // Time ran out
    Deactivated = 5 // Force closed (e.g., during an upgrade)
}
