namespace eCarbon.Api.Domain.Entities;

public class Company
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    
    // Navigation properties
    public ICollection<Facility> Facilities { get; set; } = new List<Facility>();
    public ICollection<MonthlySnapshot> MonthlySnapshots { get; set; } = new List<MonthlySnapshot>();
}