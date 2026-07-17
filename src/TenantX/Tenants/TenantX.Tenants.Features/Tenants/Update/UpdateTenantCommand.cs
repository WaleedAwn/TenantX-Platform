using FluentValidation;

using Microsoft.EntityFrameworkCore;

using TenantX.Common.Domain.Constants;
using TenantX.Common.Domain.Results;
using TenantX.Common.Domain.Shared;
using TenantX.Tenants.Domain.Tenants.valueObjects;
using TenantX.Tenants.Features.Abstractions.Data;

namespace TenantX.Tenants.Features.Tenants.Update;

public sealed record UpdateTenantCommand(Guid Id, string Name) : IRequest<Result<Updated>>;
public sealed class UpdateTenantCommandValidator : AbstractValidator<UpdateTenantCommand>
{
    public UpdateTenantCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name)
            .NotEmpty().WithErrorCode(DomainErrors.Common.Required)
            .MaximumLength(ValidationConstants.Names.MaxLength).WithErrorCode(DomainErrors.Common.TooLong);
    }
}
internal sealed class UpdateTenantCommandHandler(ITenantsDbContext context)
        : IRequestHandler<UpdateTenantCommand, Result<Updated>>
{

    public async Task<Result<Updated>> Handle(UpdateTenantCommand request, CancellationToken cancellationToken)
    {
        var tenant = await context.Tenants
        .FirstOrDefaultAsync(t => t.Id == TenantId.From(request.Id), cancellationToken);

        if (tenant is null)
            return Error.NotFound(DomainErrors.Tenant.NotFound, "Tenant not found");

        var nameResult = TenantName.Create(request.Name);
        if (nameResult.IsError)
            return nameResult.Errors;

        // 3. Execute Domain Logic
        var updateResult = tenant.UpdateName(nameResult.Value);

        // 4. Save
        await context.SaveChangesAsync(cancellationToken);

        return updateResult;
    }
}