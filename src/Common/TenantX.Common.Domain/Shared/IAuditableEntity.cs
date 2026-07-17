namespace TenantX.Common.Domain.Shared;



public abstract class AuditableEntity<TId> : BaseEntity<TId>, IAuditableEntity
		where TId : EntityId<TId>
{
	protected AuditableEntity(TId id) : base(id)
	{
		// Initial defaults. These are typically overwritten in the 
		// Infrastructure layer (DbContext.SaveChangesAsync)
		// We set these to temporary defaults. 
		// IMPORTANT: We use 'id.Value' because UserId.From expects a Guid.
		// This keeps the object "Valid" until SaveChangesAsync replaces it.
		CreatedBy = UserId.From(id);

		CreatedAt = DateTime.UtcNow;
		ModifiedAt = DateTime.UtcNow;
		ModifiedBy = default!;
	}
	// Required for EF Core
	protected AuditableEntity() : base()
	{
		CreatedBy = default!;
	}

	public UserId CreatedBy { get; set; }
	public UserId? ModifiedBy { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? ModifiedAt { get; set; }

	/// <summary>
	/// Used for optimistic concurrency checks in EF Core
	/// </summary>
	public byte[] RowVersion { get; internal set; } = Array.Empty<byte>();
}
public interface IAuditableEntity
{
	UserId CreatedBy { get; set; }
	UserId? ModifiedBy { get; set; }
	DateTime CreatedAt { get; set; }
	DateTime? ModifiedAt { get; set; }
}