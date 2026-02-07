using MediatR;

namespace eCarbon.Api.Features.Facilities.CreateFacility;

public record CreateFacilityCommand(Guid CompanyId, string Name, string Location) : IRequest<CreateFacilityResponse>;

public record CreateFacilityResponse(
    Guid Id,
    Guid CompanyId,
    string Name,
    string Location,
    DateTime CreatedAt);