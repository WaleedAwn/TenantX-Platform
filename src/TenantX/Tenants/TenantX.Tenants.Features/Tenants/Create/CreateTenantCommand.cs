using FluentValidation;

using TenantX.Common.Domain.Constants;
using TenantX.Common.Domain.Results;
using TenantX.Tenants.Domain.Tenants;
using TenantX.Tenants.Domain.Tenants.valueObjects;
using TenantX.Tenants.Features.Abstractions.Data;

namespace TenantX.Tenants.Features.Tenants.Create;

public sealed record CreateTenantCommand(string Name) : IRequest<Result<Created>>;

public sealed class CreateTenantCommandValidator : AbstractValidator<CreateTenantCommand>
{
    public CreateTenantCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithErrorCode(DomainErrors.Common.Required)
            .MaximumLength(ValidationConstants.Names.MaxLength)
            .WithErrorCode(DomainErrors.Common.TooLong);
    }
}
internal sealed class CreateTenantCommandHandler(ITenantsDbContext context)
    : IRequestHandler<CreateTenantCommand, Result<Created>>
{
    private readonly ITenantsDbContext _context = context;

    public async Task<Result<Created>> Handle(
        CreateTenantCommand request,
        CancellationToken cancellationToken)
    {
        var nameResult = TenantName.Create(request.Name);
        if (nameResult.IsError)
            return nameResult.Errors;

        var tenantResult = Tenant.Create(nameResult.Value);
        if (tenantResult.IsError)
            return tenantResult.Errors;

        _context.Tenants.Add(tenantResult.Value);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Created;
    }
}