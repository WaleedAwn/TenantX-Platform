using Microsoft.EntityFrameworkCore;
using TenantX.Common.Domain;
using TenantX.Subscriptions.Application.Abstractions.Data;

namespace TenantX.Subscriptions.Infrastructure.Database;

public sealed class SubscriptionsDbContext(DbContextOptions<SubscriptionsDbContext> options)
    : DbContext(options), ISubscriptionsDbContext
{


  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.HasDefaultSchema(Schemas.Subscriptions);
    modelBuilder.Ignore<DomainEvent>();

    base.OnModelCreating(modelBuilder);
  }
  
}
