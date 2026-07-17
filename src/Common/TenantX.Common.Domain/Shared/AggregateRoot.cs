namespace TenantX.Common.Domain.Shared;

/// <summary>
/// Base class for all aggregate roots.
/// Provides domain events support but NO multi-tenancy (TenantId).
/// Use <see cref="TenantScopedAggregateRoot{TId}"/> for tenant-scoped aggregates.
/// </summary>
/// <remarks>
/// Global aggregates (User, Tenant, ApiClient) should inherit from this directly.
/// Tenant-scoped aggregates (Account, Workspace, Transaction, etc.) should use TenantScopedAggregateRoot.
/// </remarks>
public abstract class AggregateRoot<TId> : AuditableEntity<TId>, IHasDomainEvents
    where TId : EntityId<TId>
{
  private readonly List<DomainEvent> _domainEvents = new();
  protected AggregateRoot(TId id) : base(id) { }
  protected AggregateRoot() : base() { }   // For EF Core
  public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

#pragma warning disable CA1030 // Use events where appropriate
  protected void RaiseDomainEvent(DomainEvent domainEvent)
  {
    _domainEvents.Add(domainEvent);
  }

  public void ClearDomainEvents()
  {
    _domainEvents.Clear();
  }
}