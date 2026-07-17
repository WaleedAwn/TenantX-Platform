using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TenantX.Common.Domain.Constants;
using TenantX.Common.Domain.Results;
using TenantX.Common.Domain.Shared;

namespace TenantX.Tenants.Domain.Tenants.valueObjects;

public record TenantName : ValueObject
{
    private const int MinLength = 3;
    private const int MaxLength = 100;

    public string Value { get; init; }
    public static Result<TenantName> Create(string value)
    {
        var minRule = Validate(StringValidationRules.MinLength(value, 3, "Tenant name"));
        if (minRule.IsError)
            return minRule.TopError;

        return new TenantName(value);
    }
    public TenantName(string value)
    {
        CheckRule(StringValidationRules.NotEmpty(value, "Tenant name"));
        CheckRule(StringValidationRules.MinLength(value, MinLength, "Tenant name"));
        CheckRule(StringValidationRules.MaxLength(value, MaxLength, "Tenant name"));

        Value = value.Trim();
    }

    public static implicit operator string(TenantName name) => name.Value;
    public static implicit operator TenantName(string value) => new(value);

    public override string ToString() => Value;
}
