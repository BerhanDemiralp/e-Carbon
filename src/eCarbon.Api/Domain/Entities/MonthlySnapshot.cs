using eCarbon.Api.Domain.Enums;

namespace eCarbon.Api.Domain.Entities;

public class MonthlySnapshot
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Month { get; set; } = string.Empty; // YYYY-MM format
    public SnapshotStatus Status { get; set; }
    public decimal Scope1TotalKg { get; set; }
    public decimal Scope2TotalKg { get; set; }
    public decimal TotalKg { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? FrozenAt { get; set; }
    
    // Navigation properties
    public Company Company { get; set; } = null!;
    public ICollection<SnapshotItem> SnapshotItems { get; set; } = new List<SnapshotItem>();
    public Report? Report { get; set; }
}