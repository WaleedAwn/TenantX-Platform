namespace TenantX.Common.Domain.Shared;
// <summary>
/// Provides access to the security context of the user currently performing the action.
/// This data is typically extracted from the JWT Claims or Session.
/// </summary>
public interface ICurrentUserContext
{
    /// <summary>
    /// The unique identifier of the authenticated user.
    /// </summary>
    UserId UserId { get; }

    /// <summary>
    /// The current Tenant the user is acting within.
    /// Necessary for data isolation in the multi-tenant model.
    /// </summary>
    TenantId TenantId { get; }


    /// <summary>
    /// Returns true if the request is being made by an authenticated user.
    /// </summary>
    bool IsAuthenticated { get; }
}