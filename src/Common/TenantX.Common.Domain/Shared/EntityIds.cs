namespace TenantX.Common.Domain.Shared;

/// <summary>
/// Strongly typed identifier for the <c>Tenant</c> aggregate.
/// Represents the top-level organizational unit in the multi-tenancy model.
/// </summary>
public record TenantId(Guid Value) : EntityId<TenantId>(Value)
{
    public static new TenantId New() => new(Guid.NewGuid());
    public static new TenantId From(Guid value) => new(value);
}
/// <summary>
/// Strongly typed identifier for the <c>User</c> aggregate.
/// Represents an authenticated user in the system (global, not tenant-scoped).
/// </summary>
public record UserId(Guid Value) : EntityId<UserId>(Value)
{
    public static new UserId New() => new(Guid.NewGuid());
    public static new UserId From(Guid value) => new(value);
}