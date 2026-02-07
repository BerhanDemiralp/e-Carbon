using eCarbon.Api.Domain.Enums;
using MediatR;

namespace eCarbon.Api.Features.ActivityRecords.CreateActivityRecord;

public record CreateActivityRecordCommand(
    Guid FacilityId,
    DateTime ActivityDate,
    ScopeType Scope,
    ActivityType ActivityType,
    decimal Quantity,
    string Unit) : IRequest<CreateActivityRecordResponse>;

public record CreateActivityRecordResponse(
    Guid Id,
    Guid FacilityId,
    DateTime ActivityDate,
    ScopeType Scope,
    ActivityType ActivityType,
    decimal Quantity,
    string Unit,
    DateTime CreatedAt);