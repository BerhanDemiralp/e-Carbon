namespace eCarbon.Api.Domain.Entities;

public class Facility
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    
    // Navigation properties
    public Company Company { get; set; } = null!;
    public ICollection<ActivityRecord> ActivityRecords { get; set; } = new List<ActivityRecord>();
}