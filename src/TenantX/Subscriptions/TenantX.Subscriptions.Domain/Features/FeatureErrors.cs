using TenantX.Common.Domain.Constants;
using TenantX.Common.Domain.Results;

namespace TenantX.Subscriptions.Domain.Features;

public static class FeatureErrors
{
  public static readonly Error NotFound =
      Error.NotFound(DomainErrors.Feature.NotFound, "Feature not found.");

  public static readonly Error AlreadyExists =
      Error.Conflict(DomainErrors.Feature.AlreadyExists, "A feature with this key already exists for this tenant.");
}
