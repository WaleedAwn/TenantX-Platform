using TenantX.Common.Domain.Shared;

namespace TenantX.Common.Domain.Constants;

public static class StringValidationRules
{
    public static IBusinessRule NotEmpty(string? value, string fieldName) => new StringNotEmptyRule(value, fieldName);
    public static IBusinessRule MaxLength(string? value, int maxLength, string fieldName) => new StringMaxLengthRule(value, maxLength, fieldName);
    public static IBusinessRule MinLength(string? value, int minLength, string fieldName) => new StringMinLengthRule(value, minLength, fieldName);
}

internal sealed class StringMinLengthRule(string? value, int minLength, string fieldName) : IBusinessRule
{
    public bool IsBroken() => value is not null && value.Length < minLength;
    public string Message => $"{fieldName} must be at least {minLength} characters";
    public string Code => DomainErrors.Common.TooShort;
}
internal sealed class StringNotEmptyRule(string? value, string fieldName) : IBusinessRule
{
    public bool IsBroken() => string.IsNullOrWhiteSpace(value);
    public string Message => $"{fieldName} cannot be empty";
    public string Code => DomainErrors.Common.Required;
}

internal sealed class StringMaxLengthRule(string? value, int maxLength, string fieldName) : IBusinessRule
{
    public bool IsBroken() => value?.Length > maxLength;
    public string Message => $"{fieldName} cannot exceed {maxLength} characters";
    public string Code => DomainErrors.Common.TooLong;
}

