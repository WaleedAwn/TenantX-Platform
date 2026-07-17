using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using Modules.Common.Application.Behaviors;

using TenantX.Common.Application.Behaviors;

namespace TenantX.Common.Application;

public static class ApplicationConfiguration
{
	public static IServiceCollection AddApplication(
		this IServiceCollection services,
		Assembly[] moduleAssemblies)
	{
		ArgumentNullException.ThrowIfNull(services);
		ArgumentNullException.ThrowIfNull(moduleAssemblies);

		// 1. Register OpenMediation and scan the current assembly for handlers
		services.AddOpenMediation(moduleAssemblies);

		// 2. Register Pipeline Behaviors
		services.AddOpenMediationBehavior(typeof(UnhandledExceptionBehavior<,>));
		services.AddOpenMediationBehavior(typeof(LoggingBehavior<,>));
		services.AddOpenMediationBehavior(typeof(ValidationBehavior<,>));

		return services;
	}
}
