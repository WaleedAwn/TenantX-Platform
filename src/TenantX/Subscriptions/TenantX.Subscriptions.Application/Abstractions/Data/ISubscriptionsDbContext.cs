using Microsoft.EntityFrameworkCore;

using TenantX.Subscriptions.Domain.Plans;
using TenantX.Subscriptions.Domain.Subscriptions;

namespace TenantX.Subscriptions.Application.Abstractions.Data;

public interface ISubscriptionsDbContext
{
        DbSet<Subscription> Subscriptions { get; }
        DbSet<Plan> Plans { get; }

    Task<int> SaveChangesAsync(CancellationToken cts = default);
}
