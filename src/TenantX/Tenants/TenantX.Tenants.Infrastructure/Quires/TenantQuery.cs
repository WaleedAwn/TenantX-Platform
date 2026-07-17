using System.Data.Common;

using Dapper;

using TenantX.Common.Infrastructure.Data;
using TenantX.Tenants.Features.Abstractions;
using TenantX.Tenants.Features.Shared.Responses;


namespace TenantX.Tenants.Infrastructure.Quires;

internal sealed class TenantQuery(IDbConnectionFactory dbConnectionFactory) : ITenantQuery
{
    private readonly IDbConnectionFactory _dbConnectionFactory = dbConnectionFactory;

    public async Task<TenantResponse?> GetByIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        await using DbConnection connection = await _dbConnectionFactory.OpenConnectionAsync();

        // Using your preferred nameof() style for robust aliasing
        const string sql =
            $"""
             SELECT 
                 id AS {nameof(TenantResponse.Id)},
                 name AS {nameof(TenantResponse.Name)},
                 api_key AS {nameof(TenantResponse.ApiKey)},
                 is_active AS {nameof(TenantResponse.IsActive)},
                 created_at AS {nameof(TenantResponse.CreatedAt)}
             FROM tenants.tenants
             WHERE id = @TenantId
             """;

        return await connection.QueryFirstOrDefaultAsync<TenantResponse>(sql, new { TenantId = tenantId });
    }

    public async Task<IReadOnlyList<TenantResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using DbConnection connection = await _dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            $"""
             SELECT 
                 id AS {nameof(TenantResponse.Id)},
                 name AS {nameof(TenantResponse.Name)},
                 api_key AS {nameof(TenantResponse.ApiKey)},
                 is_active AS {nameof(TenantResponse.IsActive)},
                 created_at AS {nameof(TenantResponse.CreatedAt)}
             FROM tenants.tenants
             """;

        var tenants = await connection.QueryAsync<TenantResponse>(sql);

        return tenants.AsList();
    }
}