using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

using TenantX.Common.Api.Abstractions;
using TenantX.Common.Api.Extensions;

namespace TenantX.Tenants.Features.Tenants.Create;

public sealed class CreateTenantEndpoint : IApiEndpoint
{

    public void MapEndpoint(WebApplication app)
    {
        app.MapPost("api/tenants", async (CreateTenantRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateTenantCommand(request.Name));

            return result.Match(
                _ => Results.Created($"/api/tenants", null),
                errors => errors.ToProblem());
        })
        .WithTags(Tags.Tenants);
    }
}

public record CreateTenantRequest(string Name);