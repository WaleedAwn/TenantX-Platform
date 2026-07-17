using Microsoft.EntityFrameworkCore;

using TenantX.Tenants.Domain.Tenants;

namespace TenantX.Tenants.Features.Abstractions.Data;

public interface ITenantsDbContext
{
    DbSet<Tenant> Tenants { get; }
    Task<int> SaveChangesAsync(CancellationToken cts = default);
}
