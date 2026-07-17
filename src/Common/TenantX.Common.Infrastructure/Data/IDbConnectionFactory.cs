using System.Data.Common;

namespace TenantX.Common.Infrastructure.Data;

public interface IDbConnectionFactory
{
	ValueTask<DbConnection> OpenConnectionAsync();
}
