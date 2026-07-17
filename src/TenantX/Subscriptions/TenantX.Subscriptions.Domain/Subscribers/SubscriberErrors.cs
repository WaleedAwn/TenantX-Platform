using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TenantX.Common.Domain.Constants;
using TenantX.Common.Domain.Results;

namespace TenantX.Subscriptions.Domain.Subscribers;

public static class SubscriberErrors
{
    public static readonly Error NotFound =
        Error.NotFound(DomainErrors.Subscriber.NotFound, "Subscriber not found.");

    public static readonly Error AlreadyExists =
        Error.Conflict(DomainErrors.Subscriber.AlreadyExists, "A subscriber with this external ID already exists for this tenant.");

    public static readonly Error ExternalIdRequired =
        Error.Validation(DomainErrors.Subscriber.ExternalIdRequired, "External User ID is required.");
}
