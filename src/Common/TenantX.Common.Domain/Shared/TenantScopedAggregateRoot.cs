namespace TenantX.Common.Domain.Shared;

/// <summary>
/// Base class for tenant-scoped aggregate roots.
/// Extends <see cref="AggregateRoot{TId}"/> with multi-tenancy support (TenantId).
/// </summary>
/// <remarks>
/// Tenant-scoped aggregates (Account, Workspace, Transaction, Permission, Party,
/// ContactMechanism, Classifier) should inherit from this class.
/// Global aggregates (User, Tenant, ApiClient) should inherit from AggregateRoot directly.
/// </remarks>
public abstract class TenantScopedAggregateRoot<TId> : AggregateRoot<TId>, ITenantEntity
    where TId : EntityId<TId>
{

  protected TenantScopedAggregateRoot(TId id) : base(id)
  {
    // TenantId will be set automatically by ApplicationDbContext.SaveChangesAsync()
    TenantId = TenantId.From(Guid.Empty); // Temporary - will be set by infrastructure
  }

  protected TenantScopedAggregateRoot() : base()
  {
    TenantId = default!;
  }
  public TenantId TenantId { get; set; }
}



/// <summary>
/// Marker interface for entities that belong to a tenant.
/// Implemented by  TenantScopedAggregateRoot to ensure all aggregates are tenant-scoped.
/// </summary>
public interface ITenantEntity
{
  TenantId TenantId { get; set; }
}