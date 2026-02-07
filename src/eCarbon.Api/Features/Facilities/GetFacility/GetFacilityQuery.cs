using MediatR;

namespace eCarbon.Api.Features.Facilities.GetFacility;

public record GetFacilityQuery(Guid Id) : IRequest<GetFacilityResponse>;

public record GetFacilityResponse(
    Guid Id,
    Guid CompanyId,
    string Name,
    string Location,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    int ActivityRecordCount);