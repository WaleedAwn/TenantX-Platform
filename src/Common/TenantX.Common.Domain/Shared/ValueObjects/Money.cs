using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TenantX.Common.Domain.Constants;
using TenantX.Common.Domain.Results;

namespace TenantX.Common.Domain.Shared.ValueObjects;

public record Money : ValueObject
{
    public decimal Amount { get; init; }
    public Currency Currency { get; init; }

    private Money(decimal amount, Currency currency)
    {
        Amount = currency.RoundAmount(amount);
        Currency = currency;
    }

    public static Result<Money> Create(decimal amount, string currencyCode)
    {
        if (amount < 0)
            return Error.Validation(DomainErrors.Common.InvalidAmount, "Amount cannot be negative");

        var currencyResult = Currency.FromCode(currencyCode);
        if (currencyResult.IsError)
            return currencyResult.Errors;

        return new Money(amount, currencyResult.Value);
    }

    // Helpers for money math
    public static Money Zero(Currency currency) => new(0, currency);


}
