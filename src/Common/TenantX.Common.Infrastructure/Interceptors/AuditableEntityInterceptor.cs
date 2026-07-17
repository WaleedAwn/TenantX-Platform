using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

using TenantX.Common.Application.Clock;
using TenantX.Common.Domain.Shared;

namespace TenantX.Common.Infrastructure.Interceptors;

public sealed class AuditableEntityInterceptor(
		IDateTimeProvider dateTime,
		ICurrentUserContext userContext) : SaveChangesInterceptor
{
	private readonly IDateTimeProvider _dateTime = dateTime;
	private readonly ICurrentUserContext _userContext = userContext;

	public override InterceptionResult<int> SavingChanges(
			DbContextEventData eventData,
			InterceptionResult<int> result)
	{
		UpdateEntities(eventData.Context);
		return base.SavingChanges(eventData, result);
	}

	public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
			DbContextEventData eventData,
			InterceptionResult<int> result,
			CancellationToken cancellationToken = default)
	{
		UpdateEntities(eventData.Context);
		return base.SavingChangesAsync(eventData, result, cancellationToken);
	}

	private void UpdateEntities(DbContext? context)
	{
		if (context is null)
			return;

		DateTime utcNow = _dateTime.UtcNow;
		UserId currentUserId = _userContext.UserId;
		TenantId currentTenantId = _userContext.TenantId;

		foreach (EntityEntry entry in context.ChangeTracker.Entries())
		{
			// 1. Handle Multi-Tenancy (ITenantEntity)
			if (entry.Entity is ITenantEntity tenantEntity && entry.State == EntityState.Added)
			{
				// Ensure the entity is locked to the user's current tenant
				tenantEntity.TenantId = currentTenantId;
			}

			// 2. Handle Auditing (IAuditableEntity)
			if (entry.Entity is IAuditableEntity auditableEntity)
			{
				if (entry.State == EntityState.Added)
				{
					auditableEntity.CreatedAt = utcNow;
					auditableEntity.CreatedBy = currentUserId;

					// On creation, Modified is the same as Created
					auditableEntity.ModifiedAt = utcNow;
					auditableEntity.ModifiedBy = currentUserId;
				}

				if (entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
				{
					auditableEntity.ModifiedAt = utcNow;
					auditableEntity.ModifiedBy = currentUserId;
				}
			}
		}
	}
}

public static class EntityEntryExtensions
{
	/// <summary>
	/// Checks if any "Owned Entities" (Value Objects like PersonName, Address, etc.) 
	/// inside this Aggregate have changed.
	/// </summary>
	public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
			entry.References.Any(r =>
					r.TargetEntry != null &&
					r.TargetEntry.Metadata.IsOwned() &&
					(r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
}