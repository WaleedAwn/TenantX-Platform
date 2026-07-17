using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TenantX.Tenants.Features.Shared.Responses;


namespace TenantX.Tenants.Features.Abstractions;

public interface ITenantQuery
{
    Task<TenantResponse?> GetByIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TenantResponse>> GetAllAsync(CancellationToken cancellationToken = default);

}