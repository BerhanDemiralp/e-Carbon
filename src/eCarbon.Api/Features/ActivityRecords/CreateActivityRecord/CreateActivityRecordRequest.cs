using eCarbon.Api.Domain.Enums;
using System.Text.Json.Serialization;

namespace eCarbon.Api.Features.ActivityRecords.CreateActivityRecord;

public class CreateActivityRecordRequest
{
    [JsonPropertyName("facilityId")]
    public Guid FacilityId { get; set; }
    
    [JsonPropertyName("activityDate")]
    public DateTime ActivityDate { get; set; }
    
    [JsonPropertyName("scope")]
    public ScopeType Scope { get; set; }
    
    [JsonPropertyName("activityType")]
    public ActivityType ActivityType { get; set; }
    
    [JsonPropertyName("quantity")]
    public decimal Quantity { get; set; }
    
    [JsonPropertyName("unit")]
    public string Unit { get; set; } = string.Empty;

    public CreateActivityRecordCommand ToCommand() => new(
        FacilityId, ActivityDate, Scope, ActivityType, Quantity, Unit);
}
