using TenantX.Common.Domain.Results;
using TenantX.Common.Domain.Shared;
using TenantX.Tenants.Domain.Tenants.valueObjects;

namespace TenantX.Tenants.Domain.Tenants;

public sealed class Tenant : AggregateRoot<TenantId>
{
    public TenantName Name { get; private set; }
    public ApiKey ApiKey { get; private set; }
    public bool IsActive { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
		private Tenant() : base() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

		private Tenant(TenantId id, TenantName name, ApiKey apiKey) : base(id)
    {
        Name = name;
        ApiKey = apiKey;
        IsActive = true;
    }

    /// <summary>
    /// Static factory returning Result<Tenant>
    /// </summary>
    public static Result<Tenant> Create(TenantName name)
    {
        var tenant = new Tenant(TenantId.New(), name, ApiKey.Generate());
        
        // tenant.RaiseDomainEvent(new TenantCreatedDomainEvent(tenant.Id, tenant.Name));

        return tenant; 
    }

    public Result<Updated> UpdateName(TenantName newName)
    {
        if (Name == newName) return Result.Updated;

        Name = newName;
        
        return Result.Updated;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
    public void RegenerateApiKey() => ApiKey = ApiKey.Generate();
}