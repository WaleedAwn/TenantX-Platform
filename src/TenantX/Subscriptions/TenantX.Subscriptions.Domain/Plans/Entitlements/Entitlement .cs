using TenantX.Common.Domain.Results;
using TenantX.Common.Domain.Shared;
using TenantX.Subscriptions.Domain.Features;

namespace TenantX.Subscriptions.Domain.Plans.Entitlements;

/// <summary>
/// Represents the concrete assignment of a value to a specific capability within a commercial package.
/// </summary>
/// <remarks>
/// <para>
/// <b>Main Use:</b>
/// The Entitlement serves as the "connective tissue" between a <see cref="Plan"/> (the Product) 
/// and a <see cref="Feature"/> (the Metric). While a Feature defines *what* can be limited 
/// (e.g., "Max Workspaces"), the Entitlement defines *how much* is allowed for that specific Plan 
/// (e.g., "5 Workspaces").
/// </para>
/// <para>
/// <b>Domain Role:</b>
/// This is a <b>Child Entity</b> within the <see cref="Plan"/> Aggregate. It has no independent 
/// lifecycle; it is created, updated, and deleted exclusively through the Plan Aggregate Root 
/// to ensure that business rules (such as type-safety and tenant isolation) are never bypassed.
/// </para>
/// <para>
/// <b>Data Integrity:</b>
/// Values are stored as strings to maintain a generic engine. However, the integrity of these 
/// strings (Numeric vs Boolean) is enforced by the Plan Aggregate Root by referencing the 
/// linked Feature's metadata.
/// </para>
/// </remarks>
public sealed class Entitlement : AuditableEntity<EntitlementId>
{
    /// <summary>
    /// Gets the unique identifier of the feature this entitlement refers to.
    /// </summary>
    public FeatureId FeatureId { get; private set; }

    /// <summary>
    /// Gets the specific limit, quota, or toggle value.
    /// <example>Possible values: "true", "false", "10", "-1" (for unlimited).</example>
    /// </summary>
    public string Value { get; private set; }

    // Required for EF Core materialization via proxy or reflection
#pragma warning disable CS8618 
    private Entitlement() { }
#pragma warning restore CS8618 

    /// <summary>
    /// Initializes a new instance of the Entitlement. 
    /// Private to enforce the use of the <see cref="Create"/> factory method.
    /// </summary>
    private Entitlement(EntitlementId id, FeatureId featureId, string value) : base(id)
    {
        FeatureId = featureId;
        Value = value;
    }

    /// <summary>
    /// Factory Method for internal Aggregate use.
    /// Validates basic entity requirements before allowing instantiation.
    /// </summary>
    /// <param name="featureId">The ID of the feature being limited.</param>
    /// <param name="value">The initial value assigned to the feature.</param>
    /// <returns>A Result containing the Entitlement or a validation error.</returns>
    internal static Result<Entitlement> Create(FeatureId featureId, string value)
    {
        if (featureId is null)
        {
            return EntitlementErrors.FeatureIdRequired;
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            return EntitlementErrors.ValueRequired;
        }

        return new Entitlement(EntitlementId.New(), featureId, value.Trim());
    }

    /// <summary>
    /// Updates the assigned value of the entitlement.
    /// </summary>
    /// <param name="newValue">The new validated value string.</param>
    /// <returns>A Result indicating the update success or a validation error.</returns>
    internal Result<Updated> UpdateValue(string newValue)
    {
        if (string.IsNullOrWhiteSpace(newValue))
        {
            return EntitlementErrors.ValueRequired;
        }

        Value = newValue.Trim();
        return Result.Updated;
    }
}