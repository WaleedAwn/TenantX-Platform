using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TenantX.Common.Domain.Shared;

namespace TenantX.Subscriptions.Domain.Features;

public record FeatureId(Guid Value) : EntityId<FeatureId>(Value)
{
    public static new FeatureId New() => new(Guid.NewGuid());
    public static new FeatureId From(Guid value) => new(value);
}
