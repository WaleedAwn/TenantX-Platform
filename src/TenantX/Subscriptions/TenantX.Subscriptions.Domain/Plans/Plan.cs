using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TenantX.Common.Domain.Constants;
using TenantX.Common.Domain.Enums;
using TenantX.Common.Domain.Results;
using TenantX.Common.Domain.Shared;
using TenantX.Common.Domain.Shared.ValueObjects;
using TenantX.Subscriptions.Domain.Features;
using TenantX.Subscriptions.Domain.Plans.Entitlements;
using TenantX.Subscriptions.Domain.Plans.Events;

namespace TenantX.Subscriptions.Domain.Plans;

/// <summary>
/// Represents a commercial subscription package (e.g., "Gold Tier").
/// Bundles pricing, billing cycles, and a collection of specific feature limits (Entitlements).
/// </summary>
/// <remarks>
/// <b>Business Rules:</b>
/// 1. Type Safety: Values assigned to features must match the feature's defined type (Numeric/Boolean).
/// 2. Tenant Isolation: A plan can only contain features belonging to the same tenant.
/// 3. Archival Locking: Once archived, the plan becomes a historical record and cannot be edited.
/// 4. Unique Entitlements: A plan cannot have duplicate entries for the same feature; updates replace old values.
/// </remarks>
public sealed class Plan : TenantScopedAggregateRoot<PlanId>
{
    public DisplayName Name { get; private set; }
    public Description Description { get; private set; }
    public Money Price { get; private set; }
    public BillingPeriod BillingPeriod { get; private set; }
    public bool IsArchived { get; private set; }

    // Internal collection managed by the Aggregate Root
    private readonly List<Entitlement> _entitlements = new();

    /// <summary>
    /// Gets the read-only collection of feature limits defined for this plan.
    /// </summary>
    public IReadOnlyCollection<Entitlement> Entitlements => _entitlements.AsReadOnly();

    // Required for EF Core
#pragma warning disable CS8618
    private Plan() { }
#pragma warning restore CS8618

    private Plan(PlanId id, DisplayName name, Description description, Money price, BillingPeriod billingPeriod) : base(id)
    {
        Name = name;
        Description = description;
        Price = price;
        BillingPeriod = billingPeriod;
        IsArchived = false;
    }

    /// <summary>
    /// Professional Factory Method. Coordinates validation of DisplayName, Description, and Money.
    /// </summary>
    public static Plan Create(
         DisplayName name,
         Description description,
         Money price,
         BillingPeriod billingPeriod)
    {
        var plan = new Plan(PlanId.New(), name, description, price, billingPeriod);

        plan.RaiseDomainEvent(new PlanCreatedDomainEvent(plan.Id));

        return plan;
    }

    /// <summary>
    /// Configures a limit for a specific feature. Handles both adding new and updating existing entitlements.
    /// </summary>
    /// <param name="feature">The feature definition to reference.</param>
    /// <param name="value">The value string (e.g., "10" or "true").</param>
    /// <returns>A result indicating success or a specific business rule violation.</returns>
    public Result<Success> SetEntitlement(Feature feature, string value)
    {
        // Rule: Archival Locking
        if (IsArchived)
            return PlanErrors.Archived;

        // Rule: Tenant Isolation
        if (feature.TenantId != this.TenantId)
            return PlanErrors.TenantMismatch;

        // Rule: Type Safety Check
        var typeValidation = ValidateTypeIntegrity(feature, value);

        if (typeValidation.IsError)
            return typeValidation;

        var existing = _entitlements.FirstOrDefault(e => e.FeatureId == feature.Id);

        if (existing != null)
        {
            // Rule: Unique Entitlements (Update existing)
            var updateResult = existing.UpdateValue(value);
            if (updateResult.IsError)
                return updateResult.Errors;
        }
        else
        {
            // Create new child entity via its internal factory
            var entitlementResult = Entitlement.Create(feature.Id, value);
            if (entitlementResult.IsError)
                return entitlementResult.Errors;

            _entitlements.Add(entitlementResult.Value);
        }

        RaiseDomainEvent(new PlanEntitlementSetDomainEvent(Id, feature.Id.Value, value));

        return Result.Success;
    }

    /// <summary>
    /// Validates metadata updates for the plan.
    /// </summary>
    public Result<Updated> UpdateMetadata(DisplayName name, Description description)
    {
        if (IsArchived)
            return PlanErrors.Archived;

        Name = name;
        Description = description;

        return Result.Updated;
    }

    /// <summary>
    /// Retires the plan. Archiving is irreversible to maintain historical billing integrity.
    /// </summary>
    public void Archive()
    {
        if (IsArchived)
            return;

        IsArchived = true;
        RaiseDomainEvent(new PlanArchivedDomainEvent(Id));
    }

    /// <summary>
    /// Internal validation logic to ensure values match the feature blueprint.
    /// </summary>
    private static Result<Success> ValidateTypeIntegrity(Feature feature, string value)
    {
        if (feature.Type == FeatureType.Numeric)
        {
            // Note: We support "-1" as a standard convention for "Unlimited"
            if (!int.TryParse(value, out _))
                return PlanErrors.InvalidNumericValue;
        }
        else if (feature.Type == FeatureType.Boolean)
        {
            var normalized = value.ToLowerInvariant().Trim();
            if (normalized != "true" && normalized != "false")
                return PlanErrors.InvalidBooleanValue;
        }

        return Result.Success;
    }
}
