using eCarbon.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eCarbon.Api.Common.Persistence.Configurations;

public class EmissionFactorConfiguration : IEntityTypeConfiguration<EmissionFactor>
{
    public void Configure(EntityTypeBuilder<EmissionFactor> builder)
    {
        builder.ToTable("emission_factors");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.ActivityType)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(x => x.Unit)
            .IsRequired()
            .HasMaxLength(20);
        
        builder.Property(x => x.KgCo2ePerUnit)
            .IsRequired()
            .HasPrecision(18, 8);
        
        builder.Property(x => x.Source)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(x => x.Year)
            .IsRequired();
        
        builder.Property(x => x.Region)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);
        
        builder.Property(x => x.CreatedAt)
            .IsRequired();
        
        builder.HasIndex(x => new { x.ActivityType, x.Unit, x.Year, x.Region })
            .IsUnique()
            .HasDatabaseName("UX_factors_activity_type_unit_year_region");
    }
}