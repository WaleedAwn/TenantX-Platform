using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TenantX.Common.Domain.Results;

namespace TenantX.Subscriptions.Application.Common.Queries;

/// <summary>
/// Dapper-based read model for high-performance queries.
/// </summary>
public interface ISubscriptionQueries
{
    /// <summary>
    /// The "Heartbeat" query: Returns all effective entitlements for a user right now.
    /// Used by the Tenant App to enforce limits.
    /// /// </summary>
    Task<Result<IReadOnlyDictionary<string, string>>> GetActiveEntitlementsAsync(
        Guid externalUserId, 
        Guid tenantId, 
        CancellationToken ct = default);

    Task<Result<SubscriptionDetailsResponse>> GetSubscriptionDetailsAsync(Guid subscriptionId);
}

// Simple DTO for the Query
public record SubscriptionDetailsResponse(
    Guid Id,
    string Status,
    string PlanName,
    decimal Price,
    string Currency,
    DateTime? EndDate);