
using TenantX.Common.Domain.Constants;
using TenantX.Common.Domain.Results;

namespace TenantX.Subscriptions.Domain.Plans.Entitlements;

public static class EntitlementErrors
{
  public static readonly Error FeatureIdRequired =
      Error.Validation(DomainErrors.Entitlement.FeatureIdRequired, "Feature ID is required for an entitlement.");

  public static readonly Error ValueRequired =
      Error.Validation(DomainErrors.Entitlement.ValueRequired, "Entitlement value cannot be empty.");

  public static readonly Error InvalidValue =
      Error.Validation(DomainErrors.Entitlement.InvalidValue, "The provided value is not valid for this feature type.");
}
