using eCarbon.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace eCarbon.Api.Common.Persistence;

public static class UtcDateTimeConverter
{
    public static readonly ValueConverter<DateTime, DateTime> ValueConverter = new(
        v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
        v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
}

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Company> Companies { get; set; }
    public DbSet<Facility> Facilities { get; set; }
    public DbSet<EmissionFactor> EmissionFactors { get; set; }
    public DbSet<ActivityRecord> ActivityRecords { get; set; }
    public DbSet<MonthlySnapshot> MonthlySnapshots { get; set; }
    public DbSet<SnapshotItem> SnapshotItems { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(UtcDateTimeConverter.ValueConverter);
                }
            }
        }
        
        base.OnModelCreating(modelBuilder);
    }
}
