namespace TenantX.Common.Domain.Shared;


#pragma warning disable S4035 // Classes implementing "IEquatable<T>" should be sealed
public abstract class BaseEntity<TId> : IEquatable<BaseEntity<TId>>
    where TId : EntityId<TId>
{
  protected BaseEntity(TId id)
  {
    Id = id;
  }

  // For EF Core
  protected BaseEntity()
  {
    Id = default!;
  }
  public TId Id { get; private set; }
  public bool Equals(BaseEntity<TId>? other)
  {
    if (other is null)
      return false;
    if (ReferenceEquals(this, other))
      return true;
    return Id.Equals(other.Id);
  }

  public override bool Equals(object? obj) => obj is BaseEntity<TId> other && Equals(other);
  public override int GetHashCode() => Id.GetHashCode();
  public static bool operator ==(BaseEntity<TId>? left, BaseEntity<TId>? right) => Equals(left, right);
  public static bool operator !=(BaseEntity<TId>? left, BaseEntity<TId>? right) => !Equals(left, right);

}