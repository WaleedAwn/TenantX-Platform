using System.Reflection;

namespace TenantX.Tenants.Features;

public static class AssemblyReference
{
	public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
