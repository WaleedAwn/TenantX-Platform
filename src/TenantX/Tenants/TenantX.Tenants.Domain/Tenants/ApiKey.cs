using System.Security.Cryptography;

using TenantX.Common.Domain.Constants;
using TenantX.Common.Domain.Shared;

namespace TenantX.Tenants.Domain.Tenants;

public sealed record ApiKey : ValueObject
{
    public string Value { get; init; }

    private ApiKey(string value)
    {
        CheckRule(StringValidationRules.NotEmpty(value, nameof(ApiKey)));
        Value = value;
    }

    public static ApiKey Generate()
    {
        var bytes = new byte[32];
        using var generator = RandomNumberGenerator.Create();
        generator.GetBytes(bytes);
        return new ApiKey(Convert.ToBase64String(bytes));
    }

    public static ApiKey From(string value) => new(value);
}
