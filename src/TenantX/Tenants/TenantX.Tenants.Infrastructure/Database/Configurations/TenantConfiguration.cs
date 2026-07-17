

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TenantX.Common.Domain.Constants;
using TenantX.Common.Domain.Shared;
using TenantX.Tenants.Domain.Tenants;


namespace TenantX.Tenants.Infrastructure.Database.Configurations;

public sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {

        builder.HasKey(t => t.Id);

        // Strongly-Typed ID Conversion
        builder.Property(t => t.Id)
            .HasConversion(id => id.Value, value => TenantId.From(value))
            .ValueGeneratedNever();

        // Value Object: TenantName (Owned Entity)
        builder.OwnsOne(t => t.Name, nameBuilder =>
        {
            nameBuilder.Property(n => n.Value)
                .IsRequired()
                .HasMaxLength(ValidationConstants.Names.MaxLength);
        });

        // Value Object: ApiKey (Owned Entity)
        builder.OwnsOne(t => t.ApiKey, keyBuilder =>
        {
            keyBuilder.Property(k => k.Value)
                .IsRequired();

            // API Key should be unique for security
            keyBuilder.HasIndex(k => k.Value).IsUnique();
        });

        builder.Property(t => t.IsActive)
            .IsRequired();

        // Audit Fields (Inherited from AuditableEntity via AggregateRoot)
        builder.Property(t => t.CreatedAt).IsRequired();
        builder.Property(t => t.CreatedBy)
            .HasConversion(id => id.Value, value => UserId.From(value))
            .IsRequired();

        builder.Property(t => t.ModifiedAt);
        builder.Property(t => t.ModifiedBy)
            .HasConversion(id => id!.Value, value => UserId.From(value));

        // Indexes for performance
        builder.HasIndex(t => t.IsActive)
               .HasDatabaseName("ix_tenants_is_active");
    }
}
