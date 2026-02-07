using MediatR;

namespace eCarbon.Api.Features.ActivityRecords.ListActivityRecordsByFacility;

public record ListActivityRecordsByFacilityQuery(Guid FacilityId) : IRequest<List<ListActivityRecordsByFacilityResponse>>;

public record ListActivityRecordsByFacilityResponse(
    Guid Id,
    DateTime ActivityDate,
    string ActivityType,
    int Scope,
    decimal Quantity,
    string Unit,
    DateTime CreatedAt);