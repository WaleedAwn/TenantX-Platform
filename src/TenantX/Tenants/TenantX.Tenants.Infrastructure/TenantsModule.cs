using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using TenantX.Common.Api.Extensions;
using TenantX.Common.Application;

using TenantX.Tenants.Features.Abstractions;
using TenantX.Tenants.Features.Abstractions.Data;
using TenantX.Tenants.Infrastructure.Database;

using TenantX.Tenants.Infrastructure.Quires;

namespace TenantX.Tenants.Infrastructure;


public static class TenantsModule
{

	public static IServiceCollection AddTenantsModule(this IServiceCollection services, IConfiguration configuration)
	{
		// Commong API configurations
		services.AddEndpoints(Features.AssemblyReference.Assembly);

		// Common Application configurations
		services.AddApplication([Features.AssemblyReference.Assembly]);

		services.AddTenantsInfrastructure(configuration);

		return services;
	}

	private static IServiceCollection AddTenantsInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		string databaseConnectionString = configuration.GetConnectionString("Database")!;
		// Common infrastructure configurations
		services.AddDatabaseQueries();

		services.AddDbContext<TenantsDbContext>((sp, options) =>
			options.UseNpgsql(databaseConnectionString,
			npgsqlOptions => npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Tenants))
			.UseSnakeCaseNamingConvention()
		// .AddInterceptors(auditableInterceptor)

		);

		services.AddScoped<ITenantsDbContext>(c => c.GetRequiredService<TenantsDbContext>());

		return services;
	}

	private static IServiceCollection AddDatabaseQueries(this IServiceCollection services)
	{
		services.TryAddScoped<ITenantQuery, TenantQuery>();

		return services;
	}
}
