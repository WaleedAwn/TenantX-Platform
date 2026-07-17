using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FluentValidation;

using Microsoft.EntityFrameworkCore;

using TenantX.Common.Domain.Constants;
using TenantX.Common.Domain.Results;
using TenantX.Subscriptions.Application.Features.Subscriptions.Dtos;

namespace TenantX.Subscriptions.Application.Features.Subscriptions.Commands.InitiateSubscription;

public record InitiateSubscriptionCommand(
    Guid SubscriberId,
    Guid PlanId
    ) : IRequest<Result<InitiateSubscriptionResponse>>;
public sealed class InitiateSubscriptionCommandValidator : AbstractValidator<InitiateSubscriptionCommand>
{
    public InitiateSubscriptionCommandValidator()
    {
        RuleFor(x => x.SubscriberId)
            .NotEmpty()
            .WithErrorCode(DomainErrors.Common.Required);

        RuleFor(x => x.PlanId)
            .NotEmpty()
            .WithErrorCode(DomainErrors.Common.Required);

       
    }
}
