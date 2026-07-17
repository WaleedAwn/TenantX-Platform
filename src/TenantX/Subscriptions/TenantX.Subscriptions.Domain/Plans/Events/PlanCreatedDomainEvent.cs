using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TenantX.Common.Domain;

namespace TenantX.Subscriptions.Domain.Plans.Events;
public record PlanCreatedDomainEvent(PlanId PlanId) : DomainEvent;
public record PlanEntitlementSetDomainEvent(PlanId PlanId, Guid FeatureId, string Value) : DomainEvent;
public record PlanArchivedDomainEvent(PlanId PlanId) : DomainEvent;