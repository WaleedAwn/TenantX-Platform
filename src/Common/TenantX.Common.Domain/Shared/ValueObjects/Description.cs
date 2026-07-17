using TenantX.Common.Domain.Constants;
using TenantX.Common.Domain.Results;

namespace TenantX.Common.Domain.Shared.ValueObjects;

public record Description : ValueObject
{
  public string Value { get; init; }
  private Description(string value)
  {
    CheckRule(StringValidationRules.MaxLength(value, ValidationConstants.Description.MaxLength, "Description"));
    Value = value?.Trim() ?? string.Empty;
  }
  public static Result<Description> Create(string value)
  {
    var v = Validate(StringValidationRules.MaxLength(value, ValidationConstants.Description.MaxLength, "Description"));
    if (v.IsError)
      return v.TopError;

    return new Description(value);
  }
  public bool IsEmpty => string.IsNullOrWhiteSpace(Value);
  public static implicit operator string(Description d) => d.Value;
  public static implicit operator Description(string value) => new(value);
}
