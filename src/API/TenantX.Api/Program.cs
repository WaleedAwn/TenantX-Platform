using Serilog;

using TenantX.Api;
using TenantX.Api.Extensions;
using TenantX.Api.Middlewares;
using TenantX.Common.Api.Extensions;
using TenantX.Subscriptions.Infrastructure;
using TenantX.Tenants.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddProblemDetails();

builder.Services.AddOpenApi();

// modules configuration

// Reuse common infrastructure for web API, authentication, etc.
builder.Services.AddCoreWebApiInfrastructure(builder.Configuration, "Tenants", "Subscriptions");

builder.Configuration.AddModuleConfiguration(["Tenants", "Subscriptions"]);

builder.Services.AddTenantsModule(builder.Configuration);
builder.Services.AddSubscriptionsModule(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();

	app.MapOpenApi();

	app.UseSwaggerUI(options =>
	{
		options.SwaggerEndpoint("/openapi/v1.json", "Baqalati  API");

		options.EnableDeepLinking();
		options.DisplayRequestDuration();
		options.EnableFilter();
	});

	app.ApplyMigrations();
}
// global middlewares
app.UseCoreMiddlewares(app.Environment);

// modules endpoints
app.MapApiEndpoints();

await app.RunAsync();
