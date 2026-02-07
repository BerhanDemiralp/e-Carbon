using MediatR;

namespace eCarbon.Api.Features.ActivityRecords.ListAllActivityRecords;

public record ListAllActivityRecordsQuery : IRequest<List<ListAllActivityRecordsResponse>>;

public record ListAllActivityRecordsResponse(
    Guid Id,
    Guid FacilityId,
    string FacilityName,
    Guid CompanyId,
    string CompanyName,
    DateTime ActivityDate,
    string ActivityType,
    int Scope,
    decimal Quantity,
    string Unit,
    DateTime CreatedAt);