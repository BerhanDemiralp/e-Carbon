using eCarbon.Api.Domain.Enums;

namespace eCarbon.Api.Features.ActivityRecords.CreateActivityRecord;

public class CreateActivityRecordRequest
{
    public Guid FacilityId { get; set; }
    public DateTime ActivityDate { get; set; }
    public ScopeType Scope { get; set; }
    public ActivityType ActivityType { get; set; }
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;

    public CreateActivityRecordCommand ToCommand() => new(
        FacilityId, ActivityDate, Scope, ActivityType, Quantity, Unit);
}