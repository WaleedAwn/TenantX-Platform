using System.Reflection;

namespace TenantX.Subscriptions.Application;

public static class AssemblyReference
{
	public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
