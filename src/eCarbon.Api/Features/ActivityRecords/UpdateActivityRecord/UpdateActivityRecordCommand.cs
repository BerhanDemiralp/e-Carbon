using eCarbon.Api.Domain.Enums;
using MediatR;

namespace eCarbon.Api.Features.ActivityRecords.UpdateActivityRecord;

public record UpdateActivityRecordCommand(
    Guid Id,
    DateTime ActivityDate,
    ScopeType Scope,
    ActivityType ActivityType,
    decimal Quantity,
    string Unit) : IRequest<UpdateActivityRecordResponse>;

public record UpdateActivityRecordResponse(
    Guid Id,
    Guid FacilityId,
    DateTime ActivityDate,
    ScopeType Scope,
    ActivityType ActivityType,
    decimal Quantity,
    string Unit,
    DateTime UpdatedAt);