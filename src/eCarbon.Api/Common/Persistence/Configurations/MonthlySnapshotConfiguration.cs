using eCarbon.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eCarbon.Api.Common.Persistence.Configurations;

public class MonthlySnapshotConfiguration : IEntityTypeConfiguration<MonthlySnapshot>
{
    public void Configure(EntityTypeBuilder<MonthlySnapshot> builder)
    {
        builder.ToTable("monthly_snapshots");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Month)
            .IsRequired()
            .HasMaxLength(7); // YYYY-MM format
        
        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(20);
        
        builder.Property(x => x.Scope1TotalKg)
            .IsRequired()
            .HasPrecision(18, 4);
        
        builder.Property(x => x.Scope2TotalKg)
            .IsRequired()
            .HasPrecision(18, 4);
        
        builder.Property(x => x.TotalKg)
            .IsRequired()
            .HasPrecision(18, 4);
        
        builder.Property(x => x.CreatedAt)
            .IsRequired();
        
        builder.HasOne(x => x.Company)
            .WithMany(c => c.MonthlySnapshots)
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(x => new { x.CompanyId, x.Month })
            .IsUnique()
            .HasDatabaseName("UX_snapshot_company_month");
    }
}