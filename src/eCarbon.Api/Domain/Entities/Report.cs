namespace eCarbon.Api.Domain.Entities;

public class Report
{
    public Guid Id { get; set; }
    public Guid SnapshotId { get; set; }
    public string PdfPath { get; set; } = string.Empty;
    public string HashAlgorithm { get; set; } = string.Empty;
    public string HashValue { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public MonthlySnapshot Snapshot { get; set; } = null!;
}