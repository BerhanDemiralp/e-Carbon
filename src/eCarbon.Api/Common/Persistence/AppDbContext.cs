using eCarbon.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace eCarbon.Api.Common.Persistence;

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
        base.OnModelCreating(modelBuilder);
    }
}