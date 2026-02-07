using eCarbon.Api.Domain.Enums;

namespace eCarbon.Api.Domain.Entities;

public class SnapshotItem
{
    public Guid Id { get; set; }
    public Guid SnapshotId { get; set; }
    public Guid FacilityId { get; set; }
    public DateTime ActivityDate { get; set; }
    public ScopeType Scope { get; set; }
    public ActivityType ActivityType { get; set; }
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal FactorKgPerUnit { get; set; }
    public decimal Co2eKg { get; set; }
    public Guid SourceActivityRecordId { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public MonthlySnapshot Snapshot { get; set; } = null!;
    public Facility Facility { get; set; } = null!;
}