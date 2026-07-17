using System.Text.RegularExpressions;

using TenantX.Common.Domain.Constants;
using TenantX.Common.Domain.Results;
using TenantX.Common.Domain.Shared;

namespace Modules.Common.Domain;

public record WorkspaceName : ValueObject
{
    public string Value { get; init; }
    private WorkspaceName(string value)
    {
        CheckRule(StringValidationRules.NotEmpty(value, "Workspace name"));
        CheckRule(StringValidationRules.MaxLength(value, 100, "Workspace name"));
        Value = value.Trim();
    }
    public static Result<WorkspaceName> Create(string value)
    {
        var v1 = Validate(StringValidationRules.NotEmpty(value, "Workspace name"));
        if (v1.IsError)
            return v1.TopError;

        var v2 = Validate(StringValidationRules.MaxLength(value, 100, "Workspace name"));
        if (v2.IsError)
            return v2.TopError;

        return new WorkspaceName(value);
    }
    public static implicit operator string(WorkspaceName name) => name.Value;
    public static implicit operator WorkspaceName(string value) => new(value);
}

// --- ACCOUNT NAME ---
public record AccountName : ValueObject
{
    public string Value { get; init; }
    private AccountName(string value)
    {
        CheckRule(StringValidationRules.NotEmpty(value, "Account name"));
        CheckRule(StringValidationRules.MaxLength(value, 100, "Account name"));
        Value = value.Trim();
    }
    public static Result<AccountName> Create(string value)
    {
        var v1 = Validate(StringValidationRules.NotEmpty(value, "Account name"));
        if (v1.IsError)
            return v1.TopError;

        var v2 = Validate(StringValidationRules.MaxLength(value, 100, "Account name"));
        if (v2.IsError)
            return v2.TopError;

        return new AccountName(value);
    }
    public static implicit operator string(AccountName name) => name.Value;
    public static implicit operator AccountName(string value) => new(value);
}

// --- DESCRIPTION ---
public record Description : ValueObject
{
    public string Value { get; init; }
    private Description(string value)
    {
        CheckRule(StringValidationRules.MaxLength(value, 500, "Description"));
        Value = value?.Trim() ?? string.Empty;
    }
    public static Result<Description> Create(string value)
    {
        var v = Validate(StringValidationRules.MaxLength(value, 500, "Description"));
        if (v.IsError)
            return v.TopError;

        return new Description(value);
    }
    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);
    public static implicit operator string(Description d) => d.Value;
    public static implicit operator Description(string value) => new(value);
}

// --- CATEGORY NAME ---
public record CategoryName : ValueObject
{
    public string Value { get; init; }
    private CategoryName(string value)
    {
        CheckRule(StringValidationRules.NotEmpty(value, "Category name"));
        CheckRule(StringValidationRules.MaxLength(value, 100, "Category name"));
        Value = value.Trim();
    }
    public static Result<CategoryName> Create(string value)
    {
        var v1 = Validate(StringValidationRules.NotEmpty(value, "Category name"));
        if (v1.IsError)
            return v1.TopError;

        var v2 = Validate(StringValidationRules.MaxLength(value, 100, "Category name"));
        if (v2.IsError)
            return v2.TopError;

        return new CategoryName(value);
    }
    public static implicit operator string(CategoryName name) => name.Value;
    public static implicit operator CategoryName(string value) => new(value);
}

// --- CLASSIFIER NAME ---
public record ClassifierName : ValueObject
{
    public string Value { get; init; }
    private ClassifierName(string value)
    {
        CheckRule(StringValidationRules.NotEmpty(value, "Classifier name"));
        CheckRule(StringValidationRules.MaxLength(value, 100, "Classifier name"));
        Value = value.Trim();
    }
    public static Result<ClassifierName> Create(string value)
    {
        var v1 = Validate(StringValidationRules.NotEmpty(value, "Classifier name"));
        if (v1.IsError)
            return v1.TopError;

        var v2 = Validate(StringValidationRules.MaxLength(value, 100, "Classifier name"));
        if (v2.IsError)
            return v2.TopError;

        return new ClassifierName(value);
    }
    public static implicit operator string(ClassifierName name) => name.Value;
    public static implicit operator ClassifierName(string value) => new(value);
}

// --- DISPLAY NAME ---
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
    public static implicit operator string(DisplayName name) => name.Value;
    public static implicit operator DisplayName(string value) => new(value);
}

// --- ORGANIZATION NAME ---
public record OrganizationName : ValueObject
{
    public string Value { get; init; }
    private OrganizationName(string value)
    {
        CheckRule(StringValidationRules.NotEmpty(value, "Organization name"));
        CheckRule(StringValidationRules.MaxLength(value, 100, "Organization name"));
        Value = value.Trim();
    }
    public static Result<OrganizationName> Create(string value)
    {
        var v1 = Validate(StringValidationRules.NotEmpty(value, "Organization name"));
        if (v1.IsError)
            return v1.TopError;

        var v2 = Validate(StringValidationRules.MaxLength(value, 100, "Organization name"));
        if (v2.IsError)
            return v2.TopError;

        return new OrganizationName(value);
    }
    public static implicit operator string(OrganizationName name) => name.Value;
    public static implicit operator OrganizationName(string value) => new(value);
}

// --- CONTACT VALUE ---
public record ContactValue : ValueObject
{
    public string Value { get; init; }
    private ContactValue(string value)
    {
        CheckRule(StringValidationRules.NotEmpty(value, "Contact value"));
        CheckRule(StringValidationRules.MaxLength(value, 100, "Contact value"));
        Value = value.Trim();
    }
    public static Result<ContactValue> Create(string value)
    {
        var v1 = Validate(StringValidationRules.NotEmpty(value, "Contact value"));
        if (v1.IsError)
            return v1.TopError;

        var v2 = Validate(StringValidationRules.MaxLength(value, 100, "Contact value"));
        if (v2.IsError)
            return v2.TopError;

        return new ContactValue(value);
    }
    public static implicit operator string(ContactValue name) => name.Value;
    public static implicit operator ContactValue(string value) => new(value);
}

// --- PERSON NAME (Complex) ---
public record PersonName : ValueObject
{
    public string FirstName { get; init; }
    public string MiddleName { get; init; }
    public string LastName { get; init; }
    public string FullName => $"{FirstName} {MiddleName} {LastName}".Trim();


    public static Result<PersonName> Create(string first, string middle, string last)
    {
        var v1 = Validate(StringValidationRules.NotEmpty(first, "First name"));
        if (v1.IsError)
            return v1.TopError;

        var v2 = Validate(StringValidationRules.NotEmpty(middle, "Middle name"));
        if (v2.IsError)
            return v2.TopError;

        var v3 = Validate(StringValidationRules.NotEmpty(last, "Last name"));
        if (v3.IsError)
            return v3.TopError;

        return new PersonName(first, middle, last);
    }
    private PersonName(string firstName, string middleName, string lastName)
    {
        CheckRule(StringValidationRules.NotEmpty(firstName, "First name"));
        CheckRule(StringValidationRules.NotEmpty(middleName, "Middle name"));
        CheckRule(StringValidationRules.NotEmpty(lastName, "Last name"));

        FirstName = firstName.Trim();
        MiddleName = middleName.Trim();
        LastName = lastName.Trim();
    }
}

/// <summary>
/// Represents a validated and normalized email address.
/// </summary>
public partial record Email : ValueObject
{
    public const int MaxLength = 255;

    // .NET Source Generator for Regex (High Performance & Secure)
    // 1. RegexOptions.ExplicitCapture: Fixes MA0023 (Saves memory)
    // 2. Timeout (250ms): Fixes MA0009 (Prevents ReDoS attacks)
    [GeneratedRegex(
        @"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.ExplicitCapture,
        250)]
    private static partial Regex EmailFormatRegex();
    public string Value { get; init; }

    // Private constructor ensures the Guardian (CheckRule) is always passed
    private Email(string value)
    {
        CheckRule(new EmailMustNotBeEmptyRule(value));
        CheckRule(new EmailMustNotBeTooLongRule(value));
        CheckRule(new EmailFormatMustBeValidRule(value));

        // Normalization: Lowercase ensures "User@Gmail.com" == "user@gmail.com"
        Value = value.Trim().ToLowerInvariant();
    }

    public static Result<Email> Create(string value)
    {
        var v1 = Validate(new EmailMustNotBeEmptyRule(value));
        if (v1.IsError)
            return v1.TopError;

        var v2 = Validate(new EmailMustNotBeTooLongRule(value));
        if (v2.IsError)
            return v2.TopError;

        var v3 = Validate(new EmailFormatMustBeValidRule(value));
        if (v3.IsError)
            return v3.TopError;

        return new Email(value);
    }

    public static implicit operator string(Email email) => email.Value;
    public static explicit operator Email(string value) => new(value);

    // --- RULES ---

    internal sealed class EmailMustNotBeEmptyRule(string? value) : IBusinessRule
    {
        public bool IsBroken() => string.IsNullOrWhiteSpace(value);
        public string Message => "Email Address is required.";
        public string Code => DomainErrors.Email.Required;
    }

    internal sealed class EmailMustNotBeTooLongRule(string? value) : IBusinessRule
    {
        public bool IsBroken() => value?.Length > 255;
        public string Message => "Email Address is to long.";
        public string Code => DomainErrors.Email.TooLong;
    }

    internal sealed class EmailFormatMustBeValidRule(string? value) : IBusinessRule
    {
        public bool IsBroken() => value is not null && !EmailFormatRegex().IsMatch(value);
        public string Message => "The email address format is invalid.";
        public string Code => DomainErrors.Email.InvalidFormat;
    }
}