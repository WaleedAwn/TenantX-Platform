namespace TenantX.Common.Domain.Shared;

/// <summary>
/// Interface for entities that can raise domain events.
/// This is used by the Infrastructure layer (EF Core Interceptors or Unit of Work) 
/// to dispatch events after a successful database save.
/// </summary>
public interface IHasDomainEvents
{
  /// <summary>
  /// The collection of domain events raised during the current business transaction.
  /// </summary>
  IReadOnlyCollection<DomainEvent> DomainEvents { get; }

  /// <summary>
  /// Clears the events collection after they have been successfully dispatched.
  /// </summary>
  void ClearDomainEvents();

  // we can use this clear one method 

}

