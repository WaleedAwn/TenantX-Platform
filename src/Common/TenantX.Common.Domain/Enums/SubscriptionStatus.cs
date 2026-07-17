namespace TenantX.Common.Domain.Enums;

public enum SubscriptionStatus
{
    /// <summary>Waiting for payment code confirmation from wallet/gateway.</summary>
    Pending = 1,
    
    /// <summary>Payment confirmed, access granted.</summary>
    Active = 2,
    
    /// <summary>User stopped renewal; access remains until EndDate + GracePeriod.</summary>
    Canceled = 3,
    
    /// <summary>Access revoked because the period and grace period have ended.</summary>
    Expired = 4,
    
    /// <summary>Forcefully closed (e.g., during a mid-month upgrade to a new plan).</summary>
    Deactivated = 5
}