using eCarbon.Api.Domain.Enums;

namespace eCarbon.Api.Domain.Entities;

public class EmissionFactor
{
    public Guid Id { get; set; }
    public ActivityType ActivityType { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal KgCo2ePerUnit { get; set; }
    public string Source { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Region { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}