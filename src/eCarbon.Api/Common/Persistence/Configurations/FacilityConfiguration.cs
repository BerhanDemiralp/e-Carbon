using eCarbon.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eCarbon.Api.Common.Persistence.Configurations;

public class FacilityConfiguration : IEntityTypeConfiguration<Facility>
{
    public void Configure(EntityTypeBuilder<Facility> builder)
    {
        builder.ToTable("facilities");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(x => x.Location)
            .IsRequired()
            .HasMaxLength(300);
        
        builder.Property(x => x.CreatedAt)
            .IsRequired();
        
        builder.Property(x => x.UpdatedAt)
            .IsRequired();
        
        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);
        
        builder.HasOne(x => x.Company)
            .WithMany(c => c.Facilities)
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(x => x.CompanyId)
            .HasDatabaseName("IX_facilities_company_id");
        
        builder.HasIndex(x => new { x.CompanyId, x.Name })
            .IsUnique()
            .HasDatabaseName("UX_facilities_company_id_name");
        
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}