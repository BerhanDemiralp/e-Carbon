using eCarbon.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eCarbon.Api.Common.Persistence.Configurations;

public class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.ToTable("reports");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.PdfPath)
            .IsRequired()
            .HasMaxLength(500);
        
        builder.Property(x => x.HashAlgorithm)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(x => x.HashValue)
            .IsRequired()
            .HasMaxLength(256);
        
        builder.Property(x => x.CreatedAt)
            .IsRequired();
        
        builder.HasOne(x => x.Snapshot)
            .WithOne(s => s.Report)
            .HasForeignKey<Report>(x => x.SnapshotId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(x => x.SnapshotId)
            .IsUnique()
            .HasDatabaseName("UX_reports_snapshot_id");
    }
}