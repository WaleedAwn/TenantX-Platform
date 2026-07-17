using TenantX.Common.Domain.Constants;
using TenantX.Common.Domain.Results;

namespace TenantX.Subscriptions.Domain.Plans;

public static class PlanErrors
{
  public static readonly Error NotFound =
      Error.NotFound(DomainErrors.Plan.PlanNotFound, "The requested plan was not found.");

  public static readonly Error Archived =
      Error.Conflict(DomainErrors.Plan.Archived, "This plan is archived and cannot be modified.");

  public static readonly Error TenantMismatch =
      Error.Forbidden(DomainErrors.Plan.TenantMismatch, "The feature belongs to a different tenant.");

  public static readonly Error InvalidNumericValue =
      Error.Validation(DomainErrors.Entitlement.InvalidValue, "The feature requires a numeric value.");

  public static readonly Error InvalidBooleanValue =
      Error.Validation(DomainErrors.Entitlement.InvalidValue, "The feature requires a boolean value ('true' or 'false').");
}
