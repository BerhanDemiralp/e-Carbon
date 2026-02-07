using eCarbon.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eCarbon.Api.Common.Persistence.Configurations;

public class SnapshotItemConfiguration : IEntityTypeConfiguration<SnapshotItem>
{
    public void Configure(EntityTypeBuilder<SnapshotItem> builder)
    {
        builder.ToTable("snapshot_items");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.ActivityDate)
            .IsRequired();
        
        builder.Property(x => x.Scope)
            .IsRequired();
        
        builder.Property(x => x.ActivityType)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(x => x.Quantity)
            .IsRequired()
            .HasPrecision(18, 4);
        
        builder.Property(x => x.Unit)
            .IsRequired()
            .HasMaxLength(20);
        
        builder.Property(x => x.FactorKgPerUnit)
            .IsRequired()
            .HasPrecision(18, 8);
        
        builder.Property(x => x.Co2eKg)
            .IsRequired()
            .HasPrecision(18, 4);
        
        builder.Property(x => x.SourceActivityRecordId)
            .IsRequired();
        
        builder.Property(x => x.CreatedAt)
            .IsRequired();
        
        builder.HasOne(x => x.Snapshot)
            .WithMany(s => s.SnapshotItems)
            .HasForeignKey(x => x.SnapshotId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.Facility)
            .WithMany()
            .HasForeignKey(x => x.FacilityId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(x => x.SnapshotId)
            .HasDatabaseName("IX_snapshot_items_snapshot_id");
        
        builder.HasIndex(x => x.FacilityId)
            .HasDatabaseName("IX_snapshot_items_facility_id");
    }
}