using System.Text.RegularExpressions;

using TenantX.Common.Domain.Constants;
using TenantX.Common.Domain.Results;
using TenantX.Common.Domain.Shared;

namespace TenantX.Subscriptions.Domain.Features;
// <summary>
/// Represents a unique, slug-style key for a feature (e.g., "max_projects").
/// Note: Must be 'partial' to allow the .NET Regex Source Generator to work.
/// </summary>
public partial record FeatureKey : ValueObject
{
  public string Value { get; init; }

  // 1. Private constructor ensures the only way to create this is through the 'Create' method.
  private FeatureKey(string value)
  {
    Value = value.ToLowerInvariant().Trim();
  }

  /// <summary>
  /// Factory method with validation logic.
  /// </summary>
  public static Result<FeatureKey> Create(string value)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      return Error.Validation(DomainErrors.Common.Required, "Feature key is required.");
    }

    // 2. Use the Source-Generated Regex for matching.
    if (!KeyValidator().IsMatch(value))
    {
      return Error.Validation(
          DomainErrors.Feature.InvalidKey,
          "Feature key must be lowercase alphanumeric with underscores only.");
    }

    return new FeatureKey(value);
  }

  // 3. This is the "Professional" way to handle Regex in .NET 7/8+.
  // It is compiled at build time (fast) and has a timeout (secure).
  [GeneratedRegex(@"^[a-z0-9_]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase, 1000)]
  private static partial Regex KeyValidator();

  public override string ToString() => Value;

  // 5. Implicit operators for easier usage in code
  public static implicit operator string(FeatureKey key) => key.Value;
}