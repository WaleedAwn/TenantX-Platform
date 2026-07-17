using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using TenantX.Subscriptions.Application.Abstractions.Data;
using TenantX.Common.Infrastructure.Outbox;
using TenantX.Common.Api.Extensions;
using TenantX.Common.Application;
using TenantX.Subscriptions.Infrastructure.Database;

using AssemblyReference = TenantX.Subscriptions.Presentation.AssemblyReference;

namespace TenantX.Subscriptions.Infrastructure;

public static class SubscriptionsModule
{
    public static IServiceCollection AddSubscriptionsModule(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddEndpoints(Presentation.AssemblyReference.Assembly);
        // services.AddApplication(Application.AssemblyReference.Assembly);
        services.AddApplication([AssemblyReference.Assembly]);

        services.AddSubscriptionsInfrastructure(configuration);

        return services;
    }

    private static IServiceCollection AddSubscriptionsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        string databaseConnectionString = configuration.GetConnectionString("Database")!;

        services.AddDbContext<SubscriptionsDbContext>((sp, options) =>
            options.UseNpgsql(databaseConnectionString,
                npgsqlOptions => npgsqlOptions.MigrationsHistoryTable(
                    HistoryRepository.DefaultTableName,
                    Schemas.Subscriptions)) // Isolated History Table
            .UseSnakeCaseNamingConvention()
        );
        
        services.AddScoped<ISubscriptionsDbContext>(sp => sp.GetRequiredService<SubscriptionsDbContext>());
       
        return services;
    }
}