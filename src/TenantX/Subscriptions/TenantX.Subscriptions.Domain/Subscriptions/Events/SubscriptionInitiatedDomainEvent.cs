using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TenantX.Common.Domain;
using TenantX.Subscriptions.Domain.Plans;
using TenantX.Subscriptions.Domain.Subscribers;

namespace TenantX.Subscriptions.Domain.Subscriptions.Events;

public record SubscriptionInitiatedDomainEvent(SubscriptionId SubscriptionId, SubscriberId SubscriberId) : DomainEvent;
public record SubscriptionActivatedDomainEvent(SubscriptionId SubscriptionId, PlanId PlanId) : DomainEvent;
public record SubscriptionCanceledDomainEvent(SubscriptionId SubscriptionId) : DomainEvent;
public record SubscriptionExpiredDomainEvent(SubscriptionId SubscriptionId) : DomainEvent;
public record SubscriptionDeactivatedDomainEvent(SubscriptionId SubscriptionId) : DomainEvent;