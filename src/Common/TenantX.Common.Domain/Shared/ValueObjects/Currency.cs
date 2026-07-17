using TenantX.Common.Domain.Constants;
using TenantX.Common.Domain.Results;

namespace TenantX.Common.Domain.Shared.ValueObjects;

public record Currency : ValueObject
{
  public string Code { get; init; }
  public string Name { get; init; }
  public string Symbol { get; init; }
  public int DecimalPlaces { get; init; }


  /// <summary>
  /// Creates a custom currency. Returns a Result instead of throwing an exception.
  /// </summary>
  public static Result<Currency> Create(string code, string name, string symbol, int decimalPlaces)
  {
    CurrencyCodeMustNotBeEmptyRule codeRule = new CurrencyCodeMustNotBeEmptyRule(code);
    if (codeRule.IsBroken())
      return Error.Validation(codeRule.Code, codeRule.Message);

    CurrencyNameMustNotBeEmptyRule nameRule = new CurrencyNameMustNotBeEmptyRule(name);
    if (nameRule.IsBroken())
      return Error.Validation(nameRule.Code, nameRule.Message);

    CurrencySymbolMustNotBeEmptyRule symbolRule = new CurrencySymbolMustNotBeEmptyRule(symbol);
    if (symbolRule.IsBroken())
      return Error.Validation(symbolRule.Code, symbolRule.Message);

    DecimalPlacesMustBeValidRule decimalRule = new DecimalPlacesMustBeValidRule(decimalPlaces);
    if (decimalRule.IsBroken())
      return Error.Validation(decimalRule.Code, decimalRule.Message);

    return new Currency(code, name, symbol, decimalPlaces);
  }
  private Currency(string code, string name, string symbol, int decimalPlaces)
  {
    CheckRule(new CurrencyCodeMustNotBeEmptyRule(code));
    CheckRule(new CurrencyNameMustNotBeEmptyRule(name));
    CheckRule(new CurrencySymbolMustNotBeEmptyRule(symbol));
    CheckRule(new DecimalPlacesMustBeValidRule(decimalPlaces));

    Code = code.ToUpperInvariant();
    Name = name;
    Symbol = symbol;
    DecimalPlaces = decimalPlaces;
  }
  // Required for EF Core Complex Type materialization.
  // Currency is stored as Code and reconstructed via FromCode().
  // This constructor allows EF Core to instantiate before setting properties.
  private Currency()
  {
    Code = default!;
    Name = default!;
    Symbol = default!;
    DecimalPlaces = default;
  }

  public static Currency USD => new("USD", "US Dollar", "$", 2);
  public static Currency EUR => new("EUR", "Euro", "€", 2);
  public static Currency GBP => new("GBP", "British Pound", "£", 2);
  public static Currency JPY => new("JPY", "Japanese Yen", "¥", 0);
  public static Currency CAD => new("CAD", "Canadian Dollar", "C$", 2);
  public static Currency AUD => new("AUD", "Australian Dollar", "A$", 2);
  public static Currency CHF => new("CHF", "Swiss Franc", "Fr", 2);
  public static Currency CNY => new("CNY", "Chinese Yuan", "¥", 2);
  public static Currency SEK => new("SEK", "Swedish Krona", "kr", 2);
  public static Currency NZD => new("NZD", "New Zealand Dollar", "NZ$", 2);
  public static Currency YER => new("YER", "Yemeni Rial", "﷼", 2);
  public static Currency SAR => new("SAR", "Saudi Riyal", "﷼", 2);

  public static IReadOnlyList<Currency> SupportedCurrencies => new[]
  {
        USD, EUR, GBP, JPY, CAD, AUD, CHF, CNY, SEK, NZD, YER, SAR
    };

  // public static Currency FromCode(string code)
  // {
  //     CheckRule(new CurrencyCodeMustNotBeEmptyRule(code));

  //     Currency? currency = SupportedCurrencies.FirstOrDefault(c => c.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
  //     CheckRule(new CurrencyCodeMustBeSupportedRule(code, currency));

  //     return currency!;
  // }
  /// <summary>
  /// Professional Result-based lookup for a currency by its code.
  /// </summary>
  public static Result<Currency> FromCode(string code)
  {
    CurrencyCodeMustNotBeEmptyRule emptyRule = new CurrencyCodeMustNotBeEmptyRule(code);
    if (emptyRule.IsBroken())
      return Error.Validation(emptyRule.Code, emptyRule.Message);

    Currency? currency = SupportedCurrencies.FirstOrDefault(c =>
     c.Code.Equals(code, StringComparison.OrdinalIgnoreCase));

    CurrencyCodeMustBeSupportedRule supportedRule = new CurrencyCodeMustBeSupportedRule(code, currency);
    if (supportedRule.IsBroken())
      return Error.NotFound(supportedRule.Code, supportedRule.Message);

    return currency!;
  }
  public decimal RoundAmount(decimal amount)
  {
    return Math.Round(amount, DecimalPlaces, MidpointRounding.AwayFromZero);
  }
}
internal sealed class CurrencyCodeMustNotBeEmptyRule(string code) : IBusinessRule
{
    public bool IsBroken() => string.IsNullOrWhiteSpace(code);
    public string Message => "Currency code cannot be empty";
    public string Code => DomainErrors.Currency.CodeRequired;
}

internal sealed class CurrencyNameMustNotBeEmptyRule(string name) : IBusinessRule
{
    public bool IsBroken() => string.IsNullOrWhiteSpace(name);
    public string Message => "Currency name cannot be empty";
    public string Code => DomainErrors.Currency.NameRequired;
}

internal sealed class CurrencySymbolMustNotBeEmptyRule(string symbol) : IBusinessRule
{
    public bool IsBroken() => string.IsNullOrWhiteSpace(symbol);
    public string Message => "Currency symbol cannot be empty";
    public string Code => DomainErrors.Currency.SymbolRequired;
}

internal sealed class DecimalPlacesMustBeValidRule(int decimalPlaces) : IBusinessRule
{
    public bool IsBroken() => decimalPlaces < 0 || decimalPlaces > 4;
    public string Message => "Decimal places must be between 0 and 4";
    public string Code => DomainErrors.Currency.InvalidDecimalPlaces;
}

internal sealed class CurrencyCodeMustBeSupportedRule(string code, Currency? currency) : IBusinessRule
{
    public bool IsBroken() => currency is null;
    public string Message => $"Unsupported currency code: {code}";
    public string Code => DomainErrors.Currency.Unsupported;
}
