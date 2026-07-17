using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TenantX.Common.Domain;

namespace TenantX.Subscriptions.Domain.Subscribers.Events;
public record SubscriberCreatedDomainEvent(SubscriberId SubscriberId, ExternalUserId ExternalId) : DomainEvent;
