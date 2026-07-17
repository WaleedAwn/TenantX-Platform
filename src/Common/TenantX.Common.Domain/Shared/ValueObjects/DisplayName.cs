using TenantX.Common.Domain.Constants;
using TenantX.Common.Domain.Results;

namespace TenantX.Common.Domain.Shared.ValueObjects;

public record DisplayName : ValueObject
{
  public string Value { get; init; }
  private DisplayName(string value)
  {
    CheckRule(StringValidationRules.NotEmpty(value, "Display name"));
    CheckRule(StringValidationRules.MaxLength(value, 50, "Display name"));
    Value = value.Trim();
  }
  public static Result<DisplayName> Create(string value)
  {
    var v1 = Validate(StringValidationRules.NotEmpty(value, "Display name"));
    if (v1.IsError)
      return v1.TopError;

    var v2 = Validate(StringValidationRules.MaxLength(value, 50, "Display name"));
    if (v2.IsError)
      return v2.TopError;

    return new DisplayName(value);
  }

  // public static implicit operator string(DisplayName name) => name.Value;
  // public static implicit operator DisplayName(string value) => new(value);
}
