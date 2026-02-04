using eCarbon.Api.Domain.Enums;

namespace eCarbon.Api.Domain.Entities;

public class ActivityRecord
{
    public Guid Id { get; set; }
    public Guid FacilityId { get; set; }
    public DateTime ActivityDate { get; set; }
    public ScopeType Scope { get; set; }
    public ActivityType ActivityType { get; set; }
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    
    // Navigation properties
    public Facility Facility { get; set; } = null!;
}