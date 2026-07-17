using TenantX.Common.Application.Clock;

namespace TenantX.Common.Infrastructure.Clock;

internal sealed class DateTimeProvider : IDateTimeProvider
{
	public DateTime UtcNow => DateTime.UtcNow;
}
