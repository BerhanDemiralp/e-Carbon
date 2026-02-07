using eCarbon.Api.Domain.Enums;

namespace eCarbon.Api.Features.ActivityRecords.UpdateActivityRecord;

public class UpdateActivityRecordRequest
{
    public DateTime ActivityDate { get; set; }
    public ScopeType Scope { get; set; }
    public ActivityType ActivityType { get; set; }
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;

    public UpdateActivityRecordCommand ToCommand(Guid id) => new(
        id, ActivityDate, Scope, ActivityType, Quantity, Unit);
}