using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenantX.Tenants.Features.Shared.Responses;

public sealed record TenantResponse(
    Guid Id,
    string Name,
    string ApiKey,
    bool IsActive,
    DateTime CreatedAt);