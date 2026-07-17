using TenantX.Common.Domain.Results;
using TenantX.Common.Domain.Shared;

namespace TenantX.Subscriptions.Domain.Subscribers;

/// <summary>
/// Represents the User ID from the Tenant's external system.
/// </summary>
public record ExternalUserId : ValueObject
{
  public string Value { get; init; }

  private ExternalUserId(string value) => Value = value.Trim();

  public static Result<ExternalUserId> Create(string value)
  {
    if (string.IsNullOrWhiteSpace(value))
      return SubscriberErrors.ExternalIdRequired;

    return new ExternalUserId(value);
  }

  public static implicit operator string(ExternalUserId id) => id.Value;
}
