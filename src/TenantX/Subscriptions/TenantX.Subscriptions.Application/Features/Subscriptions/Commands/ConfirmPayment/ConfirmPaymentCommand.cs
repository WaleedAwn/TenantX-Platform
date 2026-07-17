using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FluentValidation;

using TenantX.Common.Domain.Constants;
using TenantX.Common.Domain.Results;
using TenantX.Subscriptions.Application.Features.Subscriptions.Dtos;

namespace TenantX.Subscriptions.Application.Features.Subscriptions.Commands.ConfirmPayment;

public record ConfirmPaymentCommand(
Guid SubscriptionId,
string PaymentCode,
string ExternalTransactionId) : IRequest<Result<ConfirmPaymentResponse>>;
public sealed class ConfirmPaymentCommandValidator : AbstractValidator<ConfirmPaymentCommand>
{
    public ConfirmPaymentCommandValidator()
    {
        RuleFor(x => x.SubscriptionId)
            .NotEmpty()
            .WithErrorCode(DomainErrors.Common.Required);

        RuleFor(x => x.PaymentCode)
            .NotEmpty()
            .WithErrorCode(DomainErrors.Subscriptions.PaymentCodeInvalid);

        RuleFor(x => x.ExternalTransactionId)
            .NotEmpty()
            .WithErrorCode(DomainErrors.Common.Required);
    }
}
