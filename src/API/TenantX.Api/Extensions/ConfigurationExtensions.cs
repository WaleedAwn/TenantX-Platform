namespace TenantX.Api.Extensions;

internal static class ConfigurationExtensions
{
	internal static void AddModuleConfiguration(this IConfigurationBuilder configurationBuilder, string[] modules)
	{

		foreach (string module in modules)
		{
			configurationBuilder.AddJsonFile($"TenantX.{module}.json", false, true);
			configurationBuilder.AddJsonFile($"TenantX.{module}.Development.json", true, true);

		}
	}
}
