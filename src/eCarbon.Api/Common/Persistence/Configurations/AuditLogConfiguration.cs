using eCarbon.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eCarbon.Api.Common.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_logs");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Actor)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(x => x.Action)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(x => x.EntityType)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(x => x.EntityId)
            .IsRequired();
        
        builder.Property(x => x.Summary)
            .IsRequired()
            .HasMaxLength(1000);
        
        builder.Property(x => x.CreatedAt)
            .IsRequired();
        
        builder.HasIndex(x => new { x.EntityType, x.EntityId })
            .HasDatabaseName("IX_audit_entity");
    }
}