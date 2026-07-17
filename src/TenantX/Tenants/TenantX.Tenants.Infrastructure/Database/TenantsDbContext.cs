
using Microsoft.EntityFrameworkCore;

using TenantX.Common.Domain;

using TenantX.Tenants.Infrastructure.Database.Configurations;
using TenantX.Tenants.Domain.Tenants;
using TenantX.Tenants.Features.Abstractions.Data;

namespace TenantX.Tenants.Infrastructure.Database;

public sealed class TenantsDbContext(DbContextOptions<TenantsDbContext> options) : DbContext(options), ITenantsDbContext
{
	public DbSet<Tenant> Tenants { get; set; }


	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.HasDefaultSchema(Schemas.Tenants);
		modelBuilder.Ignore<DomainEvent>();

		modelBuilder.ApplyConfigurationsFromAssembly(typeof(TenantConfiguration).Assembly);

		base.OnModelCreating(modelBuilder);
	}
	public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		return await base.SaveChangesAsync(cancellationToken);
	}
}
