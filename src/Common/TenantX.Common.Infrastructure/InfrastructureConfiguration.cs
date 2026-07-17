using MassTransit;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Npgsql;

using Quartz;

using TenantX.Common.Application.Clock;
using TenantX.Common.Application.EventBus;
using TenantX.Common.Domain.Shared;
using TenantX.Common.Infrastructure.Clock;
using TenantX.Common.Infrastructure.Data;
using TenantX.Common.Infrastructure.EventBuses;
using TenantX.Common.Infrastructure.Interceptors;
using TenantX.Common.Infrastructure.Outbox;
using TenantX.Common.Infrastructure.Services;

namespace TenantX.Common.Infrastructure;


public static class InfrastructureConfiguration
{
	public static IServiceCollection AddCoreInfrastructure(this IServiceCollection services
	, string databaseConnectionString)
	{

		NpgsqlDataSource npgsqlDataSource = new NpgsqlDataSourceBuilder(databaseConnectionString).Build();

		services.TryAddSingleton(npgsqlDataSource);

		services.AddScoped<IDbConnectionFactory, DBConnectionFactory>();
		services.TryAddSingleton<IDateTimeProvider, DateTimeProvider>();
		// user context
		services.AddHttpContextAccessor();
		services.TryAddScoped<ICurrentUserContext, CurrentUserContext>();

		services.TryAddSingleton<InsertOutboxMessagesInterceptor>();
		services.TryAddScoped<AuditableEntityInterceptor>();

		// Quartz configuration with unique scheduler ID and name to avoid conflicts in tests
		services.AddQuartz(configurator =>
		{
			var scheduler = Guid.NewGuid();
			configurator.SchedulerId = $"default-id-{scheduler}";
			configurator.SchedulerName = $"default-name-{scheduler}";
		});

		services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);


		services.TryAddSingleton<IEventBus, EventBus>();

		services.AddMassTransit(configure =>
		{
			//foreach (Action<IRegistrationConfigurator> configureConsumer in moduleConfigureConsumers)
			//{
			//	configureConsumer(configure);
			//}

			configure.SetKebabCaseEndpointNameFormatter();
			configure.UsingInMemory((context, cfg) =>
			{
				cfg.ConfigureEndpoints(context);
			});
		});

		return services;
	}
}
