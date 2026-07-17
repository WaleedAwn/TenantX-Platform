using Microsoft.EntityFrameworkCore;

using TenantX.Common.Domain;
using TenantX.Subscriptions.Application.Abstractions.Data;
using TenantX.Subscriptions.Domain.Plans;
using TenantX.Subscriptions.Domain.Subscribers;
using TenantX.Subscriptions.Domain.Subscriptions;

namespace TenantX.Subscriptions.Infrastructure.Database;

public sealed class SubscriptionsDbContext(DbContextOptions<SubscriptionsDbContext> options)
    : DbContext(options), ISubscriptionsDbContext
{
  public DbSet<Subscription> Subscriptions { get; set; }
  public DbSet<Plan> Plans { get; set; }
  public DbSet<Subscriber> Subscribers { get; set; }
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.HasDefaultSchema(Schemas.Subscriptions);
    modelBuilder.Ignore<DomainEvent>();

    base.OnModelCreating(modelBuilder);
  }

}
