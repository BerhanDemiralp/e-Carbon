using MediatR;

namespace eCarbon.Api.Features.Facilities.UpdateFacility;

public record UpdateFacilityCommand(Guid Id, string Name, string Location) : IRequest<UpdateFacilityResponse>;

public record UpdateFacilityResponse(
    Guid Id,
    Guid CompanyId,
    string Name,
    string Location,
    DateTime UpdatedAt);