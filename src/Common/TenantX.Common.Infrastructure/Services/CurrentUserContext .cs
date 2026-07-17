using TenantX.Common.Domain.Shared;

namespace TenantX.Common.Infrastructure.Services;

public sealed class CurrentUserContext : ICurrentUserContext
{
	public TenantId TenantId => TenantId.New();


	public bool IsAuthenticated => true;
	public UserId UserId => UserId.From(Guid.Parse("ac133ab2-1f86-4c25-9c09-8c6acea4dd17"));

}
