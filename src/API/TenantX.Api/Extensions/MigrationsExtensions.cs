using Microsoft.EntityFrameworkCore;

using TenantX.Subscriptions.Infrastructure.Database;
using TenantX.Tenants.Infrastructure.Database;

namespace TenantX.Api.Extensions;

internal static class MigrationsExtensions
{
	public static void ApplyMigrations(this IApplicationBuilder app)
	{
		using IServiceScope scope = app.ApplicationServices.CreateScope();
		ApplyMigrations<TenantsDbContext>(scope);


	}

	private static void ApplyMigrations<TDbContext>(IServiceScope scope)
		where TDbContext : DbContext
	{
		using TDbContext context = scope.ServiceProvider.GetRequiredService<TDbContext>();
		context.Database.Migrate();

	}
}
