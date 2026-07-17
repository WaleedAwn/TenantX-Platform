using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TenantX.Common.Domain.Results;

namespace TenantX.Subscriptions.Application.Features.Subscriptions.Dtos;

public record InitiateSubscriptionResponse(
    Guid SubscriptionId,
    string PaymentCode,
    DateTime ExpiresAtUtc);