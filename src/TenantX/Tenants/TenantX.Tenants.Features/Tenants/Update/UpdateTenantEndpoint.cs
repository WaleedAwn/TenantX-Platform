using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

using TenantX.Common.Api.Abstractions;
using TenantX.Common.Api.Extensions;

namespace TenantX.Tenants.Features.Tenants.Update;

public sealed class UpdateTenantEndpoint : IApiEndpoint
{

    public void MapEndpoint(WebApplication app)
    {
        app.MapPut("api/tenants/{id:guid}", async (Guid id, UpdateTenantRequest request, ISender sender) =>
              {
                  var command = new UpdateTenantCommand(id, request.Name);
                  var result = await sender.Send(command);

                  return result.Match(
                      _ => Results.NoContent(), // 204 No Content is standard for successful PUT
                      errors => errors.ToProblem());
              })
              .WithTags(Tags.Tenants);
    }
}

public record UpdateTenantRequest(string Name);