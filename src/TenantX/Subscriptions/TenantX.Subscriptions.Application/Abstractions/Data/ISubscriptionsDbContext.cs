namespace TenantX.Subscriptions.Application.Abstractions.Data;

public interface ISubscriptionsDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cts = default);
}
