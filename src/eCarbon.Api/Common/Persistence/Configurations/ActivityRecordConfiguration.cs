using eCarbon.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eCarbon.Api.Common.Persistence.Configurations;

public class ActivityRecordConfiguration : IEntityTypeConfiguration<ActivityRecord>
{
    public void Configure(EntityTypeBuilder<ActivityRecord> builder)
    {
        builder.ToTable("activity_records");
        
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
        
        builder.Property(x => x.CreatedAt)
            .IsRequired();
        
        builder.Property(x => x.UpdatedAt)
            .IsRequired();
        
        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);
        
        builder.HasOne(x => x.Facility)
            .WithMany(f => f.ActivityRecords)
            .HasForeignKey(x => x.FacilityId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(x => new { x.FacilityId, x.ActivityDate })
            .HasDatabaseName("IX_activity_facility_date");
        
        builder.HasIndex(x => x.ActivityDate)
            .HasDatabaseName("IX_activity_date");
        
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}